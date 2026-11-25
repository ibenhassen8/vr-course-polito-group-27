#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

// Unisce un'immagine di colore (RGB) e una di trasparenza (in bianco e nero)
// in un'unica texture RGBA, da usare come Base Map con Alpha Clipping in URP Lit.
// Bianco nell'alpha = visibile, nero = trasparente.
public class AlphaCombiner : EditorWindow
{
    Texture2D colorTex;
    Texture2D alphaTex;

    [MenuItem("Tools/Alpha Combiner")]
    static void Open()
    {
        GetWindow<AlphaCombiner>("Alpha Combiner");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("RGB = Colore, A = Trasparenza", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        colorTex = (Texture2D)EditorGUILayout.ObjectField("Colore (diff)", colorTex, typeof(Texture2D), false);
        alphaTex = (Texture2D)EditorGUILayout.ObjectField("Trasparenza (alpha)", alphaTex, typeof(Texture2D), false);

        EditorGUILayout.Space();
        using (new EditorGUI.DisabledScope(colorTex == null || alphaTex == null))
        {
            if (GUILayout.Button("Crea texture con trasparenza", GUILayout.Height(30)))
            {
                Combine();
            }
        }
    }

    // Legge i valori originali dell'immagine passando dalla GPU.
    // Disattiva temporaneamente sRGB e compressione, poi li ripristina.
    static Texture2D Load(Texture2D asset)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog("Alpha Combiner", "Impossibile leggere " + path + ".", "OK");
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

    void Combine()
    {
        Texture2D color = Load(colorTex);
        if (color == null) return;
        Texture2D alpha = Load(alphaTex);
        if (alpha == null)
        {
            DestroyImmediate(color);
            return;
        }

        if (color.width != alpha.width || color.height != alpha.height)
        {
            EditorUtility.DisplayDialog("Alpha Combiner", "Le due immagini devono avere la stessa risoluzione (per esempio entrambe 2K).", "OK");
            DestroyImmediate(color);
            DestroyImmediate(alpha);
            return;
        }

        Color32[] c = color.GetPixels32();
        Color32[] a = alpha.GetPixels32();
        var result = new Color32[c.Length];
        for (int i = 0; i < c.Length; i++)
        {
            result[i] = new Color32(c[i].r, c[i].g, c[i].b, a[i].r);
        }

        var output = new Texture2D(color.width, color.height, TextureFormat.RGBA32, false, true);
        output.SetPixels32(result);
        output.Apply();

        string srcPath = AssetDatabase.GetAssetPath(colorTex);
        string dir = Path.GetDirectoryName(srcPath).Replace("\\", "/");
        string baseName = Path.GetFileNameWithoutExtension(srcPath);
        string outPath = AssetDatabase.GenerateUniqueAssetPath(dir + "/" + baseName + "_alpha.png");

        File.WriteAllBytes(outPath, output.EncodeToPNG());
        DestroyImmediate(color);
        DestroyImmediate(alpha);
        DestroyImmediate(output);

        AssetDatabase.ImportAsset(outPath);
        var importer = (TextureImporter)AssetImporter.GetAtPath(outPath);
        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = true;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();

        var created = AssetDatabase.LoadAssetAtPath<Texture2D>(outPath);
        Selection.activeObject = created;
        EditorGUIUtility.PingObject(created);
        Debug.Log("Texture con trasparenza creata: " + outPath);
    }
}
#endif
