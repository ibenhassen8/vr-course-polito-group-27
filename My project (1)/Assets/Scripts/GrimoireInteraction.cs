using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Mettete questo script sul libro (OpenAntiqueBook).
// Quando il giocatore e' vicino compare "Premi X per leggere la ricetta".
// Premendo X si apre la pagina con la ricetta; premendo di nuovo X si chiude.
public class GrimoireInteraction : MonoBehaviour
{
    [Header("Distanza")]
    [Tooltip("Entro quanti metri dal libro compare la scritta.")]
    public float distanzaInterazione = 3f;

    [Header("Testi")]
    public string messaggio = "Premi X per leggere la ricetta";
    public string titolo = "Luce di Lucciole";

    [TextArea(8, 20)]
    public string ricetta =
        "Per risvegliare il bosco addormentato, raccogli:\n\n" +
        "  - una goccia di Rugiada di Luna, che brilla solo di notte\n" +
        "  - tre Bacche di Ginepro Argentato\n" +
        "  - una Piuma di Gufo Reale\n\n" +
        "Getta nel calderone, in quest'ordine:\n" +
        "  1. la Rugiada di Luna\n" +
        "  2. le Bacche di Ginepro\n" +
        "  3. la Piuma di Gufo\n\n" +
        "Attenta ai funghi velenosi e ai sassi comuni:\n" +
        "il calderone non perdona gli errori.";

    bool vicino;
    bool aperto;
    Transform giocatore;
    GUIStyle stileMessaggio, stileTitolo, stileTesto, stilePiccolo;

    void Update()
    {
        if (giocatore == null && Camera.main != null)
            giocatore = Camera.main.transform;
        if (giocatore == null) return;

        vicino = Vector3.Distance(giocatore.position, transform.position) <= distanzaInterazione;
        if (!vicino) aperto = false;

        if (vicino && PremutoX())
            aperto = !aperto;
    }

    bool PremutoX()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.X);
#endif
    }

    void PreparaStili()
    {
        if (stileMessaggio != null) return;

        stileMessaggio = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.85f, 0.95f, 1f) }
        };
        stileTitolo = new GUIStyle(GUI.skin.label)
        {
            fontSize = 34,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.25f, 0.15f, 0.05f) }
        };
        stileTesto = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            wordWrap = true,
            normal = { textColor = new Color(0.2f, 0.12f, 0.05f) }
        };
        stilePiccolo = new GUIStyle(stileTesto)
        {
            fontSize = 16,
            alignment = TextAnchor.LowerCenter,
            fontStyle = FontStyle.Italic
        };
    }

    void OnGUI()
    {
        if (!vicino) return;
        PreparaStili();

        if (!aperto)
        {
            Rect r = new Rect(0, Screen.height * 0.75f, Screen.width, 50);
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(Screen.width * 0.3f, r.y, Screen.width * 0.4f, r.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(r, messaggio, stileMessaggio);
            return;
        }

        float w = Mathf.Min(700, Screen.width * 0.8f);
        float h = Mathf.Min(560, Screen.height * 0.85f);
        Rect pagina = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);

        // Sfondo color pergamena
        GUI.color = new Color(0.93f, 0.86f, 0.68f, 0.97f);
        GUI.DrawTexture(pagina, Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Label(new Rect(pagina.x, pagina.y + 20, w, 50), titolo, stileTitolo);
        GUI.Label(new Rect(pagina.x + 40, pagina.y + 90, w - 80, h - 150), ricetta, stileTesto);
        GUI.Label(new Rect(pagina.x, pagina.y, w, h - 15), "Premi X per chiudere", stilePiccolo);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, distanzaInterazione);
    }
}
