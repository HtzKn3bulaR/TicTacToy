using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelector : MonoBehaviour
{
    public TMP_Dropdown classMenu;
    [SerializeField] Button helpClose;
    [SerializeField] Button helpOpen;
    public TextMeshProUGUI carClassText;
    public TextMeshProUGUI car1Text;
    public TextMeshProUGUI car2Text;
    public TextMeshProUGUI car3Text;
    public TextMeshProUGUI car4Text;
    public GameObject carClassMenu;
    public GameObject carTable;
    [SerializeField] GameObject helpScreen;
    private int classSelected;
    public Button goToNameSelectScreen;
    [SerializeField] Button carClassConfirm;
    List<string> activeList;

    [SerializeField] string[] carClasses = { "Rookie", "Amateur", "Advanced", "Semi-Pro", "Pro", "Super-Pro" };

    List<string> rookieList = new List<string>

    {
        "Toukka 4x4","Starfire GT","Lancer","El Gekko","Condor GRV","Junky","Rouge","Get Air","BigVolt","Road Star","Sunset Light","Show-Off","Nimbus","Harvester","Rebound 4x4","Albatross GT",
        "Updraft","Chubble","El Rapido","Vaanbus","Blobster","Col. Moss","Angus 400","Nesbitt","Hot Spot","Micro","Phat Slug","Hurricane","LR 64","Super Wheat","Dust Mite","High-Rod",
        "Crazy Pat","Myrmech","Mr. Bedtime","Tesla","Funziona","Phat Trucker","Splat","Panorama","Ciagnik","Genghis Kar","Quaqa Turbo","Volken Turbo","HSF-1","Pipsqueak","Naranja Turbo","RC Phink","E-Razr"
    };

    List<string> amateurList = new List<string>

    {
        "RCBandit","Dr.Grudge","SprinterXL","CandyPebbles","Mouse","Evil Weasel","NY 54","Rotor","LA 54","Groovster","RVLoco","AMCOTC","BadBison","BaddRC","Baja Dash","Breadfast","Bumblebee","Eatium",
        "Emilia","Exceed","Flatter 4V","Frograph","Fun Zone","Harmor","Honeybee","Hotknife","Ignit-9","Koin Karp","Kyarus","LMW","Locker","Madness","Manfred","Moby Trick","Mongoose","Muller GT","Nevermore","Nitro Crusher",
        "Off Gear","Phantum","Power Cap","Queen Bee","Red Kermit","Reddlum","Reliance","RoadKing","Silvarooky","Smokie","StarCarbs","Strax","Tempest","Toy-Volt Towing","Triton","Ultima","UltraGamma","Vixen","WildRide","Chapman"

    };

    List<string> advancedList = new List<string>

    {
        "Le Pastel","Aquasonic","Urban Jungle","Spearhead","Pest Control","DRJ-61","R6 Turbo","Whiplash","Springtrap","Hammerhead","Frostbite","Panga TC","APC L-13","Sturm","Lithmus","Prizmer","Bertha Ballistics",
        "Duck Sky","BossVolt","Raudy","Shocker","Romeo","Breaker","Drawall","Phenom","Frosted Delight","Grimlock","Recon MK1","Vibe Box","RC San","Rice Ball","Pole Poz","Fulon X","Matra XL","Alice","Wave Dancer","Junker",
        "Cerveth","Aerozad","Bajaette","Panther","Swizz Cheezer","75C","Akagi Attacker","Aquamarina","Bendor","Donnie TC","Ember","Emperor","Fierro","Flower Power","Hyper XL","Llag Sat","Marauder","Micro Tache","Sarge","Twilight GT"
    };

    List<string> semiProList = new List<string>

    {
        "Adeon","Zipper","Dual Signal","JG-7","Runner 2000","Sokudo","Dragoon","Serrate","BHV 1","Winger","Tribute","Acclaim GT","Victoria","Mambra","Max Attack","Jackal","Tri-Enter","Swede","Yuurei V8",
        "Sasquatch","Arnoux","Danger","Quazar","Ancile","Riptor","Voltz XL","RC-Erra","Norwood","Aeromaster","Bushido RS","Pemto","Nitromare","Rothams Racing","Iron-Z","Locust","AMW","Big Load",
        "Gravel Basher","Artifact","Cossie","LV 54","Karlington","Predator","RG1","Big Match Jim","Jet Astro","Blazar","CHC 305","Cobra Max","Fat Agnus","KC-3","Blaze V8","Chubba","Ducktail","Toy-World GT"
        ,"Current","Ballista"

    };

    List<string> proList = new List<string>

    {
        "Toyeca","Chimera TC","Drome Champ","Purp XL","Humma","Puma","Cougar","Outlaw","Sunrise","Mid-Musc","Ryu","Ayrton SP","S13 Alltune","SNW 35","Prime Target","Keyakizaka",
        "Visconti R","Indy B","Cintach","Wildstar","Artair","After Image","G3X","Panga","Velter Ultron","BajaVolt","Cherencov","Mean Streak","Patriot","Cerberus","Power Loader","RC Winglet","The Knight",
        "BanKing","Black Widow","Duflame","Eaglet","Electric Sheep","EXE TC","Gust","Hydro Flame","Jet Spike","Karen","Maverick","N-Sharp","Proto Combo","Quinx","RC Bulldog","RVRC 20","RVXXL 5","Sandstorm",
        "Shark Bite","Shinobi","Sir Gleam","Tizzoni"

    };

    List<string> superProList = new List<string>

    {
        "Endo","Stinger","Elyta","Calcure","Prototype FX77","Saeger","FLIR","Gungnir","P4 Super","Napalm","La Rossa","Cambold R","Sylea","King Kaiju","Selsia Turbo","Komet","Quicksilver","Armand","Commandine"
        ,"Starmac","Maxxas XLR8","XM250","Sentaro XL","Skarlet","AU-8","Revel","U.V.G.S.","Mudman","Dragheat","Reiser","Sterling F77","Wind Slicer","Rinne","Megalodon XL","Tesseract","Orbitron","Anaconda GT"
        ,"Identity X","Voltrex","Yinisa","King Moloko","Orion","Slingshot","Daemmon","Horizenna","Sideswipe","Spectron","Exclaim GT Mk.2","FD-400","Golden Eye","Hanabira","Hemera","Hetgarde GT1","Madax GT"
        ,"Nakajima","Nyx","Spedion","Hoshino","Nain"
    };

    List<T> GetUniqueRandomElements<T>(List<T> inputList, int count)

    {
        List<T> inputListClone = new List<T>(inputList);
        Shuffle(inputListClone);
        return inputListClone.GetRange(0, count);

    }

    void Shuffle<T>(List<T> inputList)

    {
        for (int i = 0; i < inputList.Count - 1; i++)

        {
            T temp = inputList[i];
            int rand = Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClassSelect()

    {
        carClassMenu.SetActive(false);
        carTable.SetActive(true);
        classSelected = (classMenu.value)-1;
        carClassText.text = carClasses[classSelected];
        MainManager.carClass = carClasses[classSelected];

    }

    public void CarSelect()


    {
        switch (classSelected)

        {
            case 0:
                    activeList = rookieList;
                break;

            case 1:
                activeList = amateurList;
                break;

            case 2:
                activeList = advancedList;
                break;

            case 3:
                activeList = semiProList;
                break;

            case 4:
                activeList = proList;
                break;

            case 5:
                activeList = superProList;
                break;

        }

        var uniqueRandomList = GetUniqueRandomElements(activeList, 4);

        for (int i = 0; i < uniqueRandomList.Count; i++)

        {
            MainManager.cars[i] = uniqueRandomList[i];
                     
                    }

        car1Text.gameObject.SetActive(true);
        car1Text.text = MainManager.cars[0];

        car2Text.gameObject.SetActive(true);
        car2Text.text = MainManager.cars[1];

        goToNameSelectScreen.gameObject.SetActive(true);
        carClassConfirm.gameObject.SetActive(false);
    }

    public void CloseHelp()

    {
        helpScreen.gameObject.SetActive(false);
        helpOpen.gameObject.SetActive(true);

    }

    public void OpenHelp()
    {
        helpScreen.gameObject.SetActive(true);
        helpOpen.gameObject.SetActive(false);

    }

           
    public void BackToStart()

    {
        SceneManager.LoadScene(0);

    }
    


    public void GoToNextScreen()

    {
        SceneManager.LoadScene(1);

    }




}
