using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(Terrain))]
public sealed class TerrainTrackMask : MonoBehaviour
{
    [SerializeField, Min(256)] private int _resolution = 2048;
    [SerializeField] private Color _trackColor = new Color(0.45f, 0.45f, 0.45f, 1f);
    [SerializeField, Range(0f, 1f)] private float _trackStrength = 0.65f;
    [SerializeField] private Shader _stampShader;

    private Terrain _terrain;
    private RenderTexture _mask;
    private Material _stampMaterial;
    private MaterialPropertyBlock _stampProperties;
    private CommandBuffer _commandBuffer;
    private Mesh _stampMesh;
    private bool _hasPendingDraws;

    private readonly int _trackMaskID = Shader.PropertyToID("_TrackMask");
    private readonly int _trackWorldMinID = Shader.PropertyToID("_TrackWorldMin");
    private readonly int _trackWorldInvSizeID = Shader.PropertyToID("_TrackWorldInvSize");
    private readonly int TrackColorID = Shader.PropertyToID("_TrackColor");
    private readonly int TrackStrengthID = Shader.PropertyToID("_TrackStrength");

    private readonly int _segmentLengthID = Shader.PropertyToID("_SegmentLength");
    private readonly int _trackHalfWidthID = Shader.PropertyToID("_TrackHalfWidth");
    private readonly int _trackEdgeSoftnessID = Shader.PropertyToID("_TrackEdgeSoftness");

    public RenderTexture Mask => _mask;

    private void Awake()
    {
        _terrain = GetComponent<Terrain>();
        EnsureMaskCreated();
        PublishShaderGlobals();
        InitializeStampResources();
    }

    private void OnEnable()
    {
        if (_terrain == null)
            _terrain = GetComponent<Terrain>();

        EnsureMaskCreated();
        PublishShaderGlobals();
    }

    private void OnDestroy()
    {
        Shader.SetGlobalTexture(_trackMaskID, Texture2D.blackTexture);

        if (_mask == null)
            return;

        if (_commandBuffer != null)
            _commandBuffer.Release();

        if (_stampMaterial != null)
            Destroy(_stampMaterial);

        if (_stampMesh != null)
            Destroy(_stampMesh);

        _mask.Release();
        Destroy(_mask);
        _mask = null;
    }

    private void LateUpdate()
    {
        if (!_hasPendingDraws)
            return;

        Graphics.ExecuteCommandBuffer(_commandBuffer);

        _commandBuffer.Clear();
        _hasPendingDraws = false;
    }

    private void EnsureMaskCreated()
    {
        if (_mask != null && _mask.IsCreated())
            return;

        GraphicsFormat format = SystemInfo.IsFormatSupported(GraphicsFormat.R8_UNorm, FormatUsage.Render)
            ? GraphicsFormat.R8_UNorm
            : GraphicsFormat.R8G8B8A8_UNorm;

        RenderTextureDescriptor descriptor = new RenderTextureDescriptor(_resolution, _resolution, format, 0)
        {
            msaaSamples = 1,
            useMipMap = false,
            autoGenerateMips = false,
            enableRandomWrite = false,
            useDynamicScale = false
        };

        _mask = new RenderTexture(descriptor)
        {
            name = "Terrain Track Mask",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        _mask.Create();
        ClearMask();
    }

    private void PublishShaderGlobals()
    {
        Vector3 position = _terrain.transform.position;
        Vector3 size = _terrain.terrainData.size;

        Shader.SetGlobalTexture(_trackMaskID, _mask);
        Shader.SetGlobalVector(_trackWorldMinID, new Vector4(position.x, position.z, 0f, 0f));
        Shader.SetGlobalVector(_trackWorldInvSizeID, new Vector4(1f / size.x, 1f / size.z, 0f, 0f));
        Shader.SetGlobalColor(TrackColorID, _trackColor);
        Shader.SetGlobalFloat(TrackStrengthID, _trackStrength);
    }

    public void ClearMask()
    {
        RenderTexture previous = RenderTexture.active;

        RenderTexture.active = _mask;
        GL.Clear(false, true, Color.black);
        RenderTexture.active = previous;
    }

    private void InitializeStampResources()
    {
        if (_stampShader == null)
        {
            Debug.LogError("Track stamp shader is not assigned.", this);
            return;
        }

        if (_stampMaterial == null)
        {
            _stampMaterial = new Material(_stampShader);
            _stampMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        if (_stampProperties == null)
            _stampProperties = new MaterialPropertyBlock();

        if (_commandBuffer == null)
        {
            _commandBuffer = new CommandBuffer();
            _commandBuffer.name = "Draw Terrain Track";
        }

        if (_stampMesh == null)
            _stampMesh = CreateStampMesh();
    }

    private Mesh CreateStampMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Terrain Track Stamp";

        mesh.vertices = new[]
        {
        new Vector3(-0.5f, 0f, -0.5f),
        new Vector3(0.5f, 0f, -0.5f),
        new Vector3(-0.5f, 0f, 0.5f),
        new Vector3(0.5f, 0f, 0.5f)
    };

        mesh.uv = new[]
        {
        new Vector2(0f, 0f),
        new Vector2(1f, 0f),
        new Vector2(0f, 1f),
        new Vector2(1f, 1f)
    };

        mesh.triangles = new[] { 0, 2, 1, 1, 2, 3 };
        mesh.UploadMeshData(true);

        return mesh;
    }

    public void QueueSegment(Vector3 start, Vector3 end, float width, float edgeSoftness)
    {
        EnsureMaskCreated();
        InitializeStampResources();

        if (_stampMaterial == null)
            return;

        Vector3 direction = end - start;
        direction.y = 0f;

        float length = direction.magnitude;

        if (length < 0.001f || width <= 0f)
            return;

        float halfWidth = width * 0.5f;
        edgeSoftness = Mathf.Clamp(edgeSoftness, 0.001f, halfWidth);

        Vector3 center = (start + end) * 0.5f;
        Quaternion rotation = Quaternion.LookRotation(direction / length, Vector3.up);
        Matrix4x4 matrix = Matrix4x4.TRS(center, rotation, new Vector3(width, 1f, length + width));

        _stampProperties.Clear();
        _stampProperties.SetFloat(_segmentLengthID, length);
        _stampProperties.SetFloat(_trackHalfWidthID, halfWidth);
        _stampProperties.SetFloat(_trackEdgeSoftnessID, edgeSoftness);

        if (!_hasPendingDraws)
        {
            _commandBuffer.Clear();
            _commandBuffer.SetRenderTarget(_mask, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
            _hasPendingDraws = true;
        }

        _commandBuffer.DrawMesh(_stampMesh, matrix, _stampMaterial, 0, 0, _stampProperties);
    }
}