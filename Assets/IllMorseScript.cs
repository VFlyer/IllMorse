using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;

public class IllMorseScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;

    public TextMesh MainDisplay;
    public TextMesh WordDisplay;
    public KMSelectable dotButton;
    public KMSelectable dashButton;
    public KMSelectable spaceButton;
    public KMSelectable nextButton;
    public KMSelectable submitButton;
    public Material[] LED;

    public GameObject[] Stages;
    public KMAudio Audio;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    String[] morseCombinations = {".", "-", ".-", "-.", "..", "--", "...", "---", ".--", "..-", "-..", "--.", "-.-", ".-.", 
    "....", "...-", "..--", ".---", "----", "---.", "--..", "-...", "..-.", ".-..", ".--.", "-.--", "--.-", "-..-",
    ".----", "..---", "...--", "....-", ".....", "-....", "--...", "---..", "----.", "-----", 
    "-...-", "-..--", "-.---", "-.-.-", "-.--.", "--.-.", "--..-", "-.-..", "-..-.", ".--..", ".-.-.", "..--.", "..-.-",
    ".-..-", ".---.", ".-.--", ".--.-", "...-.", "..-..", ".-..."};
    char[] letters = new char[26];
    string[] letterToMorseMap = new string[26];
    bool acceptingAnswer = false;
    string morseAnswer;

    int stageCounter = 0;
    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        GenerateLetters();
        DisplayRandomWord(letters, letterToMorseMap);

        
        dotButton.OnInteract += delegate () { DotButton(); return false; };
        dashButton.OnInteract += delegate () { DashButton(); return false; };
        spaceButton.OnInteract += delegate () { SpaceButton(); return false; };
        nextButton.OnInteract += delegate () { NextButton(); return false; };
        submitButton.OnInteract += delegate () { Submission(); return false; };
    }

    void Submission(){
        GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);
        if (!acceptingAnswer){
            Debug.LogFormat("[Ill Morse #{0}] Entering submission mode.", _moduleId);
            Debug.LogFormat("[Ill Morse #{0}] Stage {1}:", _moduleId, stageCounter + 1);
            acceptingAnswer = true; 
            DisplayRandomWord(letters, letterToMorseMap);
            MainDisplay.text = "";
        }
        else if (acceptingAnswer && stageCounter < 3 && stageCounter != 4){
            
            if (MainDisplay.text == morseAnswer){
                Stages[stageCounter].GetComponent<MeshRenderer>().material = LED[1]; //each correct stage changes green
                stageCounter++;
                if (stageCounter == 3){
                    Stages[stageCounter - 1].GetComponent<MeshRenderer>().material = LED[1]; //last led green cause it's out of the for loop above.
                    StartCoroutine(Animation());
                }
                else{
                    Debug.LogFormat("[Ill Morse #{0}] Stage {1}:", _moduleId, stageCounter + 1);
                    DisplayRandomWord(letters, letterToMorseMap);
                    MainDisplay.text = "";
                }
              
            }
            else {
                Module.HandleStrike();
                acceptingAnswer = false; //get out of submission mode
                stageCounter = 0;
                for (int i = 0; i < 3; i++){ //reset all leds back to black.
                    Stages[i].GetComponent<MeshRenderer>().material = LED[0];
                }
                DisplayRandomWord(letters, letterToMorseMap);
            }
    }
}

    private IEnumerator Animation () {
            WordDisplay.text = "";
            WordDisplay.text += "G";
            Audio.PlaySoundAtTransform("HardTypewriterKey", transform);
            yield return new WaitForSeconds(0.5f);
            WordDisplay.text += "G";
            Audio.PlaySoundAtTransform("HardTypewriterKey", transform);
            yield return new WaitForSeconds(0.5f);
            WordDisplay.text += "!";
            Audio.PlaySoundAtTransform("HardTypewriterKey", transform);
            yield return new WaitForSeconds(0.3f);
            Audio.PlaySoundAtTransform("DingSound", transform);
            Module.HandlePass();
        }
    public void DotButton(){
        GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
        Audio.PlaySoundAtTransform("KeySound", transform);
        if (!acceptingAnswer){
            return;
        }
        MainDisplay.text += ".";
    }

    public void DashButton()
    {
        GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
        Audio.PlaySoundAtTransform("KeySound", transform);
        if (!acceptingAnswer)
        {
            return;
        }
        MainDisplay.text += "-";
    }

    public void SpaceButton()
    {
        GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
        Audio.PlaySoundAtTransform("SpaceSound", transform);
        if (!acceptingAnswer)
        {
            return;
        }
        MainDisplay.text += " ";
    }

    public void NextButton()
    {
        GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);
        if (acceptingAnswer)
        {
            return;
        }
        DisplayRandomWord(letters, letterToMorseMap);
    }




    void GenerateLetters(){
        ShuffleArray(morseCombinations);
        // Assign Morse code combinations to each letter
        for (int i = 0; i < 26; i++)
        {
            letters[i] = (char)('A' + i);
            letterToMorseMap[i] = morseCombinations[i];
            Debug.LogFormat(letters[i] + " = " + letterToMorseMap[i]);
            Debug.LogFormat("[Ill Morse #{0}] {1} = {2}", _moduleId, letters[i], letterToMorseMap[i]);
        }
    }

    
    void DisplayRandomWord(char[] letters, string[] shuffledMorseCodes)
    {
        // Example word list
        string[] wordList = 
            {
                "ABBEY", "ABIDE", "ABORT", "ABOUT", "ABOVE", "ABUSE", "ABYSS", "ACIDS", "ACORN", "ACRES", "ACTED", "ACTOR", "ACUTE", "ADAPT", "ADDED", "ADDER", "ADIEU", "ADIOS", "ADMIN", "ADMIT", "ADOPT", "ADORE", "ADORN", "ADULT", "AFFIX", "AFTER", "AGAIN", "AGENT", "AGILE", "AGING", "AGONY", "AGREE", "AHEAD", "AIDED", "AIMED", "AIRED", "AISLE", "ALARM", "ALBUM", "ALERT", "ALGAE", "ALIAS", "ALIBI", "ALIEN", "ALIGN", "ALIKE", "ALIVE", "ALLEY", "ALLOW", "ALLOY", "ALOFT", "ALONE", "ALONG", "ALOOF", "ALOUD", "ALPHA", "ALTAR", "ALTER", "AMASS", "AMAZE", "AMBER", "AMBLE", "AMEND", "AMISH", "AMONG", "AMPLE", "AMUSE", "ANGEL", "ANGER", "ANGLE", "ANGRY", "ANGST", "ANIME", "ANION", "ANISE", "ANKLE", "ANNEX", "ANNOY", "ANNUL", "ANTIC", "ANVIL", "APART", "APPLE", "APPLY", "APRON", "AREAS", "ARENA", "ARGUE", "ARISE", "ARMED", "ARMOR", "AROMA", "AROSE", "ARRAY", "ARROW", "ARSON", "ASHEN", "ASHES", "ASIAN", "ASIDE", "ASKED", "ASSET", "ATOLL", "ATOMS", "ATONE", "ATTIC", "AUDIO", "AUDIT", "AUNTY", "AVAIL", "AVIAN", "AVOID", "AWAIT", "AWAKE", "AWARD", "AWARE", "AWASH", "AWFUL", "AWOKE", "AXIAL", "AXIOM", "AZTEC",
                "BACKS", "BACON", "BADGE", "BADLY", "BAKED", "BAKER", "BALLS", "BANDS", "BANKS", "BARGE", "BARON", "BASED", "BASES", "BASIC", "BASIL", "BASIN", "BASIS", "BATCH", "BATHS", "BEACH", "BEADS", "BEAMS", "BEANS", "BEARD", "BEARS", "BEAST", "BEERS", "BEGAN", "BEGIN", "BEGUN", "BEING", "BELLS", "BELLY", "BELOW", "BELTS", "BENCH", "BERRY", "BIBLE", "BIDET", "BIKES", "BILLS", "BINGE", "BINGO", "BIOME", "BIRCH", "BIRDS", "BIRTH", "BISON", "BITER", "BLACK", "BLADE", "BLAME", "BLAND", "BLANK", "BLARE", "BLAST", "BLAZE", "BLEAK", "BLEAT", "BLEED", "BLEND", "BLESS", "BLIMP", "BLIND", "BLINK", "BLISS", "BLITZ", "BLOCK", "BLOND", "BLOOD", "BLOOM", "BLOWN", "BLOWS", "BLUES", "BLUFF", "BLUNT", "BLUSH", "BOARD", "BOATS", "BOGUS", "BOLTS", "BOMBS", "BONDS", "BONES", "BONUS", "BOOKS", "BOOST", "BOOTH", "BOOTS", "BORED", "BORON", "BOTCH", "BOUND", "BOWED", "BOWEL", "BOWLS", "BOXED", "BOXER", "BOXES", "BRACE", "BRAID", "BRAIN", "BRAKE", "BRAND", "BRASH", "BRASS", "BRAVE", "BRAWL", "BRAWN", "BREAD", "BREAK", "BREED", "BRIBE", "BRICK", "BRIDE", "BRIEF", "BRINE", "BRING", "BRINK", "BRISK", "BROAD", "BROIL", "BROKE", "BROOK", "BROOM", "BROTH", "BROWN", "BRUSH", "BRUTE", "BUCKS", "BUDDY", "BUDGE", "BUGGY", "BUILD", "BUILT", "BULBS", "BULGE", "BULKY", "BULLS", "BUMPY", "BUNCH", "BUNNY", "BURNS", "BURNT", "BURST", "BUSES", "BUYER", "BUZZY", "BYLAW", "BYWAY",
                "CABBY", "CABIN", "CABLE", "CACHE", "CAKES", "CALLS", "CALVE", "CAMPS", "CAMPY", "CANAL", "CANDY", "CANED", "CANNY", "CANOE", "CANON", "CARDS", "CARED", "CARER", "CARES", "CARGO", "CAROL", "CARRY", "CARVE", "CASED", "CASES", "CASTE", "CATCH", "CATER", "CAULK", "CAUSE", "CAVES", "CEASE", "CEDED", "CELLS", "CENTS", "CHAFE", "CHAIN", "CHAIR", "CHALK", "CHAMP", "CHANT", "CHAOS", "CHAPS", "CHARM", "CHART", "CHARY", "CHASE", "CHASM", "CHEAP", "CHEAT", "CHECK", "CHEEK", "CHEER", "CHESS", "CHEST", "CHICK", "CHIEF", "CHILD", "CHILI", "CHILL", "CHIME", "CHINA", "CHIPS", "CHOIR", "CHORD", "CHORE", "CHOSE", "CHUCK", "CHUNK", "CHUTE", "CIDER", "CIGAR", "CINCH", "CITED", "CITES", "CIVIC", "CIVIL", "CLAIM", "CLANK", "CLASH", "CLASS", "CLAWS", "CLEAN", "CLEAR", "CLEAT", "CLERK", "CLICK", "CLIFF", "CLIMB", "CLING", "CLOAK", "CLOCK", "CLONE", "CLOSE", "CLOTH", "CLOUD", "CLOUT", "CLOVE", "CLOWN", "CLUBS", "CLUCK", "CLUES", "CLUNG", "CLUNK", "COACH", "COAST", "COATS", "COCOA", "CODES", "COINS", "COLON", "COLOR", "COMES", "COMIC", "COMMA", "CONCH", "CONIC", "CORAL", "CORGI", "CORNY", "CORPS", "COSTS", "COUCH", "COUGH", "COULD", "COUNT", "COURT", "COVEN", "COVER", "CRACK", "CRAFT", "CRANE", "CRANK", "CRASH", "CRASS", "CRATE", "CRAVE", "CRAWL", "CRAZY", "CREAK", "CREAM", "CREED", "CREEK", "CREPT", "CREST", "CREWS", "CRIED", "CRIES", "CRIME", "CRISP", "CROPS", "CROSS", "CROWD", "CROWN", "CRUDE", "CRUEL", "CRUSH", "CRUST", "CRYPT", "CUBAN", "CUBBY", "CUBIC", "CUMIN", "CURLS", "CURLY", "CURRY", "CURSE", "CURVE", "CUTIE", "CYCLE", "CYNIC", "CZECH",
                "DADDY", "DAILY", "DAIRY", "DAISY", "DANCE", "DARED", "DATED", "DATES", "DATUM", "DEALS", "DEALT", "DEATH", "DEBIT", "DEBTS", "DEBUG", "DEBUT", "DECAF", "DECAL", "DECAY", "DECOR", "DECOY", "DEEDS", "DEITY", "DELAY", "DELVE", "DENIM", "DENSE", "DEPOT", "DEPTH", "DERBY", "DESKS", "DETER", "DETOX", "DEUCE", "DEVIL", "DIARY", "DICED", "DIETS", "DIGIT", "DIMLY", "DINAR", "DINER", "DINGY", "DIRTY", "DISCO", "DISCS", "DISKS", "DITCH", "DIVED", "DIVER", "DIZZY", "DOCKS", "DODGE", "DODGY", "DOGGY", "DOGMA", "DOING", "DOLLS", "DONOR", "DONUT", "DOORS", "DOSED", "DOSES", "DOUBT", "DOUGH", "DOUSE", "DOZEN", "DRAFT", "DRAIN", "DRAMA", "DRANK", "DRAWN", "DRAWS", "DREAD", "DREAM", "DRESS", "DRIED", "DRIER", "DRIFT", "DRILL", "DRINK", "DRIVE", "DRONE", "DROPS", "DROVE", "DROWN", "DRUGS", "DRUMS", "DRUNK", "DRYER", "DUCKS", "DUMMY", "DUNCE", "DUNES", "DUSTY", "DUTCH", "DWARF", "DWELL", "DYING",
                "EAGER", "EAGLE", "EARLY", "EARTH", "EASED", "EASEL", "EATEN", "EDGES", "EDICT", "EERIE", "EIGHT", "ELBOW", "ELDER", "ELECT", "ELITE", "ELUDE", "ELVES", "EMOTE", "EMPTY", "ENACT", "ENDED", "ENEMY", "ENJOY", "ENSUE", "ENTER", "ENTRY", "EQUAL", "EQUIP", "ERASE", "ERECT", "ERROR", "ESSAY", "ETHIC", "ETUDE", "EVADE", "EVENT", "EVERY", "EVICT", "EXACT", "EXALT", "EXAMS", "EXERT", "EXILE", "EXIST", "EXTRA",
                "FACED", "FACES", "FACTS", "FADED", "FAILS", "FAINT", "FAIRS", "FAIRY", "FAITH", "FALLS", "FALSE", "FAMED", "FANCY", "FARES", "FARMS", "FATAL", "FATED", "FATTY", "FAULT", "FAUNA", "FAVOR", "FEARS", "FEAST", "FEELS", "FEINT", "FELLA", "FENCE", "FERRY", "FETAL", "FETCH", "FEVER", "FEWER", "FIBER", "FIBRE", "FIELD", "FIERY", "FIFTH", "FIFTY", "FIGHT", "FILED", "FILES", "FILET", "FILLS", "FILLY", "FILMS", "FILMY", "FILTH", "FINAL", "FINDS", "FINED", "FINER", "FINES", "FIRED", "FIRES", "FIRMS", "FIRST", "FISTS", "FIXED", "FLAGS", "FLAIL", "FLAIR", "FLAME", "FLANK", "FLARE", "FLASH", "FLASK", "FLATS", "FLAWS", "FLEET", "FLESH", "FLIES", "FLING", "FLIRT", "FLOAT", "FLOCK", "FLOOD", "FLOOR", "FLORA", "FLOUR", "FLOWN", "FLOWS", "FLUID", "FLUNG", "FLUNK", "FLUSH", "FLUTE", "FOCAL", "FOCUS", "FOGGY", "FOLDS", "FOLIO", "FOLKS", "FOLLY", "FONTS", "FOODS", "FOOLS", "FORCE", "FORGE", "FORMS", "FORTE", "FORTH", "FORTY", "FORUM", "FOUND", "FOURS", "FOXES", "FOYER", "FRAIL", "FRAME", "FRANK", "FRAUD", "FREAK", "FREED", "FRESH", "FRIED", "FRISK", "FROGS", "FRONT", "FROST", "FROWN", "FROZE", "FRUIT", "FUDGE", "FUELS", "FULLY", "FUMES", "FUNDS", "FUNNY", "FUSED", "FUTON", "FUZZY",
                "GAINS", "GAMES", "GANGS", "GASES", "GATES", "GAUGE", "GAZED", "GEESE", "GENES", "GENIE", "GENRE", "GENUS", "GHOST", "GHOUL", "GIANT", "GIDDY", "GIFTS", "GIMPY", "GIRLS", "GIRLY", "GIRTH", "GIVEN", "GIVES", "GIZMO", "GLAND", "GLARE", "GLASS", "GLEAM", "GLIDE", "GLINT", "GLOBE", "GLOOM", "GLORY", "GLOSS", "GLOVE", "GLUED", "GOALS", "GOATS", "GOING", "GOLLY", "GOODS", "GOOFY", "GOOPY", "GOOSE", "GORGE", "GRACE", "GRADE", "GRAFT", "GRAIN", "GRAMS", "GRAND", "GRANT", "GRAPE", "GRAPH", "GRASP", "GRASS", "GRATE", "GRAVE", "GRAVY", "GREAT", "GREED", "GREEK", "GREEN", "GREET", "GRIEF", "GRILL", "GRIME", "GRIMY", "GRIND", "GRIPS", "GROIN", "GROOM", "GROSS", "GROUP", "GROUT", "GROWN", "GROWS", "GRUEL", "GRUMP", "GRUNT", "GUARD", "GUAVA", "GUESS", "GUEST", "GUIDE", "GUILD", "GUILT", "GULLS", "GUMMY", "GUNKY", "GUSTY", "GUTSY",
                "HABIT", "HAIKU", "HAIRS", "HAIRY", "HALAL", "HALLS", "HALVE", "HANDS", "HANDY", "HANGS", "HAPPY", "HARDY", "HAREM", "HARPY", "HARSH", "HASTE", "HASTY", "HATCH", "HATED", "HATES", "HAUNT", "HAVEN", "HAVOC", "HAZEL", "HEADS", "HEARD", "HEARS", "HEART", "HEAVE", "HEAVY", "HEDGE", "HEELS", "HEFTY", "HEIRS", "HEIST", "HELIX", "HELLO", "HELPS", "HENCE", "HERBS", "HERDS", "HILLS", "HILLY", "HINDU", "HINGE", "HINTS", "HIPPO", "HIRED", "HITCH", "HOBBY", "HOIST", "HOLDS", "HOLES", "HOLLY", "HOMED", "HOMES", "HONEY", "HONOR", "HOOKS", "HOPED", "HOPES", "HORNS", "HORSE", "HOSTS", "HOTEL", "HOUND", "HOURS", "HOUSE", "HUMAN", "HUMID", "HUMOR", "HUMUS", "HURRY", "HURTS", "HUSKY", "HYENA", "HYMNS",
                "ICHOR", "ICING", "ICONS", "IDEAL", "IDEAS", "IDIOM", "IDIOT", "IDLED", "IDYLL", "IGLOO", "IMAGE", "IMBUE", "IMPLY", "INANE", "INDEX", "INDIA", "INERT", "INFER", "INGOT", "INLAY", "INLET", "INNER", "INPUT", "INTRO", "IRISH", "IRONY", "ISSUE", "ITCHY", "ITEMS", "IVORY",
                "JAPAN", "JEANS", "JELLY", "JEWEL", "JOINS", "JOINT", "JOKER", "JOKES", "JOLLY", "JOULE", "JOUST", "JUDGE", "JUICE", "JUICY", "JUMBO", "JUMPS",
                "KABOB", "KANJI", "KARAT", "KARMA", "KAYAK", "KAZOO", "KEEPS", "KICKS", "KILLS", "KINDA", "KINDS", "KINGS", "KITTY", "KNAVE", "KNEAD", "KNEEL", "KNEES", "KNELT", "KNIFE", "KNOBS", "KNOCK", "KNOLL", "KNOTS", "KNOWN", "KNOWS", "KOALA", "KUDOS",
                "LABEL", "LABOR", "LACED", "LACKS", "LADLE", "LAGER", "LAKES", "LAMBS", "LAMPS", "LANDS", "LANES", "LAPSE", "LARGE", "LARVA", "LASER", "LASSO", "LASTS", "LATCH", "LATER", "LATIN", "LATTE", "LAUGH", "LAWNS", "LAYER", "LEADS", "LEAFY", "LEAKY", "LEAPT", "LEARN", "LEASE", "LEASH", "LEAST", "LEAVE", "LEDGE", "LEECH", "LEGAL", "LEMON", "LEMUR", "LEVEL", "LEVER", "LIBEL", "LIEGE", "LIFTS", "LIGHT", "LIKED", "LIKES", "LILAC", "LIMBO", "LIMBS", "LIMIT", "LINED", "LINEN", "LINES", "LINKS", "LIONS", "LIPID", "LISTS", "LITER", "LITRE", "LIVED", "LIVEN", "LIVER", "LIVES", "LIVID", "LLAMA", "LOADS", "LOANS", "LOBBY", "LOCAL", "LOCKS", "LODGE", "LOGIC", "LOGIN", "LOLLY", "LONER", "LOOKS", "LOONY", "LOOPS", "LOOPY", "LOOSE", "LORDS", "LOSER", "LOSES", "LOTTO", "LOTUS", "LOUSE", "LOUSY", "LOVED", "LOVER", "LOVES", "LOWER", "LOYAL", "LUCID", "LUCKY", "LUMEN", "LUMPS", "LUMPY", "LUNAR", "LUNCH", "LUNGE", "LUNGS", "LUSTY", "LYING", "LYNCH", "LYRIC",
                "MACHO", "MADAM", "MADLY", "MAGIC", "MAGMA", "MAINS", "MAIZE", "MAJOR", "MAKER", "MAKES", "MALES", "MAMBO", "MANGO", "MANIA", "MANIC", "MANLY", "MANOR", "MAPLE", "MARCH", "MARKS", "MARRY", "MARSH", "MASKS", "MATCH", "MATED", "MATES", "MATHS", "MATTE", "MAXIM", "MAYAN", "MAYBE", "MAYOR", "MEALS", "MEANS", "MEANT", "MEATY", "MEDAL", "MEDIA", "MEDIC", "MEETS", "MELON", "MENUS", "MERCY", "MERGE", "MERIT", "MERRY", "MESSY", "METAL", "METER", "METRE", "MICRO", "MIDST", "MIGHT", "MILES", "MILLS", "MIMIC", "MINCE", "MINDS", "MINED", "MINER", "MINES", "MINOR", "MINTY", "MINUS", "MIRTH", "MISTY", "MITRE", "MIXED", "MIXER", "MODEL", "MODEM", "MODES", "MOGUL", "MOIST", "MOLAR", "MOLDY", "MOLES", "MONEY", "MONKS", "MONTH", "MOODS", "MOORS", "MOOSE", "MORAL", "MORAY", "MORPH", "MOTEL", "MOTIF", "MOTOR", "MOTTO", "MOUND", "MOUNT", "MOUSE", "MOUTH", "MOVED", "MOVER", "MOVES", "MOVIE", "MUCUS", "MUDDY", "MUMMY", "MUNCH", "MURKY", "MUSED", "MUSIC", "MUSTY", "MUTED", "MYTHS",
                "NACHO", "NADIR", "NAILS", "NAIVE", "NAKED", "NAMED", "NAMES", "NANNY", "NASAL", "NASTY", "NATAL", "NAVAL", "NAVEL", "NECKS", "NEEDS", "NEEDY", "NEIGH", "NERVE", "NESTS", "NEVER", "NEWER", "NEWLY", "NEXUS", "NICER", "NICHE", "NIECE", "NIFTY", "NIGHT", "NINJA", "NINTH", "NITRO", "NOBLE", "NOBLY", "NODES", "NOISE", "NOISY", "NOMAD", "NOOSE", "NORMS", "NORTH", "NOSES", "NOTCH", "NOTED", "NOTES", "NOVEL", "NUDGE", "NURSE", "NUTTY", "NYLON", "NYMPH",
                "OASIS", "OCCUR", "OCEAN", "ODDLY", "ODOUR", "OFFER", "OFTEN", "OILED", "OLDER", "OLDIE", "OLIVE", "ONION", "ONSET", "OOMPH", "OPENS", "OPERA", "OPTIC", "ORBIT", "ORDER", "ORGAN", "OTHER", "OTTER", "OUGHT", "OUNCE", "OUTDO", "OUTER", "OVERT", "OVOID", "OWNED", "OWNER", "OXIDE", "OZONE",
                "PACKS", "PAGES", "PAINS", "PAINT", "PAIRS", "PALMS", "PANDA", "PANEL", "PANIC", "PANTS", "PAPER", "PARKS", "PARTS", "PARTY", "PASTA", "PASTE", "PATCH", "PATHS", "PATIO", "PAUSE", "PEACE", "PEACH", "PEAKS", "PEARL", "PEARS", "PEDAL", "PEERS", "PENAL", "PENCE", "PENNY", "PERIL", "PESTS", "PETTY", "PHASE", "PHONE", "PHOTO", "PIANO", "PICKS", "PIECE", "PIERS", "PIGGY", "PILAF", "PILED", "PILES", "PILLS", "PILOT", "PINCH", "PINTS", "PIOUS", "PIPES", "PITCH", "PIVOT", "PIXEL", "PIXIE", "PIZZA", "PLACE", "PLAIN", "PLANE", "PLANK", "PLANS", "PLANT", "PLATE", "PLAYS", "PLAZA", "PLEAD", "PLEAS", "PLEAT", "PLOTS", "PLUME", "PLUMP", "POEMS", "POETS", "POINT", "POKER", "POLAR", "POLES", "POLIO", "POLLS", "POLYP", "PONDS", "POOLS", "PORCH", "PORES", "PORTS", "POSED", "POSES", "POSIT", "POSTS", "POUCH", "POUND", "POWER", "PREEN", "PRESS", "PRICE", "PRICY", "PRIDE", "PRIME", "PRINT", "PRIOR", "PRISM", "PRIVY", "PRIZE", "PROBE", "PROMO", "PRONE", "PRONG", "PROOF", "PROSE", "PROUD", "PROVE", "PROXY", "PRUDE", "PRUNE", "PUDGY", "PULLS", "PULSE", "PUMPS", "PUNCH", "PUPIL", "PUPPY", "PURSE", "PYLON",
                "QUACK", "QUAIL", "QUALM", "QUARK", "QUART", "QUEEN", "QUELL", "QUERY", "QUEST", "QUEUE", "QUICK", "QUIET", "QUILL", "QUILT", "QUIRK", "QUITE", "QUOTA", "QUOTE",
                "RABBI", "RACED", "RACES", "RADAR", "RADIO", "RAIDS", "RAILS", "RAINY", "RAISE", "RALLY", "RAMPS", "RANCH", "RANGE", "RANKS", "RAPID", "RATED", "RATES", "RATIO", "RAVEN", "RAZOR", "REACH", "REACT", "READS", "READY", "REALM", "REBEL", "RECAP", "RECON", "REDLY", "REFER", "REHAB", "REIGN", "REINS", "RELAX", "RELAY", "RELIC", "REMIT", "REMIX", "RENEW", "RENTS", "REPAY", "REPLY", "RESIN", "RESTS", "RETRO", "REUSE", "RHINO", "RHYME", "RIDER", "RIDGE", "RIFLE", "RIGHT", "RIGID", "RIGOR", "RILED", "RINGS", "RINSE", "RIOTS", "RISEN", "RISES", "RISKS", "RISKY", "RITES", "RITZY", "RIVAL", "RIVER", "RIVET", "ROADS", "ROAST", "ROBES", "ROBOT", "ROCKS", "ROCKY", "ROGUE", "ROLES", "ROLLS", "ROMAN", "ROOFS", "ROOMS", "ROOMY", "ROOTS", "ROPES", "ROSES", "ROSIN", "ROTOR", "ROUGE", "ROUGH", "ROUND", "ROUTE", "ROVER", "ROYAL", "RUGBY", "RUINS", "RULED", "RULER", "RULES", "RUMBA", "RUMMY", "RUMOR", "RUNIC", "RUNNY", "RURAL", "RUSTY",
                "SABLE", "SADLY", "SAFER", "SAGGY", "SAILS", "SAINT", "SALAD", "SALES", "SALON", "SALSA", "SALTS", "SALTY", "SALVE", "SAMBA", "SANDS", "SANDY", "SATED", "SATIN", "SATYR", "SAUCE", "SAUCY", "SAUNA", "SAVED", "SAVES", "SAVOR", "SAVVY", "SCALD", "SCALE", "SCALP", "SCALY", "SCAMP", "SCARE", "SCARF", "SCARS", "SCARY", "SCENE", "SCENT", "SCOFF", "SCOLD", "SCONE", "SCOOP", "SCOOT", "SCOPE", "SCORE", "SCORN", "SCOUR", "SCOUT", "SCRAM", "SCRAP", "SCREW", "SCRIM", "SCRUB", "SCRUM", "SCUBA", "SEALS", "SEAMS", "SEATS", "SEEDS", "SEEDY", "SEEKS", "SEEMS", "SEGUE", "SEIZE", "SELLS", "SENDS", "SENSE", "SERUM", "SERVE", "SETUP", "SEVEN", "SHADE", "SHADY", "SHAFT", "SHAKE", "SHAKY", "SHALL", "SHAME", "SHANK", "SHAPE", "SHARD", "SHARE", "SHARP", "SHAVE", "SHAWL", "SHEAR", "SHEEP", "SHEER", "SHEET", "SHELF", "SHELL", "SHIFT", "SHILL", "SHINE", "SHINY", "SHIPS", "SHIRE", "SHIRT", "SHOCK", "SHOES", "SHONE", "SHOOK", "SHOOT", "SHOPS", "SHORE", "SHORT", "SHOTS", "SHOUT", "SHOVE", "SHOWN", "SHOWS", "SHRUG", "SHUSH", "SIDES", "SIDLE", "SIEGE", "SIGHT", "SIGIL", "SIGNS", "SILLY", "SINCE", "SINEW", "SINGE", "SINGS", "SINUS", "SITAR", "SITES", "SIXTH", "SIXTY", "SIZED", "SIZES", "SKATE", "SKIER", "SKIES", "SKILL", "SKIMP", "SKINS", "SKIRT", "SKULL", "SLABS", "SLAIN", "SLANG", "SLANT", "SLASH", "SLATE", "SLAVE", "SLEEK", "SLEEP", "SLEET", "SLEPT", "SLICE", "SLIDE", "SLIME", "SLIMY", "SLING", "SLINK", "SLOPE", "SLOSH", "SLOTH", "SLOTS", "SLUMP", "SLUSH", "SLYLY", "SMALL", "SMART", "SMASH", "SMEAR", "SMELL", "SMELT", "SMILE", "SMITE", "SMOKE", "SNAIL", "SNAKE", "SNARE", "SNARL", "SNEER", "SNIFF", "SNIPE", "SNOOP", "SNORE", "SNORT", "SNOUT", "SOBER", "SOCKS", "SOFTY", "SOGGY", "SOILS", "SOLAR", "SOLID", "SOLVE", "SONAR", "SONGS", "SONIC", "SOOTH", "SORRY", "SORTS", "SOULS", "SOUND", "SOUTH", "SPACE", "SPADE", "SPAIN", "SPARE", "SPARK", "SPAWN", "SPEAK", "SPEED", "SPELL", "SPEND", "SPENT", "SPIES", "SPINE", "SPLAT", "SPLIT", "SPOIL", "SPOKE", "SPOON", "SPORT", "SPOTS", "SPRAY", "SPURS", "SQUAD", "STACK", "STAFF", "STAGE", "STAIN", "STAIR", "STAKE", "STALE", "STALL", "STAMP", "STAND", "STARE", "STARK", "STARS", "START", "STASH", "STATE", "STAYS", "STEAD", "STEAK", "STEAL", "STEAM", "STEEL", "STEEP", "STEER", "STEMS", "STENO", "STEPS", "STERN", "STICK", "STIFF", "STILL", "STILT", "STING", "STINK", "STINT", "STOCK", "STOIC", "STOKE", "STOLE", "STOMP", "STONE", "STONY", "STOOD", "STOOL", "STOOP", "STOPS", "STORE", "STORK", "STORM", "STORY", "STOUT", "STOVE", "STRAP", "STRAW", "STRAY", "STRIP", "STRUM", "STRUT", "STUCK", "STUDY", "STUFF", "STUMP", "STUNT", "STYLE", "SUAVE", "SUEDE", "SUGAR", "SUITE", "SUITS", "SUNNY", "SUPER", "SURGE", "SUSHI", "SWAMP", "SWANS", "SWARM", "SWATH", "SWEAR", "SWEAT", "SWEEP", "SWEET", "SWELL", "SWEPT", "SWIFT", "SWILL", "SWINE", "SWING", "SWIPE", "SWIRL", "SWISS", "SWOON", "SWOOP", "SWORD", "SWORE", "SWORN", "SWUNG",
                "TABLE", "TACIT", "TAFFY", "TAILS", "TAINT", "TAKEN", "TAKES", "TALES", "TALKS", "TALLY", "TALON", "TAMED", "TANGO", "TANGY", "TANKS", "TAPES", "TARDY", "TAROT", "TASKS", "TASTE", "TASTY", "TAUNT", "TAXED", "TAXES", "TAXIS", "TEACH", "TEAMS", "TEARS", "TEARY", "TEASE", "TECHY", "TEDDY", "TEENS", "TEENY", "TEETH", "TELLS", "TELLY", "TEMPO", "TENDS", "TENOR", "TENSE", "TENTH", "TENTS", "TERMS", "TESTS", "TEXAS", "TEXTS", "THANK", "THEFT", "THEIR", "THEME", "THERE", "THESE", "THETA", "THICK", "THIEF", "THIGH", "THING", "THINK", "THIRD", "THONG", "THORN", "THOSE", "THREE", "THREW", "THROW", "THUMB", "TIARA", "TIBIA", "TIDAL", "TIDES", "TIGER", "TIGHT", "TILDE", "TILED", "TILES", "TIMED", "TIMER", "TIMES", "TIMID", "TINNY", "TIPSY", "TIRED", "TITLE", "TOAST", "TODAY", "TOKEN", "TONAL", "TONED", "TONES", "TONGS", "TONIC", "TOOLS", "TOONS", "TOOTH", "TOPAZ", "TOPIC", "TORCH", "TORSO", "TORTE", "TORUS", "TOTAL", "TOTEM", "TOUCH", "TOUGH", "TOURS", "TOWEL", "TOWER", "TOWNS", "TOXIC", "TOXIN", "TRACE", "TRACK", "TRACT", "TRADE", "TRAIL", "TRAIN", "TRAIT", "TRAMP", "TRAMS", "TRASH", "TRAYS", "TREAD", "TREAT", "TREES", "TREND", "TRIAD", "TRIAL", "TRIBE", "TRICK", "TRIED", "TRIES", "TRIKE", "TRILL", "TRIPS", "TRITE", "TROLL", "TROOP", "TROUT", "TRUCE", "TRUCK", "TRULY", "TRUNK", "TRUST", "TRUTH", "TUBES", "TULIP", "TUMMY", "TUNED", "TUNES", "TUNIC", "TURKS", "TURNS", "TUTOR", "TWANG", "TWEAK", "TWICE", "TWINS", "TWIRL", "TWIST", "TYING", "TYPES", "TYRES",
                "UDDER", "ULCER", "ULTRA", "UNBAN", "UNCAP", "UNCLE", "UNCUT", "UNDER", "UNDUE", "UNFED", "UNFIT", "UNIFY", "UNION", "UNITE", "UNITS", "UNITY", "UNLIT", "UNMET", "UNTIE", "UNTIL", "UNZIP", "UPPER", "UPSET", "URBAN", "URGED", "URINE", "USAGE", "USERS", "USHER", "USING", "USUAL", "UTTER", "UVULA",
                "VAGUE", "VALET", "VALID", "VALOR", "VALUE", "VALVE", "VAPOR", "VAULT", "VEINS", "VEINY", "VENOM", "VENUE", "VERBS", "VERGE", "VERSE", "VICAR", "VIDEO", "VIEWS", "VIGIL", "VIGOR", "VILLA", "VINES", "VINYL", "VIRAL", "VIRUS", "VISIT", "VISOR", "VITAL", "VIVID", "VIXEN", "VOCAL", "VODKA", "VOGUE", "VOICE", "VOTED", "VOTER", "VOTES", "VOUCH", "VOWED", "VOWEL", "VROOM",
                "WAGES", "WAGON", "WAIST", "WAITS", "WAIVE", "WALKS", "WALLS", "WALTZ", "WANTS", "WARDS", "WARES", "WARNS", "WASTE", "WATCH", "WATER", "WAVED", "WAVES", "WEARS", "WEARY", "WEAVE", "WEDGE", "WEEDS", "WEEKS", "WEIGH", "WEIRD", "WELLS", "WELSH", "WHALE", "WHEAT", "WHEEL", "WHERE", "WHICH", "WHILE", "WHINE", "WHISK", "WHITE", "WHOLE", "WHOSE", "WIDEN", "WIDER", "WIDOW", "WIDTH", "WIELD", "WILLS", "WIMPY", "WINCE", "WINCH", "WINDS", "WINDY", "WINES", "WINGS", "WIPED", "WIRED", "WIRES", "WISER", "WITCH", "WITTY", "WIVES", "WOKEN", "WOMAN", "WOMEN", "WOODS", "WORDS", "WORKS", "WORLD", "WORMS", "WORMY", "WORRY", "WORSE", "WORST", "WORTH", "WOULD", "WOUND", "WOVEN", "WRATH", "WRECK", "WRIST", "WRITE", "WRONG", "WROTE", "WRYLY",
                "XENON", "XEROX",
                "YACHT", "YARDS", "YAWNS", "YEARN", "YEARS", "YEAST", "YELLS", "YIELD", "YODEL", "YOUNG", "YOURS", "YOUTH", "YUMMY",
                "ZEBRA", "ZILCH", "ZINGY", "ZONES"
            };

        // Get a random word from the list
        string randomWord = wordList[Rnd.Range(0, wordList.Length)];

        // Display the word
        Debug.LogFormat("[Ill Morse #{0}] Word: {1} ", _moduleId, randomWord);
        WordDisplay.text = randomWord;
        string shuffledWord = ShuffleWord(randomWord);
        if (!acceptingAnswer){
            Debug.LogFormat("[Ill Morse #{0}] Scrambled Word: {1}", _moduleId, shuffledWord);
        }
        

        // Display the corresponding shuffled Morse code sequence
        string normalMorseSequence = GetMorseSequence(randomWord, letters, shuffledMorseCodes);
        string randomMorseSequence = GetMorseSequence(shuffledWord, letters, shuffledMorseCodes);
        morseAnswer = normalMorseSequence;
        if (!acceptingAnswer){
           Debug.LogFormat("[Ill Morse #{0}] Scrambled Morse Code: {1}", _moduleId, randomMorseSequence); 
        }
        
        Debug.LogFormat("[Ill Morse #{0}] Unscrambled Morse Code: {1}", _moduleId, normalMorseSequence);
        MainDisplay.text = randomMorseSequence;
    }

    string GetMorseSequence(string word, char[] letters, string[] shuffledMorseCodes)
    {
        // Convert each letter in the word to its corresponding shuffled Morse code
        string morseSequence = "";
        foreach (char letter in word)
        {
            int index = System.Array.IndexOf(letters, letter);
            if (index != -1)
            {
                morseSequence += shuffledMorseCodes[index] + " ";
            }
            else
            {
                morseSequence += " ";
            }
        }

        return morseSequence.Trim(); // Trim to remove trailing space
    }
    static void ShuffleArray<T>(T[] array)
    {
        //Shuffles the morse array
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    static string ShuffleWord(string word)
    {
        //Shuffles the word array
        char[] charArray = word.ToCharArray();
        for (int i = charArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            char temp = charArray[i];
            charArray[i] = charArray[randomIndex];
            charArray[randomIndex] = temp;
        }
        return new string(charArray);
    }
}
