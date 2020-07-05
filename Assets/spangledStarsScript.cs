using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class spangledStarsScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Stars;
    public Material[] Colors; //ROYGBIVW
    public TextMesh[] OutsideLetters;
    public TextMesh[] InsideLetters;
    public GameObject Front;
    public GameObject Back;

    private List<int> ColorOrder = new List<int>{0,1,2,3,4,5,6};
    //private List<string> ModNames = new List<string>{};
    int KeyModules = 0;
    int ForgetModules = 0;
    int SpangledModules = 0;
    int SequenceStart = 0;
    int Offset = 0;
    int Presses = 0;
    int ModNames = 0;
    string KeyOrder = "";
    string Words = "O#SCYSBTDALLWSPLWHATTLLGIWBSABSTTPILFOTRPWWWSGLLSIATRERGTBBSIAGPTTNTOFWSTOSDTTSSGBNEYTWVOTLNOTFATHOTB";
    string Keys =  "gecegcedcefgggedcbcbccegcgecegcedcefgggedcbabccegceeefggfedefffedcbabcefggcccbaaadfedccbggcdefgcdefdc";
    string UsedWords = "";
    string UsedKeys = "";
    string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string TopThreeStars = "";
    string SortedTopThree = "";
    string Input = "";

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable star in Stars) {
            star.OnInteract += delegate () { starPress(star); return false; };
        }
    }

    // Use this for initialization
    void Start () {
        ColorOrder.Shuffle();
        for (int i = 0; i < 7; i++) {
            Stars[i].GetComponent<MeshRenderer>().material = Colors[ColorOrder[i]];
            switch (ColorOrder[i]) {
                case 0: InsideLetters[i].text = "R"; break;
                case 1: InsideLetters[i].text = "O"; break;
                case 2: InsideLetters[i].text = "Y"; break;
                case 3: InsideLetters[i].text = "G"; break;
                case 4: InsideLetters[i].text = "B"; break;
                case 5: InsideLetters[i].text = "I"; break;
                case 6: InsideLetters[i].text = "V"; break;
                default: break;
            }
        }

        ModNames = Bomb.GetSolvableModuleNames().Count();

        for (int i = 0; i < ModNames; i++) {
            if(Bomb.GetModuleNames()[i].IndexOf("key",  StringComparison.InvariantCultureIgnoreCase) != -1)
    		{
    			KeyModules += 1;
    		}
            if(Bomb.GetModuleNames()[i].IndexOf("forget",  StringComparison.InvariantCultureIgnoreCase) != -1)
    		{
    			ForgetModules += 1;
    		}
            if(Bomb.GetModuleNames()[i] == "Spangled Stars")
    		{
    			SpangledModules += 1;
    		}
        }

        if (KeyModules != 0) {
            KeyOrder = "FACEDBG";
        } else if (SpangledModules % 2 == 0) {
            KeyOrder = "EBDAFCG";
        } else if (ForgetModules == 0) {
            KeyOrder = "CDEFGAB";
        } else {
            KeyOrder = "DCAGBFE";
        }

        Debug.LogFormat("[Spangled Stars #{0}] Keys of the stars in ROYGBIV order: {1}", moduleId, KeyOrder);

        SequenceStart = UnityEngine.Random.Range(0, 95);
        for (int i = 0; i < 6; i++) {
            UsedWords += Words[SequenceStart + i];
            UsedKeys += Keys[SequenceStart + i];
        }

        for (int i = 6; i < 9; i++) {
            TopThreeStars += KeyOrder[ColorOrder[i % 7]];
        }
        for (int i = 0; i < 7; i++) {
            if (TopThreeStars.IndexOf(Alphabet[i]) != -1) {
                SortedTopThree += Alphabet[i];
            }
        }

        Debug.LogFormat("[Spangled Stars #{0}] Keys used: {1}", moduleId, UsedKeys.ToUpper());
        Debug.LogFormat("[Spangled Stars #{0}] First letter of words used (not encrypted): {1}", moduleId, UsedWords);
        Debug.LogFormat("[Spangled Stars #{0}] Keys of the top three stars: {1}", moduleId, SortedTopThree);

        switch (SortedTopThree) {
            case "BCE":
            case "CEG": Offset = 1; break;
            case "ACD": Offset = 3; break;
            case "BCD": Offset = 5; break;
            case "BDE":
            case "CDG": Offset = 6; break;
            case "ADF":
            case "BEF": Offset = 7; break;
            case "CDE": Offset = 9; break;
            case "BCF":
            case "EFG": Offset = 10; break;
            case "ACE":
            case "BFG": Offset = 11; break;
            case "ABE":
            case "DEF": Offset = 12; break;
            case "ACG":
            case "ADG":
            case "BDG":
            case "DFG": Offset = 13; break;
            case "ABG":
            case "BEG":
            case "CDF":
            case "CEF": Offset = 14; break;
            case "BCG": Offset = 18; break;
            case "ABD": Offset = 19; break;
            case "AEF": Offset = 20; break;
            case "ACF":
            case "AFG":
            case "BDF":
            case "DEG": Offset = 22; break;
            case "ABC":
            case "CFG": Offset = 24; break;
            case "ABF":
            case "ADE":
            case "AEG": Offset = 25; break;
            default: break;
        }

        Debug.LogFormat("[Spangled Stars #{0}] Caesar offset: {1}", moduleId, Offset);

        for (int i = 0; i < 3; i++) {
            switch (UsedKeys[i]) {
                case 'c': OutsideLetters[KeyOrder.IndexOf('C')].text = OutsideLetters[KeyOrder.IndexOf('C')].text + Caesar(UsedWords[i]); break;
                case 'd': OutsideLetters[KeyOrder.IndexOf('D')].text = OutsideLetters[KeyOrder.IndexOf('D')].text + Caesar(UsedWords[i]); break;
                case 'e': OutsideLetters[KeyOrder.IndexOf('E')].text = OutsideLetters[KeyOrder.IndexOf('E')].text + Caesar(UsedWords[i]); break;
                case 'f': OutsideLetters[KeyOrder.IndexOf('F')].text = OutsideLetters[KeyOrder.IndexOf('F')].text + Caesar(UsedWords[i]); break;
                case 'g': OutsideLetters[KeyOrder.IndexOf('G')].text = OutsideLetters[KeyOrder.IndexOf('G')].text + Caesar(UsedWords[i]); break;
                case 'a': OutsideLetters[KeyOrder.IndexOf('A')].text = OutsideLetters[KeyOrder.IndexOf('A')].text + Caesar(UsedWords[i]); break;
                case 'b': OutsideLetters[KeyOrder.IndexOf('B')].text = OutsideLetters[KeyOrder.IndexOf('B')].text + Caesar(UsedWords[i]); break;
                default: break;
            }
        }

        StartCoroutine(Flashing());
    }

    void starPress(KMSelectable star) {
        star.AddInteractionPunch();
        for (int i = 0; i < 7; i++) {
            if (star == Stars[i]) {
                switch (KeyOrder[ColorOrder[i]].ToString()) {
                    case "C": Audio.PlaySoundAtTransform("C", transform); break;
                    case "D": Audio.PlaySoundAtTransform("D", transform); break;
                    case "E": Audio.PlaySoundAtTransform("E", transform); break;
                    case "F": Audio.PlaySoundAtTransform("F", transform); break;
                    case "G": Audio.PlaySoundAtTransform("G", transform); break;
                    case "A": Audio.PlaySoundAtTransform("A", transform); break;
                    case "B": Audio.PlaySoundAtTransform("B", transform); break;
                    default: break;
                }
                Input += KeyOrder[ColorOrder[i]].ToString().ToLower();
                Presses += 1;
                if (Presses == 3) {
                    Presses = 0;
                    if (Input == UsedKeys.Substring(3)) {
                        Debug.LogFormat("[Spangled Stars #{0}] {1} is correct, module solved.", moduleId, Input.ToUpper());
                        GetComponent<KMBombModule>().HandlePass();
                        Front.gameObject.SetActive(false);
                        Back.gameObject.SetActive(true);
                    } else {
                        Debug.LogFormat("[Spangled Stars #{0}] {1} is incorrect, strike!", moduleId, Input.ToUpper());
                        GetComponent<KMBombModule>().HandleStrike();
                        Input = "";
                    }
                }
            }
        }
    }

    IEnumerator Flashing () {
        //1st flash
        switch (UsedKeys[0]) {
            case 'c': Stars[KeyOrder.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'd': Stars[KeyOrder.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'e': Stars[KeyOrder.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'f': Stars[KeyOrder.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'g': Stars[KeyOrder.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'a': Stars[KeyOrder.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'b': Stars[KeyOrder.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.5f);
        switch (UsedKeys[0]) {
            case 'c': Stars[KeyOrder.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('C')]]; break;
            case 'd': Stars[KeyOrder.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('D')]]; break;
            case 'e': Stars[KeyOrder.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('E')]]; break;
            case 'f': Stars[KeyOrder.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('F')]]; break;
            case 'g': Stars[KeyOrder.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('G')]]; break;
            case 'a': Stars[KeyOrder.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('A')]]; break;
            case 'b': Stars[KeyOrder.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('B')]]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.1f);
        switch (UsedKeys[1]) {
            case 'c': Stars[KeyOrder.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'd': Stars[KeyOrder.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'e': Stars[KeyOrder.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'f': Stars[KeyOrder.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'g': Stars[KeyOrder.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'a': Stars[KeyOrder.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'b': Stars[KeyOrder.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.5f);
        switch (UsedKeys[1]) {
            case 'c': Stars[KeyOrder.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('C')]]; break;
            case 'd': Stars[KeyOrder.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('D')]]; break;
            case 'e': Stars[KeyOrder.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('E')]]; break;
            case 'f': Stars[KeyOrder.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('F')]]; break;
            case 'g': Stars[KeyOrder.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('G')]]; break;
            case 'a': Stars[KeyOrder.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('A')]]; break;
            case 'b': Stars[KeyOrder.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('B')]]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.1f);
        switch (UsedKeys[2]) {
            case 'c': Stars[KeyOrder.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'd': Stars[KeyOrder.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'e': Stars[KeyOrder.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'f': Stars[KeyOrder.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'g': Stars[KeyOrder.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'a': Stars[KeyOrder.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'b': Stars[KeyOrder.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.5f);
        switch (UsedKeys[2]) {
            case 'c': Stars[KeyOrder.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('C')]]; break;
            case 'd': Stars[KeyOrder.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('D')]]; break;
            case 'e': Stars[KeyOrder.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('E')]]; break;
            case 'f': Stars[KeyOrder.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('F')]]; break;
            case 'g': Stars[KeyOrder.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('G')]]; break;
            case 'a': Stars[KeyOrder.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('A')]]; break;
            case 'b': Stars[KeyOrder.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[KeyOrder.IndexOf('B')]]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(Flashing());
    }

    char Caesar (char L) {
        if (L != '#') {
            return Alphabet[Alphabet.IndexOf(L) + Offset];
        } else {
            return '#';
        }
    }
}
