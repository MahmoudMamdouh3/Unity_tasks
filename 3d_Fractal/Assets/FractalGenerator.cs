using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FractalGenerator : MonoBehaviour
{
    [Header("Fractal Settings")]
    [SerializeField] private GameObject branchPrefab;
    [Range(1, 11)] [SerializeField] private int maxIterations = 7;

    [Header("Appearance Details")]
    [Range(0.1f, 0.95f)] [SerializeField] private float childScale = 0.7f;
    [Range(0f, 90f)] [SerializeField] private int splitAngle = 35;
    
    [Tooltip("Colors the fractal based on how deep the branch is.")]
    [SerializeField] private Gradient treeColors;

    private GameObject _fractalContainer;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private void Start()
    {
        GenerateFractal();
    }

    [ContextMenu("Generate New Fractal")]
    public void GenerateFractal()
    {
        if (_fractalContainer != null)
        {
            if (Application.isPlaying) Destroy(_fractalContainer);
            else DestroyImmediate(_fractalContainer);
        }

        if (branchPrefab == null) return;

        _fractalContainer = new GameObject("Fractal_Root_Container");
        _fractalContainer.transform.position = this.transform.position;

        GrowRecursive(_fractalContainer.transform, 0);
    }

    private void GrowRecursive(Transform currentParent, int currentIteration)
    {
        if (currentIteration >= maxIterations) return;

        SpawnSingleBranch(currentParent, currentIteration, -splitAngle);
        SpawnSingleBranch(currentParent, currentIteration, splitAngle);
    }

    private void SpawnSingleBranch(Transform parent, int iteration, float angleTiltX)
    {
        GameObject newBranch = Instantiate(branchPrefab, parent);

        newBranch.transform.localPosition = Vector3.up;
        newBranch.transform.localRotation = Quaternion.Euler(angleTiltX, 0, 0);
        newBranch.transform.localScale = Vector3.one * childScale;

        // Pass the new branch to the color function
        ApplyColor(newBranch, iteration);

        GrowRecursive(newBranch.transform, iteration + 1);
    }

    private void ApplyColor(GameObject branch, int iteration)
    {
        if (treeColors == null) return;

        float t = (float)iteration / maxIterations;
        Color branchColor = treeColors.Evaluate(t);

        MeshRenderer meshRenderer = branch.GetComponentInChildren<MeshRenderer>();
        
        if (meshRenderer != null)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetColor(BaseColorId, branchColor); 
            meshRenderer.SetPropertyBlock(block);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FractalGenerator))]
    public class FractalGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            FractalGenerator script = (FractalGenerator)target;
            GUILayout.Space(10);
            if (GUILayout.Button("Regenerate Fractal Now", GUILayout.Height(30)))
            {
                script.GenerateFractal();
            }
        }
    }
#endif
}