#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

// Crea una texture "packed" per i materiali URP Lit:
// R = Metallic, G = Ambient Occlusion, B = 0, A = Smoothness (1 - Roughness).
// Usatela sia come Metallic Map sia come Occlusion Map del materiale.
public class MaskMapPacker : EditorWindow
{
    Texture2D roughnessTex;
    Texture2D aoTex;
    Texture2D metallicTex;

    [MenuItem("Tools/Mask Map Packer")]
    static void Open()
    {
        GetWindow<MaskMapPacker>("Mask Map Packer");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("R = Metallic, G = AO, A = Smoothness", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        roughnessTex = (Texture2D)EditorGUILayout.ObjectField("Roughness (rough)", roughnessTex, typeof(Texture2D), false);
        aoTex = (Texture2D)EditorGUILayout.ObjectField("Ambient Occlusion (ao)", aoTex, typeof(Texture2D), false);
        metallicTex = (Texture2D)EditorGUILayout.ObjectField("Metallic (opzionale)", metallicTex, typeof(Texture2D), false);

        EditorGUILayout.Space();
        using (new EditorGUI.DisabledScope(roughnessTex == null))
        {
            if (GUILayout.Button("Crea texture packed", GUILayout.Height(30)))
            {
                Pack();
            }
        }
    }

    // Legge la texture tramite la GPU, cosi' funziona con qualsiasi formato
    // che Unity sa importare (PNG, JPG, EXR, TGA...). Per leggere i valori
    // originali disattiva temporaneamente sRGB e compressione, poi li ripristina.
    static Texture2D Load(Texture2D asset)
    {
        if (asset == null) return null;

        string path = AssetDatabase.GetAssetPath(asset);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog("Mask Map Packer", "Impossibile leggere " + path + ".", "OK");
            return null;
        }

        bool oldSrgb = importer.sRGBTexture;
        var oldCompression = importer.textureCompression;
        bool changed = oldSrgb || oldCompression != TextureImporterCompression.Uncompressed;
        if (changed)
        {
            importer.sRGBTexture = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        int w = asset.width;
        int h = asset.height;
        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        var previous = RenderTexture.active;
        Graphics.Blit(asset, rt);
        RenderTexture.active = rt;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false, true);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        if (changed)
        {
            importer.sRGBTexture = oldSrgb;
            importer.textureCompression = oldCompression;
            importer.SaveAndReimport();
        }
        return tex;
    }

    void Pack()
    {
        Texture2D rough = Load(roughnessTex);
        if (rough == null) return;
        Texture2D ao = Load(aoTex);
        Texture2D metal = Load(metallicTex);

        int w = rough.width;
        int h = rough.height;
        bool aoMismatch = ao != null && (ao.width != w || ao.height != h);
        bool metalMismatch = metal != null && (metal.width != w || metal.height != h);
        if (aoMismatch || metalMismatch)
        {
            EditorUtility.DisplayDialog("Mask Map Packer", "Le immagini devono avere la stessa risoluzione (per esempio tutte 2K).", "OK");
            Cleanup(rough, ao, metal);
            return;
        }

        Color32[] r = rough.GetPixels32();
        Color32[] a = ao != null ? ao.GetPixels32() : null;
        Color32[] m = metal != null ? metal.GetPixels32() : null;

        var packed = new Color32[r.Length];
        for (int i = 0; i < r.Length; i++)
        {
            byte metallic = m != null ? m[i].r : (byte)0;
            byte occlusion = a != null ? a[i].r : (byte)255;
            byte smoothness = (byte)(255 - r[i].r);
            packed[i] = new Color32(metallic, occlusion, 0, smoothness);
        }

        var result = new Texture2D(w, h, TextureFormat.RGBA32, false, true);
        result.SetPixels32(packed);
        result.Apply();

        string srcPath = AssetDatabase.GetAssetPath(roughnessTex);
        string dir = Path.GetDirectoryName(srcPath).Replace("\\", "/");
        string baseName = Path.GetFileNameWithoutExtension(srcPath).Replace("_rough", "");
        string outPath = AssetDatabase.GenerateUniqueAssetPath(dir + "/" + baseName + "_mask.png");

        File.WriteAllBytes(outPath, result.EncodeToPNG());
        Cleanup(rough, ao, metal);
        DestroyImmediate(result);

        AssetDatabase.ImportAsset(outPath);
        var importer = (TextureImporter)AssetImporter.GetAtPath(outPath);
        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = false;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = false;
        importer.SaveAndReimport();

        var created = AssetDatabase.LoadAssetAtPath<Texture2D>(outPath);
        Selection.activeObject = created;
        EditorGUIUtility.PingObject(created);
        Debug.Log("Texture packed creata: " + outPath);
    }

    static void Cleanup(params Texture2D[] textures)
    {
        foreach (var t in textures)
        {
            if (t != null) DestroyImmediate(t);
        }
    }
}
#endif
