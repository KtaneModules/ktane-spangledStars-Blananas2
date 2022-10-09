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
    string Words = "O#SCYSBTDELLWSPLWHATTLLGIWBSABSTTPILFOTRPWWWSGLLSIATRERGTBBSIAGPTTNTOFWSTOSDTTSSGBNEYTWVOTLNOTFATHOTB";
    string Keys =  "gecegcedcefgggedcbcbccegcgecegcedcefgggedcbabccegceeefggfedefffedcbabcefggcccbaaadfedccbggcdefgcdefdc";
    string UsedWords = "";
    string UsedKeys = "";
    string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string TopThreeStars = "";
    string SortedTopThree = "";
    string Input = "";
    string Keyboard = "ROYGBIV";
    string StarsToKeys = "";

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
        Debug.LogFormat("[Spangled Stars #{0}] Colors of stars in clockwise order: {1}{2}{3}{4}{5}{6}{7}", moduleId, Keyboard[ColorOrder[0]], Keyboard[ColorOrder[1]], Keyboard[ColorOrder[2]], Keyboard[ColorOrder[3]], Keyboard[ColorOrder[4]], Keyboard[ColorOrder[5]], Keyboard[ColorOrder[6]]);

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

        for (int i = 0; i < 7; i++) {
            StarsToKeys += KeyOrder[ColorOrder[i]];
        }

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
                case 'c': OutsideLetters[StarsToKeys.IndexOf('C')].text = OutsideLetters[StarsToKeys.IndexOf('C')].text + Caesar(UsedWords[i]); break;
                case 'd': OutsideLetters[StarsToKeys.IndexOf('D')].text = OutsideLetters[StarsToKeys.IndexOf('D')].text + Caesar(UsedWords[i]); break;
                case 'e': OutsideLetters[StarsToKeys.IndexOf('E')].text = OutsideLetters[StarsToKeys.IndexOf('E')].text + Caesar(UsedWords[i]); break;
                case 'f': OutsideLetters[StarsToKeys.IndexOf('F')].text = OutsideLetters[StarsToKeys.IndexOf('F')].text + Caesar(UsedWords[i]); break;
                case 'g': OutsideLetters[StarsToKeys.IndexOf('G')].text = OutsideLetters[StarsToKeys.IndexOf('G')].text + Caesar(UsedWords[i]); break;
                case 'a': OutsideLetters[StarsToKeys.IndexOf('A')].text = OutsideLetters[StarsToKeys.IndexOf('A')].text + Caesar(UsedWords[i]); break;
                case 'b': OutsideLetters[StarsToKeys.IndexOf('B')].text = OutsideLetters[StarsToKeys.IndexOf('B')].text + Caesar(UsedWords[i]); break;
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
        switch (UsedKeys[0]) {
            case 'c': Stars[StarsToKeys.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'd': Stars[StarsToKeys.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'e': Stars[StarsToKeys.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'f': Stars[StarsToKeys.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'g': Stars[StarsToKeys.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'a': Stars[StarsToKeys.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'b': Stars[StarsToKeys.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.5f);
        switch (UsedKeys[0]) {
            case 'c': Stars[StarsToKeys.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('C')]]; break;
            case 'd': Stars[StarsToKeys.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('D')]]; break;
            case 'e': Stars[StarsToKeys.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('E')]]; break;
            case 'f': Stars[StarsToKeys.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('F')]]; break;
            case 'g': Stars[StarsToKeys.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('G')]]; break;
            case 'a': Stars[StarsToKeys.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('A')]]; break;
            case 'b': Stars[StarsToKeys.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('B')]]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.1f);
        switch (UsedKeys[1]) {
            case 'c': Stars[StarsToKeys.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'd': Stars[StarsToKeys.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'e': Stars[StarsToKeys.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'f': Stars[StarsToKeys.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'g': Stars[StarsToKeys.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'a': Stars[StarsToKeys.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'b': Stars[StarsToKeys.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.5f);
        switch (UsedKeys[1]) {
            case 'c': Stars[StarsToKeys.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('C')]]; break;
            case 'd': Stars[StarsToKeys.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('D')]]; break;
            case 'e': Stars[StarsToKeys.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('E')]]; break;
            case 'f': Stars[StarsToKeys.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('F')]]; break;
            case 'g': Stars[StarsToKeys.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('G')]]; break;
            case 'a': Stars[StarsToKeys.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('A')]]; break;
            case 'b': Stars[StarsToKeys.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('B')]]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.1f);
        switch (UsedKeys[2]) {
            case 'c': Stars[StarsToKeys.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'd': Stars[StarsToKeys.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'e': Stars[StarsToKeys.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'f': Stars[StarsToKeys.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'g': Stars[StarsToKeys.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'a': Stars[StarsToKeys.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            case 'b': Stars[StarsToKeys.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[7]; break;
            default: break;
        }
        yield return new WaitForSeconds(0.5f);
        switch (UsedKeys[2]) {
            case 'c': Stars[StarsToKeys.IndexOf('C')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('C')]]; break;
            case 'd': Stars[StarsToKeys.IndexOf('D')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('D')]]; break;
            case 'e': Stars[StarsToKeys.IndexOf('E')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('E')]]; break;
            case 'f': Stars[StarsToKeys.IndexOf('F')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('F')]]; break;
            case 'g': Stars[StarsToKeys.IndexOf('G')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('G')]]; break;
            case 'a': Stars[StarsToKeys.IndexOf('A')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('A')]]; break;
            case 'b': Stars[StarsToKeys.IndexOf('B')].GetComponent<MeshRenderer>().material = Colors[ColorOrder[StarsToKeys.IndexOf('B')]]; break;
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

    //toilet paper support
    public string TwitchHelpMessage = "Use '!{0} press 1 2 3 4 5 6 7' to press stars 1, 2, 3, 4, 5, 6 and 7. The buttons are numbered from 1 to 7 going clockwise starting from the topmost.";

    public IEnumerator ProcessTwitchCommand(string command)
    {
		string commfinal=command.Replace("press ", "");
		string[] digitstring = commfinal.Split(' ');
		int tried;
		int index =1;
		foreach(string digit in digitstring){
			if(int.TryParse(digit, out tried)){
				if(index<=7){
					tried=int.Parse(digit);
					index+=1;
					if(tried<8){
						if(tried>0){
					yield return null;
					yield return Stars[tried-1];
					yield return Stars[tried-1];
						}
						else{
							yield return null;
							yield return "sendtochaterror Number too small!";
							yield break;
						}
					}
					else{
						yield return null;
						yield return "sendtochaterror Number too big!";
						yield break;
					}
				}
				else{
					yield return null;
					yield return "sendtochaterror Too many digits!";
					yield break;
				}
			}
			else{
				yield return null;
				yield return "sendtochaterror Digit not valid.";
				yield break;
			}
		}
	}

    IEnumerator TwitchHandleForcedSolve() {
        Stars[StarsToKeys.IndexOf(UsedKeys[3].ToString().ToUpper())].OnInteract();
        yield return new WaitForSeconds(0.1f);
        Stars[StarsToKeys.IndexOf(UsedKeys[4].ToString().ToUpper())].OnInteract();
        yield return new WaitForSeconds(0.1f);
        Stars[StarsToKeys.IndexOf(UsedKeys[5].ToString().ToUpper())].OnInteract();
    }
}
