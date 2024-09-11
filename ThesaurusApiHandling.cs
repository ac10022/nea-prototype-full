﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace api_handling_for_nea
{
    public class ThesaurusApiHandling
    {
        // require newtonsoft library
        public ThesaurusApiHandling(string wordInput = null)
        {
            WordInput = wordInput;
        }

        private const string THESAURUS_API_KEY = "a4617fe8-3dff-437b-9acf-81fefac55f40";

        private HttpClient ApiHttpClient { get; set; }
        public string WordInput { private get; set; }

        #region wordLists

        private string[] verbs = { "abide", "accelerate", "accept", "accomplish", "achieve", "acquire", "acted", "activate", "adapt", "add", "address", "administer", "admire", "admit", "adopt", "advise", "afford", "agree", "alert", "alight", "allow", "altered", "amuse", "analyze", "announce", "annoy", "answer", "anticipate", "apologize", "appear", "applaud", "applied", "appoint", "appraise", "appreciate", "approve", "arbitrate", "argue", "arise", "arrange", "arrest", "arrive", "ascertain", "ask", "assemble", "assess", "assist", "assure", "attach", "attack", "attain", "attempt", "attend", "attract", "audited", "avoid", "awake", "back", "bake", "balance", "ban", "bang", "bare", "bat", "bathe", "battle", "be", "beam", "bear", "beat", "become", "beg", "begin", "behave", "behold", "belong", "bend", "beset", "bet", "bid", "bind", "bite", "bleach", "bleed", "bless", "blind", "blink", "blot", "blow", "blush", "boast", "boil", "bolt", "bomb", "book", "bore", "borrow", "bounce", "bow", "box", "brake", "branch", "break", "breathe", "breed", "brief", "bring", "broadcast", "bruise", "brush", "bubble", "budget", "build", "bump", "burn", "burst", "bury", "bust", "buy", "buze", "calculate", "call", "camp", "care", "carry", "carve", "cast", "catalog", "catch", "cause", "challenge", "change", "charge", "chart", "chase", "cheat", "check", "cheer", "chew", "choke", "choose", "chop", "claim", "clap", "clarify", "classify", "clean", "clear", "cling", "clip", "close", "clothe", "coach", "coil", "collect", "color", "comb", "come", "command", "communicate", "compare", "compete", "compile", "complain", "complete", "compose", "compute", "conceive", "concentrate", "conceptualize", "concern", "conclude", "conduct", "confess", "confront", "confuse", "connect", "conserve", "consider", "consist", "consolidate", "construct", "consult", "contain", "continue", "contract", "control", "convert", "coordinate", "copy", "correct", "correlate", "cost", "cough", "counsel", "count", "cover", "crack", "crash", "crawl", "create", "creep", "critique", "cross", "crush", "cry", "cure", "curl", "curve", "cut", "cycle", "dam", "damage", "dance", "dare", "deal", "decay", "deceive", "decide", "decorate", "define", "delay", "delegate", "delight", "deliver", "demonstrate", "depend", "describe", "desert", "deserve", "design", "destroy", "detail", "detect", "determine", "develop", "devise", "diagnose", "dig", "direct", "disagree", "disappear", "disapprove", "disarm", "discover", "dislike", "dispense", "display", "disprove", "dissect", "distribute", "dive", "divert", "divide", "do", "double", "doubt", "draft", "drag", "drain", "dramatize", "draw", "dream", "dress", "drink", "drip", "drive", "drop", "drown", "drum", "dry", "dust", "dwell", "earn", "eat", "edited", "educate", "eliminate", "embarrass", "employ", "empty", "enacted", "encourage", "end", "endure", "enforce", "engineer", "enhance", "enjoy", "enlist", "ensure", "enter", "entertain", "escape", "establish", "estimate", "evaluate", "examine", "exceed", "excite", "excuse", "execute", "exercise", "exhibit", "exist", "expand", "expect", "expedite", "experiment", "explain", "explode", "express", "extend", "extract", "face", "facilitate", "fade", "fail", "fancy", "fasten", "fax", "fear", "feed", "feel", "fence", "fetch", "fight", "file", "fill", "film", "finalize", "finance", "find", "fire", "fit", "fix", "flap", "flash", "flee", "fling", "float", "flood", "flow", "flower", "fly", "fold", "follow", "fool", "forbid", "force", "forecast", "forego", "foresee", "foretell", "forget", "forgive", "form", "formulate", "forsake", "frame", "freeze", "frighten", "fry", "gather", "gaze", "generate", "get", "give", "glow", "glue", "go", "govern", "grab", "graduate", "grate", "grease", "greet", "grin", "grind", "grip", "groan", "grow", "guarantee", "guard", "guess", "guide", "hammer", "hand", "handle", "handwrite", "hang", "happen", "harass", "harm", "hate", "haunt", "head", "heal", "heap", "hear", "heat", "help", "hide", "hit", "hold", "hook", "hop", "hope", "hover", "hug", "hum", "hunt", "hurry", "hurt", "hypothesize", "identify", "ignore", "illustrate", "imagine", "implement", "impress", "improve", "improvise", "include", "increase", "induce", "influence", "inform", "initiate", "inject", "injure", "inlay", "innovate", "input", "inspect", "inspire", "install", "institute", "instruct", "insure", "integrate", "intend", "intensify", "interest", "interfere", "interlay", "interpret", "interrupt", "interview", "introduce", "invent", "inventory", "investigate", "invite", "irritate", "itch", "jail", "jam", "jog", "join", "joke", "judge", "juggle", "jump", "justify", "keep", "kept", "kick", "kill", "kiss", "kneel", "knit", "knock", "knot", "know", "label", "land", "last", "laugh", "launch", "lay", "lead", "lean", "leap", "learn", "leave", "lecture", "led", "lend", "let", "level", "license", "lick", "lie", "lifted", "light", "lighten", "like", "list", "listen", "live", "load", "locate", "lock", "log", "long", "look", "lose", "love", "maintain", "make", "man", "manage", "manipulate", "manufacture", "map", "march", "mark", "market", "marry", "match", "mate", "matter", "mean", "measure", "meddle", "mediate", "meet", "melt", "melt", "memorize", "mend", "mentor", "milk", "mine", "mislead", "miss", "misspell", "mistake", "misunderstand", "mix", "moan", "model", "modify", "monitor", "moor", "motivate", "mourn", "move", "mow", "muddle", "mug", "multiply", "murder", "nail", "name", "navigate", "need", "negotiate", "nest", "nod", "nominate", "normalize", "note", "notice", "number", "obey", "object", "observe", "obtain", "occur", "offend", "offer", "officiate", "open", "operate", "order", "organize", "oriented", "originate", "overcome", "overdo", "overdraw", "overflow", "overhear", "overtake", "overthrow", "owe", "own", "pack", "paddle", "paint", "park", "part", "participate", "pass", "paste", "pat", "pause", "pay", "peck", "pedal", "peel", "peep", "perceive", "perfect", "perform", "permit", "persuade", "phone", "photograph", "pick", "pilot", "pinch", "pine", "pinpoint", "pioneer", "place", "plan", "plant", "play", "plead", "please", "plug", "point", "poke", "polish", "pop", "possess", "post", "pour", "practice", "praised", "pray", "preach", "precede", "predict", "prefer", "prepare", "prescribe", "present", "preserve", "preset", "preside", "press", "pretend", "prevent", "prick", "print", "process", "procure", "produce", "profess", "program", "progress", "project", "promise", "promote", "proofread", "propose", "protect", "prove", "provide", "publicize", "pull", "pump", "punch", "puncture", "punish", "purchase", "push", "put", "qualify", "question", "queue", "quit", "race", "radiate", "rain", "raise", "rank", "rate", "reach", "read", "realign", "realize", "reason", "receive", "recognize", "recommend", "reconcile", "record", "recruit", "reduce", "refer", "reflect", "refuse", "regret", "regulate", "rehabilitate", "reign", "reinforce", "reject", "rejoice", "relate", "relax", "release", "rely", "remain", "remember", "remind", "remove", "render", "reorganize", "repair", "repeat", "replace", "reply", "report", "represent", "reproduce", "request", "rescue", "research", "resolve", "respond", "restored", "restructure", "retire", "retrieve", "return", "review", "revise", "rhyme", "rid", "ride", "ring", "rinse", "rise", "risk", "rob", "rock", "roll", "rot", "rub", "ruin", "rule", "run", "rush", "sack", "sail", "satisfy", "save", "saw", "say", "scare", "scatter", "schedule", "scold", "scorch", "scrape", "scratch", "scream", "screw", "scribble", "scrub", "seal", "search", "secure", "see", "seek", "select", "sell", "send", "sense", "separate", "serve", "service", "set", "settle", "sew", "shade", "shake", "shape", "share", "shave", "shear", "shed", "shelter", "shine", "shiver", "shock", "shoe", "shoot", "shop", "show", "shrink", "shrug", "shut", "sigh", "sign", "signal", "simplify", "sin", "sing", "sink", "sip", "sit", "sketch", "ski", "skip", "slap", "slay", "sleep", "slide", "sling", "slink", "slip", "slit", "slow", "smash", "smell", "smile", "smite", "smoke", "snatch", "sneak", "sneeze", "sniff", "snore", "snow", "soak", "solve", "soothe", "soothsay", "sort", "sound", "sow", "spare", "spark", "sparkle", "speak", "specify", "speed", "spell", "spend", "spill", "spin", "spit", "split", "spoil", "spot", "spray", "spread", "spring", "sprout", "squash", "squeak", "squeal", "squeeze", "stain", "stamp", "stand", "stare", "start", "stay", "steal", "steer", "step", "stick", "stimulate", "sting", "stink", "stir", "stitch", "stop", "store", "strap", "streamline", "strengthen", "stretch", "stride", "strike", "string", "strip", "strive", "stroke", "structure", "study", "stuff", "sublet", "subtract", "succeed", "suck", "suffer", "suggest", "suit", "summarize", "supervise", "supply", "support", "suppose", "surprise", "surround", "suspect", "suspend", "swear", "sweat", "sweep", "swell", "swim", "swing", "switch", "symbolize", "synthesize", "systemize", "tabulate", "take", "talk", "tame", "tap", "target", "taste", "teach", "tear", "tease", "telephone", "tell", "tempt", "terrify", "test", "thank", "thaw", "think", "thrive", "throw", "thrust", "tick", "tickle", "tie", "time", "tip", "tire", "touch", "tour", "tow", "trace", "trade", "train", "transcribe", "transfer", "transform", "translate", "transport", "trap", "travel", "tread", "treat", "tremble", "trick", "trip", "trot", "trouble", "troubleshoot", "trust", "try", "tug", "tumble", "turn", "tutor", "twist", "type", "undergo", "understand", "undertake", "undress", "unfasten", "unify", "unite", "unlock", "unpack", "untidy", "update", "upgrade", "uphold", "upset", "use", "utilize", "vanish", "verbalize", "verify", "vex", "visit", "wail", "wait", "wake", "walk", "wander", "want", "warm", "warn", "wash", "waste", "watch", "water", "wave", "wear", "weave", "wed", "weep", "weigh", "welcome", "wend", "wet", "whine", "whip", "whirl", "whisper", "whistle", "win", "wind", "wink", "wipe", "wish", "withdraw", "withhold", "withstand", "wobble", "wonder", "work", "worry", "wrap", "wreck", "wrestle", "wriggle", "wring", "write", "x-ray", "yawn", "yell", "zip", "zoom" };

        private string[] nouns = { "ball", "bat", "bed", "book", "boy", "bun", "can", "cake", "cap", "car", "cat", "cow", "cub", "cup", "dad", "day", "dog", "doll", "dust", "fan", "feet", "girl", "gun", "hall", "hat", "hen", "jar", "kite", "man", "map", "men", "mom", "pan", "pet", "pie", "pig", "pot", "rat", "son", "sun", "toe", "tub", "van", "apple", "arm", "banana", "bike", "bird", "book", "chin", "clam", "class", "clover", "club", "corn", "crayon", "crow", "crown", "crowd", "crib", "desk", "dime", "dirt", "dress", "fang", "field", "flag", "flower", "fog", "game", "heat", "hill", "home", "horn", "hose", "joke", "juice", "kite", "lake", "maid", "mask", "mice", "milk", "mint", "meal", "meat", "moon", "mother", "morning", "name", "nest", "nose", "pear", "pen", "pencil", "plant", "rain", "river", "road", "rock", "room", "rose", "seed", "shape", "shoe", "shop", "show", "sink", "snail", "snake", "snow", "soda", "sofa", "star", "step", "stew", "stove", "straw", "string", "summer", "swing", "table", "tank", "team", "tent", "test", "toes", "tree", "vest", "water", "wing", "winter", "woman", "women", "alarm", "animal", "aunt", "bait", "balloon", "bath", "bead", "beam", "bean", "bedroom", "boot", "bread", "brick", "brother", "camp", "chicken", "children", "crook", "deer", "dock", "doctor", "downtown", "drum", "dust", "eye", "family", "father", "fight", "flesh", "food", "frog", "goose", "grade", "grandfather", "grandmother", "grape", "grass", "hook", "horse", "jail", "jam", "kiss", "kitten", "light", "loaf", "lock", "lunch", "lunchroom", "meal", "mother", "notebook", "owl", "pail", "parent", "park", "plot", "rabbit", "rake", "robin", "sack", "sail", "scale", "sea", "sister", "soap", "song", "spark", "space", "spoon", "spot", "spy", "summer", "tiger", "toad", "town", "trail", "tramp", "tray", "trick", "trip", "uncle", "vase", "winter", "water", "week", "wheel", "wish", "wool", "yard", "zebra", "actor", "airplane", "airport", "army", "baseball", "beef", "birthday", "boy", "brush", "bushes", "butter", "cast", "cave", "cent", "cherries", "cherry", "cobweb", "coil", "cracker", "dinner", "eggnog", "elbow", "face", "fireman", "flavor", "gate", "glove", "glue", "goldfish", "goose", "grain", "hair", "haircut", "hobbies", "holiday", "hot", "jellyfish", "ladybug", "mailbox", "number", "oatmeal", "pail", "pancake", "pear", "pest", "popcorn", "queen", "quicksand", "quiet", "quilt", "rainstorm", "scarecrow", "scarf", "stream", "street", "sugar", "throne", "toothpaste", "twig", "volleyball", "wood", "wrench", "advice", "anger", "answer", "apple", "arithmetic", "badge", "basket", "basketball", "battle", "beast", "beetle", "beggar", "brain", "branch", "bubble", "bucket", "cactus", "cannon", "cattle", "celery", "cellar", "cloth", "coach", "coast", "crate", "cream", "daughter", "donkey", "drug", "earthquake", "feast", "fifth", "finger", "flock", "frame", "furniture", "geese", "ghost", "giraffe", "governor", "honey", "hope", "hydrant", "icicle", "income", "island", "jeans", "judge", "lace", "lamp", "lettuce", "marble", "month", "north", "ocean", "patch", "plane", "playground", "poison", "riddle", "rifle", "scale", "seashore", "sheet", "sidewalk", "skate", "slave", "sleet", "smoke", "stage", "station", "thrill", "throat", "throne", "title", "toothbrush", "turkey", "underwear", "vacation", "vegetable", "visitor", "voyage", "year", "able", "achieve", "acoustics", "action", "activity", "aftermath", "afternoon", "afterthought", "apparel", "appliance", "beginner", "believe", "bomb", "border", "boundary", "breakfast", "cabbage", "cable", "calculator", "calendar", "caption", "carpenter", "cemetery", "channel", "circle", "creator", "creature", "education", "faucet", "feather", "friction", "fruit", "fuel", "galley", "guide", "guitar", "health", "heart", "idea", "kitten", "laborer", "language", "lawyer", "linen", "locket", "lumber", "magic", "minister", "mitten", "money", "mountain", "music", "partner", "passenger", "pickle", "picture", "plantation", "plastic", "pleasure", "pocket", "police", "pollution", "railway", "recess", "reward", "route", "scene", "scent", "squirrel", "stranger", "suit", "sweater", "temper", "territory", "texture", "thread", "treatment", "veil", "vein", "volcano", "wealth", "weather", "wilderness", "wren", "wrist", "writer", "account", "achiever", "acoustics", "act", "action", "activity", "actor", "addition", "adjustment", "advertisement", "advice", "aftermath", "afternoon", "afterthought", "agreement", "air", "airplane", "airport", "alarm", "amount", "amusement", "anger", "angle", "animal", "answer", "ant", "ants", "apparatus", "apparel", "apple", "apples", "appliance", "approval", "arch", "argument", "arithmetic", "arm", "army", "art", "attack", "attempt", "attention", "attraction", "aunt", "authority", "babies", "baby", "back", "badge", "bag", "bait", "balance", "ball", "balloon", "balls", "banana", "band", "base", "baseball", "basin", "basket", "basketball", "bat", "bath", "battle", "bead", "beam", "bean", "bear", "bears", "beast", "bed", "bedroom", "beds", "bee", "beef", "beetle", "beggar", "beginner", "behavior", "belief", "believe", "bell", "bells", "berry", "bike", "bikes", "bird", "birds", "birth", "birthday", "bit", "bite", "blade", "blood", "blow", "board", "boat", "boats", "body", "bomb", "bone", "book", "books", "boot", "border", "bottle", "boundary", "box", "boy", "boys", "brain", "brake", "branch", "brass", "bread", "breakfast", "breath", "brick", "bridge", "brother", "brothers", "brush", "bubble", "bucket", "building", "bulb", "bun", "burn", "burst", "bushes", "business", "butter", "button", "cabbage", "cable", "cactus", "cake", "cakes", "calculator", "calendar", "camera", "camp", "can", "cannon", "canvas", "cap", "caption", "car", "card", "care", "carpenter", "carriage", "cars", "cart", "cast", "cat", "cats", "cattle", "cause", "cave", "celery", "cellar", "cemetery", "cent", "chain", "chair", "chairs", "chalk", "chance", "change", "channel", "cheese", "cherries", "cherry", "chess", "chicken", "chickens", "children", "chin", "church", "circle", "clam", "class", "clock", "clocks", "cloth", "cloud", "clouds", "clover", "club", "coach", "coal", "coast", "coat", "cobweb", "coil", "collar", "color", "comb", "comfort", "committee", "company", "comparison", "competition", "condition", "connection", "control", "cook", "copper", "copy", "cord", "cork", "corn", "cough", "country", "cover", "cow", "cows", "crack", "cracker", "crate", "crayon", "cream", "creator", "creature", "credit", "crib", "crime", "crook", "crow", "crowd", "crown", "crush", "cry", "cub", "cup", "current", "curtain", "curve", "cushion", "dad", "daughter", "day", "death", "debt", "decision", "deer", "degree", "design", "desire", "desk", "destruction", "detail", "development", "digestion", "dime", "dinner", "dinosaurs", "direction", "dirt", "discovery", "discussion", "disease", "disgust", "distance", "distribution", "division", "dock", "doctor", "dog", "dogs", "doll", "dolls", "donkey", "door", "downtown", "drain", "drawer", "dress", "drink", "driving", "drop", "drug", "drum", "duck", "ducks", "dust", "ear", "earth", "earthquake", "edge", "education", "effect", "egg", "eggnog", "eggs", "elbow", "end", "engine", "error", "event", "example", "exchange", "existence", "expansion", "experience", "expert", "eye", "eyes", "face", "fact", "fairies", "fall", "family", "fan", "fang", "farm", "farmer", "father", "father", "faucet", "fear", "feast", "feather", "feeling", "feet", "fiction", "field", "fifth", "fight", "finger", "finger", "fire", "fireman", "fish", "flag", "flame", "flavor", "flesh", "flight", "flock", "floor", "flower", "flowers", "fly", "fog", "fold", "food", "foot", "force", "fork", "form", "fowl", "frame", "friction", "friend", "friends", "frog", "frogs", "front", "fruit", "fuel", "furniture", "alley", "game", "garden", "gate", "geese", "ghost", "giants", "giraffe", "girl", "girls", "glass", "glove", "glue", "goat", "gold", "goldfish", "good-bye", "goose", "government", "governor", "grade", "grain", "grandfather", "grandmother", "grape", "grass", "grip", "ground", "group", "growth", "guide", "guitar", "gun", "hair", "haircut", "hall", "hammer", "hand", "hands", "harbor", "harmony", "hat", "hate", "head", "health", "hearing", "heart", "heat", "help", "hen", "hill", "history", "hobbies", "hole", "holiday", "home", "honey", "hook", "hope", "horn", "horse", "horses", "hose", "hospital", "hot", "hour", "house", "houses", "humor", "hydrant", "ice", "icicle", "idea", "impulse", "income", "increase", "industry", "ink", "insect", "instrument", "insurance", "interest", "invention", "iron", "island", "jail", "jam", "jar", "jeans", "jelly", "jellyfish", "jewel", "join", "joke", "journey", "judge", "juice", "jump", "kettle", "key", "kick", "kiss", "kite", "kitten", "kittens", "kitty", "knee", "knife", "knot", "knowledge", "laborer", "lace", "ladybug", "lake", "lamp", "land", "language", "laugh", "lawyer", "lead", "leaf", "learning", "leather", "leg", "legs", "letter", "letters", "lettuce", "level", "library", "lift", "light", "limit", "line", "linen", "lip", "liquid", "list", "lizards", "loaf", "lock", "locket", "look", "loss", "love", "low", "lumber", "lunch", "lunchroom", "machine", "magic", "maid", "mailbox", "man", "manager", "map", "marble", "mark", "market", "mask", "mass", "match", "meal", "measure", "meat", "meeting", "memory", "men", "metal", "mice", "middle", "milk", "mind", "mine", "minister", "mint", "minute", "mist", "mitten", "mom", "money", "monkey", "month", "moon", "morning", "mother", "motion", "mountain", "mouth", "move", "muscle", "music", "nail", "name", "nation", "neck", "need", "needle", "nerve", "nest", "net", "news", "night", "noise", "north", "nose", "note", "notebook", "number", "nut", "oatmeal", "observation", "ocean", "offer", "office", "oil", "operation", "opinion", "orange", "oranges", "order", "organization", "ornament", "oven", "owl", "owner", "page", "pail", "pain", "paint", "pan", "pancake", "paper", "parcel", "parent", "park", "part", "partner", "party", "passenger", "paste", "patch", "payment", "peace", "pear", "pen", "pencil", "person", "pest", "pet", "pets", "pickle", "picture", "pie", "pies", "pig", "pigs", "pin", "pipe", "pizzas", "place", "plane", "planes", "plant", "plantation", "plants", "plastic", "plate", "play", "playground", "pleasure", "plot", "plough", "pocket", "point", "poison", "police", "polish", "pollution", "popcorn", "porter", "position", "pot", "potato", "powder", "power", "price", "print", "prison", "process", "produce", "profit", "property", "prose", "protest", "pull", "pump", "punishment", "purpose", "push", "quarter", "quartz", "queen", "question", "quicksand", "quiet", "quill", "quilt", "quince", "quiver", "rabbit", "rabbits", "rail", "railway", "rain", "rainstorm", "rake", "range", "rat", "rate", "ray", "reaction", "reading", "reason", "receipt", "recess", "record", "regret", "relation", "religion", "representative", "request", "respect", "rest", "reward", "rhythm", "rice", "riddle", "rifle", "ring", "rings", "river", "road", "robin", "rock", "rod", "roll", "roof", "room", "root", "rose", "route", "rub", "rule", "run", "sack", "sail", "salt", "sand", "scale", "scarecrow", "scarf", "scene", "scent", "school", "science", "scissors", "screw", "sea", "seashore", "seat", "secretary", "seed", "selection", "self", "sense", "servant", "shade", "shake", "shame", "shape", "sheep", "sheet", "shelf", "ship", "shirt", "shock", "shoe", "shoes", "shop", "show", "side", "sidewalk", "sign", "silk", "silver", "sink", "sister", "sisters", "size", "skate", "skin", "skirt", "sky", "slave", "sleep", "sleet", "slip", "slope", "smash", "smell", "smile", "smoke", "snail", "snails", "snake", "snakes", "sneeze", "snow", "soap", "society", "sock", "soda", "sofa", "son", "song", "songs", "sort", "sound", "soup", "space", "spade", "spark", "spiders", "sponge", "spoon", "spot", "spring", "spy", "square", "squirrel", "stage", "stamp", "star", "start", "statement", "station", "steam", "steel", "stem", "step", "stew", "stick", "sticks", "stitch", "stocking", "stomach", "stone", "stop", "store", "story", "stove", "stranger", "straw", "stream", "street", "stretch", "string", "structure", "substance", "sugar", "suggestion", "suit", "summer", "sun", "support", "surprise", "sweater", "swim", "swing", "system", "table", "tail", "talk", "tank", "taste", "tax", "teaching", "team", "teeth", "temper", "tendency", "tent", "territory", "test", "texture", "theory", "thing", "things", "thought", "thread", "thrill", "throat", "throne", "thumb", "thunder", "ticket", "tiger", "time", "tin", "title", "toad", "toe", "toes", "tomatoes", "tongue", "tooth", "toothbrush", "toothpaste", "top", "touch", "town", "toy", "toys", "trade", "trail", "train", "trains", "tramp", "transport", "tray", "treatment", "tree", "trees", "trick", "trip", "trouble", "trousers", "truck", "trucks", "tub", "turkey", "turn", "twig", "twist", "umbrella", "uncle", "underwear", "unit", "use", "vacation", "value", "van", "vase", "vegetable", "veil", "vein", "verse", "vessel", "vest", "view", "visitor", "voice", "volcano", "volleyball", "voyage", "walk", "wall", "war", "wash", "waste", "watch", "water", "wave", "waves", "wax", "way", "wealth", "weather", "week", "weight", "wheel", "whip", "whistle", "wilderness", "wind", "window", "wine", "wing", "winter", "wire", "wish", "woman", "women", "wood", "wool", "word", "work", "worm", "wound", "wren", "wrench", "wrist", "writer", "writing", "yak", "yam", "yard", "yarn", "year", "yoke", "zebra", "zephyr", "zinc", "zipper", "zoo" };

        private string[] adjectives = { "quizzical", "highfalutin", "dynamic", "wakeful", "cheerful", "thoughtful", "cooperative", "questionable", "abundant", "uneven", "yummy", "juicy", "vacuous", "concerned", "young", "sparkling", "abhorrent", "sweltering", "late", "macho", "scrawny", "friendly", "kaput", "divergent", "busy", "charming", "protective", "premium", "puzzled", "waggish", "rambunctious", "puffy", "hard", "fat", "sedate", "yellow", "resonant", "dapper", "courageous", "vast", "cool", "elated", "wary", "bewildered", "level", "wooden", "ceaseless", "tearful", "cloudy", "gullible", "flashy", "trite", "quick", "nondescript", "round", "slow", "spiritual", "brave", "tenuous", "abstracted", "colossal", "sloppy", "obsolete", "elegant", "fabulous", "vivacious", "exuberant", "faithful", "helpless", "odd", "sordid", "blue", "imported", "ugly", "ruthless", "deeply", "eminent", "reminiscent", "rotten", "sour", "volatile", "succinct", "judicious", "abrupt", "learned", "stereotyped", "evanescent", "efficacious", "festive", "loose", "torpid", "condemned", "selective", "strong", "momentous", "ordinary", "dry", "great", "ultra", "ahead", "broken", "dusty", "piquant", "creepy", "miniature", "periodic", "equable", "unsightly", "narrow", "grieving", "whimsical", "fantastic", "kindhearted", "miscreant", "cowardly", "cloistered", "marked", "bloody", "chunky", "undesirable", "oval", "nauseating", "aberrant", "stingy", "standing", "distinct", "illegal", "angry", "faint", "rustic", "few", "calm", "gorgeous", "mysterious", "tacky", "unadvised", "greasy", "minor", "loving", "melodic", "flat", "wretched", "clever", "barbarous", "pretty", "endurable", "handsomely", "unequaled", "acceptable", "symptomatic", "hurt", "tested", "long", "warm", "ignorant", "ashamed", "excellent", "known", "adamant", "eatable", "verdant", "meek", "unbiased", "rampant", "somber", "cuddly", "harmonious", "salty", "overwrought", "stimulating", "beautiful", "crazy", "grouchy", "thirsty", "joyous", "confused", "terrible", "high", "unarmed", "gabby", "wet", "sharp", "wonderful", "magenta", "tan", "huge", "productive", "defective", "chilly", "needy", "imminent", "flaky", "fortunate", "neighborly", "hot", "husky", "optimal", "gaping", "faulty", "guttural", "massive", "watery", "abrasive", "ubiquitous", "aspiring", "impartial", "annoyed", "billowy", "lucky", "panoramic", "heartbreaking", "fragile", "purring", "wistful", "burly", "filthy", "psychedelic", "harsh", "disagreeable", "ambiguous", "short", "splendid", "crowded", "light", "yielding", "hypnotic", "dispensable", "deserted", "nonchalant", "green", "puny", "deafening", "classy", "tall", "typical", "exclusive", "materialistic", "mute", "shaky", "inconclusive", "rebellious", "doubtful", "telling", "unsuitable", "woebegone", "cold", "sassy", "arrogant", "perfect", "adhesive", "industrious", "crabby", "curly", "voiceless", "nostalgic", "better", "slippery", "willing", "nifty", "orange", "victorious", "ritzy", "wacky", "vigorous", "spotless", "good", "powerful", "bashful", "soggy", "grubby", "moaning", "placid", "permissible", "half", "towering", "bawdy", "measly", "abaft", "delightful", "goofy", "capricious", "nonstop", "addicted", "acoustic", "furtive", "erratic", "heavy", "square", "delicious", "needless", "resolute", "innocent", "abnormal", "hurried", "awful", "impossible", "aloof", "giddy", "large", "pointless", "petite", "jolly", "boundless", "abounding", "hilarious", "heavenly", "honorable", "squeamish", "red", "phobic", "trashy", "pathetic", "parched", "godly", "greedy", "pleasant", "small", "aboriginal", "dashing", "icky", "bumpy", "laughable", "hapless", "silent", "scary", "shaggy", "organic", "unbecoming", "inexpensive", "wrong", "repulsive", "flawless", "labored", "disturbed", "aboard", "gusty", "loud", "jumbled", "exotic", "vulgar", "threatening", "belligerent", "synonymous", "encouraging", "fancy", "embarrassed", "clumsy", "fast", "ethereal", "chubby", "high-pitched", "plastic", "open", "straight", "little", "ancient", "fair", "psychotic", "murky", "earthy", "callous", "heady", "lamentable", "hallowed", "obtainable", "toothsome", "oafish", "gainful", "flippant", "tangy", "tightfisted", "damaging", "utopian", "gaudy", "brainy", "imperfect", "shiny", "fanatical", "snotty", "relieved", "shallow", "foamy", "parsimonious", "gruesome", "elite", "wide", "kind", "bored", "tangible", "depressed", "boring", "screeching", "outrageous", "determined", "picayune", "glossy", "historical", "staking", "curious", "gigantic", "wandering", "profuse", "vengeful", "glib", "unaccountable", "frightened", "outstanding", "chivalrous", "workable", "modern", "swanky", "comfortable", "gentle", "substantial", "brawny", "curved", "nebulous", "boorish", "afraid", "fierce", "efficient", "lackadaisical", "recondite", "internal", "absorbed", "squealing", "frail", "thundering", "wanting", "cooing", "axiomatic", "debonair", "boiling", "tired", "numberless", "flowery", "mushy", "enthusiastic", "proud", "upset", "hungry", "astonishing", "deadpan", "prickly", "mammoth", "absurd", "clean", "jittery", "wry", "entertaining", "literate", "lying", "uninterested", "aquatic", "super", "languid", "cute", "absorbing", "scattered", "brief", "halting", "bright", "fuzzy", "lethal", "scarce", "aggressive", "obsequious", "fine", "giant", "holistic", "pastoral", "stormy", "quaint", "nervous", "wasteful", "grotesque", "loutish", "abiding", "unable", "black", "dysfunctional", "knowledgeable", "truculent", "various", "luxuriant", "shrill", "spiffy", "guarded", "colorful", "misty", "spurious", "freezing", "glamorous", "famous", "new", "instinctive", "nasty", "exultant", "seemly", "tawdry", "maniacal", "wrathful", "shy", "nutritious", "idiotic", "worried", "bad", "stupid", "ruddy", "wholesale", "naughty", "thoughtless", "futuristic", "available", "slimy", "cynical", "fluffy", "plausible", "nasty", "tender", "changeable", "smiling", "oceanic", "satisfying", "steadfast", "ugliest", "crooked", "subsequent", "fascinated", "woozy", "teeny", "quickest", "moldy", "uppity", "sable", "horrible", "silly", "ad hoc", "numerous", "berserk", "wiry", "knowing", "lazy", "childlike", "zippy", "fearless", "pumped", "weak", "tacit", "weary", "rapid", "precious", "smoggy", "swift", "lyrical", "steep", "quack", "direful", "talented", "hesitant", "fallacious", "ill", "quarrelsome", "quiet", "flipped-out", "didactic", "fluttering", "glorious", "tough", "sulky", "elfin", "abortive", "sweet", "habitual", "supreme", "hollow", "possessive", "inquisitive", "adjoining", "incandescent", "lowly", "majestic", "bizarre", "acrid", "expensive", "aback", "unusual", "foolish", "jobless", "capable", "damp", "political", "dazzling", "erect", "Early", "immense", "hellish", "omniscient", "reflective", "lovely", "incompetent", "empty", "breakable", "educated", "easy", "devilish", "assorted", "decorous", "jaded", "homely", "dangerous", "adaptable", "coherent", "dramatic", "tense", "abject", "fretful", "troubled", "diligent", "solid", "plain", "raspy", "irate", "offbeat", "healthy", "melted", "cagey", "many", "wild", "venomous", "animated", "alike", "youthful", "ripe", "alcoholic", "sincere", "teeny-tiny", "lush", "defeated", "zonked", "foregoing", "dizzy", "frantic", "obnoxious", "funny", "damaged", "grandiose", "spectacular", "maddening", "defiant", "makeshift", "strange", "painstaking", "merciful", "madly", "clammy", "itchy", "difficult", "clear", "used", "temporary", "abandoned", "null", "rainy", "evil", "alert", "domineering", "amuck", "rabid", "jealous", "robust", "obeisant", "overt", "enchanting", "longing", "cautious", "motionless", "bitter", "anxious", "craven", "breezy", "ragged", "skillful", "quixotic", "knotty", "grumpy", "dark", "draconian", "alluring", "magical", "versed", "humdrum", "accurate", "ludicrous", "sleepy", "envious", "lavish", "roasted", "thinkable", "overconfident", "roomy", "painful", "wee", "observant", "old-fashioned", "drunk", "royal", "likeable", "adventurous", "eager", "obedient", "attractive", "x-rated", "spooky", "poised", "righteous", "excited", "real", "abashed", "womanly", "ambitious", "lacking", "testy", "big", "gamy", "early", "auspicious", "blue-eyed", "discreet", "nappy", "vague", "helpful", "nosy", "perpetual", "disillusioned", "overrated", "gleaming", "tart", "soft", "agreeable", "therapeutic", "accessible", "poor", "gifted", "old", "humorous", "flagrant", "magnificent", "alive", "understood", "economic", "mighty", "ablaze", "racial", "tasteful", "purple", "broad", "lean", "legal", "witty", "nutty", "icy", "feigned", "redundant", "adorable", "apathetic", "jumpy", "scientific", "combative", "worthless", "tasteless", "voracious", "jazzy", "uptight", "utter", "hospitable", "imaginary", "finicky", "shocking", "dead", "noisy", "shivering", "subdued", "rare", "zealous", "demonic", "ratty", "snobbish", "deranged", "muddy", "whispering", "credible", "hulking", "fertile", "tight", "abusive", "functional", "obscene", "thankful", "daffy", "smelly", "lively", "homeless", "secretive", "amused", "lewd", "mere", "agonizing", "sad", "innate", "sneaky", "noxious", "illustrious", "alleged", "cultured", "tame", "macabre", "lonely", "mindless", "low", "scintillating", "statuesque", "decisive", "rhetorical", "hysterical", "happy", "earsplitting", "mundane", "spicy", "overjoyed", "taboo", "peaceful", "forgetful", "elderly", "upbeat", "squalid", "warlike", "dull", "plucky", "handsome", "groovy", "absent", "wise", "romantic", "invincible", "receptive", "smooth", "different", "tiny", "cruel", "dirty", "mature", "faded", "tiresome", "wicked", "average", "panicky", "detailed", "juvenile", "scandalous", "steady", "wealthy", "deep", "sticky", "jagged", "wide-eyed", "tasty", "disgusted", "garrulous", "graceful", "tranquil", "annoying", "hissing", "noiseless", "selfish", "onerous", "lopsided", "ossified", "penitent", "malicious", "aromatic", "successful", "zany", "evasive", "wet", "naive", "nice", "uttermost", "brash", "muddled", "energetic", "accidental", "silky", "guiltless", "important", "drab", "aware", "skinny", "careful", "rightful", "tricky", "sore", "rich", "blushing", "stale", "daily", "watchful", "uncovered", "rough", "fresh", "hushed", "rural" };

        #endregion

        public string[] Verbs { get { return verbs; } }

        public string[] Nouns { get { return nouns; } }

        public string[] Adjectives { get { return adjectives; } }

        public void Initialise()
        {
            if (WordInput == null)
            {
                throw new Exception("WordInput has not been specified.");
            }
            ApiHttpClient = new HttpClient();
            ApiHttpClient.BaseAddress = new Uri($"https://www.dictionaryapi.com/api/v3/references/thesaurus/json/{WordInput}?key={THESAURUS_API_KEY}");

            // remove default headers - GET json
            ApiHttpClient.DefaultRequestHeaders.Accept.Clear();
            ApiHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> MakeRequest()
        {
            if (ApiHttpClient == null) Initialise();

            HttpResponseMessage response = ApiHttpClient.GetAsync(ApiHttpClient.BaseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                throw new Exception("API didn't respond");
            }
        }

        public async Task<ThesaurusApiResponseModel> Fetch()
        {
            string json = await MakeRequest();
            ThesaurusApiResponseHelper helper = new ThesaurusApiResponseHelper();
            return helper.DeserialiseResult(json);
        }

        public void Deinitialise()
        {
            ApiHttpClient.Dispose();
            ApiHttpClient = null;
        }
    }

    public class ThesaurusApiResponseHelper
    {
        private Random random = new Random();

        // deserialise to synonyms, antonyms, and short definitions
        public ThesaurusApiResponseModel DeserialiseResult(string wholeJsonResponse)
        {
            StringBuilder parsedJson = new StringBuilder();

            // lazy so that multiple definitions are not matched
            string regexPredicate = @"\""syns\"".+?(?=,\""offensive\"")";
            parsedJson.Append("{" + Regex.Match(wholeJsonResponse, regexPredicate).Value + ",");

            regexPredicate = @"\""shortdef\"".+?(?=\])";
            parsedJson.Append(Regex.Match(wholeJsonResponse, regexPredicate).Value + "]}");

            return JsonConvert.DeserializeObject<ThesaurusApiResponseModel>(parsedJson.ToString());
        }

        public (string[] words, int synonymIndex) ShuffleWords(List<string> synonyms, List<string> antonyms)
        {
            if (synonyms.Count == 0) throw new Exception("Synonyms list does not contain any elements");
            if (antonyms.Count == 0) throw new Exception("Antonyms list does not contain any elements");

            int randomSynIndex = random.Next(0, synonyms.Count);

            string[] shuffledWords = new string[4];
            int newSynIndex = random.Next(0, 4);
            shuffledWords[newSynIndex] = synonyms[randomSynIndex];

            Console.WriteLine(string.Concat(shuffledWords));
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine($"{i}: {shuffledWords[i] == null}");
                if (shuffledWords[i] == null)
                {
                    shuffledWords[i] = antonyms[random.Next(0, antonyms.Count)];
                }
            }

            return (shuffledWords, newSynIndex);
        }
    }

    public enum _WordType
    {
        Noun,
        Verb,
        Adjective
    }

    public class ThesaurusQuestionHelper
    {
        private string wordInput;

        public string WordInput { get { return wordInput; } private set { wordInput = value; } }

        public ThesaurusQuestionHelper(string wordInput)
        {
            this.wordInput = wordInput;
        }

        public ThesaurusQuestionHelper(_WordType wordType)
        {
            this.wordInput = GetRandomWordFromType(wordType);
        }

        public async Task<(string[] words, int synonymIndex)> SynAntQuestion()
        {
            ThesaurusApiHandling thesaurusApiHandling = new ThesaurusApiHandling(wordInput);
            ThesaurusApiResponseModel response = await thesaurusApiHandling.Fetch();
            ThesaurusApiResponseHelper helper = new ThesaurusApiResponseHelper();
            return helper.ShuffleWords(response.Syns, response.Ants);
        }

        /// <summary>
        /// Creates a list of 4 definitions, with the first definition in the list relating to the queried word.
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> DefinitionMatchQuestion(_WordType wordType)
        {
            List<string> defs = new List<string>();

            ThesaurusApiHandling thesaurusApiHandling = new ThesaurusApiHandling(wordInput);
            ThesaurusApiResponseModel response = await thesaurusApiHandling.Fetch();

            defs.Add(response.Shortdef.First());

            for (int i = 0; i < 3; i++)
            {
                thesaurusApiHandling.WordInput = GetRandomWordFromType(wordType);

                thesaurusApiHandling.Deinitialise();
                response = await thesaurusApiHandling.Fetch();
                defs.Add(response.Shortdef.First());
            }

            return defs;
        }

        private string GetRandomWordFromType(_WordType wordType)
        {
            string result;
            ThesaurusApiHandling thesaurusApiHandling = new ThesaurusApiHandling();

            switch (wordType)
            {
                case _WordType.Noun:
                    result = thesaurusApiHandling.Nouns[new Random().Next(thesaurusApiHandling.Nouns.Length)];
                    break;
                case _WordType.Verb:
                    result = thesaurusApiHandling.Verbs[new Random().Next(thesaurusApiHandling.Verbs.Length)];
                    break;
                default:
                    result = thesaurusApiHandling.Adjectives[new Random().Next(thesaurusApiHandling.Adjectives.Length)];
                    break;
            }

            return result;
        }
    }

    public class ThesaurusApiResponseModel
    {
        public List<List<string>> syns;
        public List<List<string>> ants;
        public List<string> shortdef;

        private List<string> Flatten(List<List<string>> list)
        {
            List<string> result = new List<string>();
            foreach (List<string> singleList in list)
            {
                result.AddRange(singleList);
            }
            return result;
        }

        public List<string> Syns
        {
            get
            {
                return Flatten(syns);
            }
            private set { }
        }

        public List<string> Ants
        {
            get
            {
                return Flatten(ants);
            }
            private set { }
        }

        public List<string> Shortdef
        {
            get
            {
                return shortdef;
            }
            private set { }
        }
    }
}