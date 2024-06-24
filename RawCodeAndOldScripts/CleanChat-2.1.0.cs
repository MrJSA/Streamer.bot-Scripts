using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        string command = args.ContainsKey("command") ? args["command"].ToString().ToLower() : "";
        // check status of the filter
        bool badwordfilterstatus = bool.Parse(CPH.GetGlobalVar<string>("badwordfilterstatus", false));
        bool linkfilterstatus = bool.Parse(CPH.GetGlobalVar<string>("linkfilterstatus", false));
        // Get bools for different things
        bool botmessages = bool.Parse(args["botmessages"].ToString());
        bool checkmods = bool.Parse(args["checkmods"].ToString());
        bool checkvips = bool.Parse(args["checkvips"].ToString());
        bool ismod = bool.Parse(args["isModerator"].ToString());
        bool isvip = bool.Parse(args["isVip"].ToString());
        // check if user can send message
        bool cansendmessage = (!checkmods && ismod) || (!checkvips && isvip);
        // Get Variable for the triggers used
        string enablebadwordfilter = args["enablebadwordfilter"].ToString().ToLower();
        string disablebadwordfilter = args["disablebadwordfilter"].ToString().ToLower();
        string enablelinkblock = args["enablelinkblock"].ToString().ToLower();
        string disablelinkblock = args["disablelinkblock"].ToString().ToLower();
        string addbadword = args["addbadword"].ToString().ToLower();
        string removebadword = args["removebadword"].ToString().ToLower();
        string listbadword = args["listbadword"].ToString().ToLower();
        string permit = args["permit"].ToString().ToLower();
        // Check the command that is used (Big Iff / Uff)
        if (command == enablebadwordfilter && badwordfilterstatus)
        {
            CPH.SendMessage("BadWordFilter Already Enabled", botmessages);
            return true;
        }
        else if (command == enablebadwordfilter && !badwordfilterstatus)
        {
        	badwordfilterstatus = true;
        	CPH.SetGlobalVar("badwordfilterstatus", badwordfilterstatus, false);
            CPH.SendMessage("BadWordFilter Enabled", botmessages);
            return true;
        }
        else if (command == disablebadwordfilter && badwordfilterstatus)
        {
        	badwordfilterstatus = false;
        	CPH.SetGlobalVar("badwordfilterstatus", badwordfilterstatus, false);
            CPH.SendMessage("BadWordFilter Disabled", botmessages);
            return true;
        }
        else if (command == disablebadwordfilter && !badwordfilterstatus)
        {
            CPH.SendMessage("BadWordFilter Already Disabled", botmessages);
            return true;
        }
        else if (command == enablelinkblock && linkfilterstatus)
        {
            CPH.SendMessage("Linkblock Already Enabled", botmessages);
            return true;
        }
        else if (command == enablelinkblock && !linkfilterstatus)
        {
        	linkfilterstatus = true;
        	CPH.SetGlobalVar("linkfilterstatus", linkfilterstatus, false);
            CPH.SendMessage("Linkblock Enabled", botmessages);
            return true;
        }
        else if (command == disablelinkblock && linkfilterstatus)
        {
        	linkfilterstatus = false;
        	CPH.SetGlobalVar("linkfilterstatus", linkfilterstatus, false);
            CPH.SendMessage("Linkblock Disabled", botmessages);
            return true;
        }
        else if (command == disablelinkblock && !linkfilterstatus)
        {
            CPH.SendMessage("Linkblock Already Disabled", botmessages);
            return true;
        }
        else if (command == addbadword)
        {
			Addbadword(botmessages);
			return true;
        }
        else if (command == removebadword)
        {
            Removebadword(botmessages);
			return true;
        }
        else if (command == listbadword)
        {
            Listbadwords(botmessages);
			return true;
        }
        else if (command == permit)
        {
        	Permit(botmessages);
        	return true;
        }
		else if (badwordfilterstatus && !linkfilterstatus && !cansendmessage)
		{
			Badwordfilter(botmessages);
			return true;
		}
		else if (!badwordfilterstatus && linkfilterstatus && !cansendmessage)
		{
			Linkblock(botmessages);
			return true;
		}
		else if (linkfilterstatus && badwordfilterstatus && !cansendmessage)
		{
			Badwordfilter(botmessages);
			Linkblock(botmessages);
			return true;
		}
        return true;
    }
    // Extracted Code for the Badwordfilter
    private bool Badwordfilter(bool botmessages)
    {
		// Get Path to .txt file set in Global Variable
		string pathToFile = args["BadWordListPath"].ToString();
		// Get Message that needs to be checked
		string message = args["message"].ToString();
		// Open the file for reading
		using (StreamReader reader = new StreamReader(pathToFile))
		{
			bool foundMatch = false;
			string line;
			// Read lines from the file and check if any line matches the entered string
			while ((line = reader.ReadLine()) != null)
			{
				RegexOptions regexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase;
				if (Regex.IsMatch(message, $@"\b{Regex.Escape(line)}\b", regexOptions))
				{
					foundMatch = true;
					break;
				}
			}
			if (foundMatch)
			{
				// get Username that wrote the Message
				string chatterName = args["user"].ToString();
				// Message before the Username, set here or in Arguments
				string badwordmessage1 = args["badwordmessage1"].ToString();
				// Message before the Username, set here or in Arguments
				string badwordmessage2 = args["badwordmessage2"].ToString();
				// Do you want Timeout? set here or in arguments "yes" or "no"
				bool badwordtimeout = bool.Parse(args["badwordtimeout"].ToString());
				// check if its Chatters First Message
				bool firstMessage = bool.Parse(args["firstMessage"].ToString());
				// Do you want to ban first word offenders?
				bool banfirstwordoffender = bool.Parse(args["banfirstwordoffender"].ToString());
				if (firstMessage && banfirstwordoffender)
				{
					CPH.TwitchBanUser(chatterName, "Spambot", false);
				}
				else if (badwordtimeout)
				{
					// Timeout Time set to wanted Value in arguments or here. Whatever floats your boat
					int timeouttime = Convert.ToInt32(args["timeouttime"].ToString());
					// Reason for Timeout for your Logs, set here or in arguments
					string badwordtimeoutreason = args["badwordtimeoutreason"].ToString();
					CPH.TwitchTimeoutUser(chatterName, timeouttime, badwordtimeoutreason, false);
					CPH.SendMessage(badwordmessage1 + chatterName + badwordmessage2, botmessages);
				}
				else
				{
					// Get Message Id
					string messageId = args["msgId"].ToString();
					CPH.TwitchDeleteChatMessage(messageId, false);
					CPH.SendMessage(badwordmessage1 + chatterName + badwordmessage2, botmessages);
				}
			}
		}
		return true;
	}
	// Extracted Code for the Linkblock
	private bool Linkblock(bool botmessages)
	{
		// Whitelist add the URLs yout want to allow here. by default only your Twitch channel is allowed
		string streamerName = args["broadcastUserName"].ToString();
		List<string> whitelist = new List<string>()
		{
			"twitch.tv/" + streamerName,
		};
		bool whitelisted = false;
		string permitUser = CPH.GetGlobalVar<string>("permitUser", false);
		string chatterName = args["user"].ToString();
		if (permitUser == null || permitUser == "" || chatterName != permitUser)
		{
			string domainList = @"com|net|org|de|icu|uk|info|top|xyz|aaa|aarp|abarth|abb|abbott|abbvie|abc|able|abogado|abudhabi|ac|academy|accenture|accountant|accountants|aco|actor|ad|adac|ads|adult|ae|aeg|aero|aetna|af|afamilycompany|afl|africa|ag|agakhan|agency|ai|aig|airbus|airforce|airtel|akdn|al|alfaromeo|alibaba|alipay|allfinanz|allstate|ally|alsace|alstom|am|amazon|americanexpress|americanfamily|amex|amfam|amica|amsterdam|analytics|android|anquan|anz|ao|aol|apartments|app|apple|aq|aquarelle|ar|arab|aramco|archi|army|arpa|art|arte|as|asda|asia|associates|at|athleta|attorney|au|auction|audi|audible|audio|auspost|author|auto|autos|avianca|aw|aws|ax|axa|az|azure|ba|baby|baidu|banamex|bananarepublic|band|bank|bar|barcelona|barclaycard|barclays|barefoot|bargains|baseball|basketball|bauhaus|bayern|bb|bbc|bbt|bbva|bcg|bcn|bd|be|beats|beauty|beer|bentley|berlin|best|bestbuy|bet|bf|bg|bh|bharti|bi|bible|bid|bike|bing|bingo|bio|biz|bj|black|blackfriday|blockbuster|blog|bloomberg|blue|bm|bms|bmw|bn|bnpparibas|bo|boats|boehringer|bofa|bom|bond|boo|book|booking|bosch|bostik|boston|bot|boutique|box|br|bradesco|bridgestone|broadway|broker|brother|brussels|bs|bt|budapest|bugatti|build|builders|business|buy|buzz|bv|bw|by|bz|bzh|ca|cab|cafe|cal|call|calvinklein|cam|camera|camp|cancerresearch|canon|capetown|capital|capitalone|car|caravan|cards|care|career|careers|cars|casa|case|cash|casino|cat|catering|catholic|cba|cbn|cbre|cbs|cc|cd|center|ceo|cern|cf|cfa|cfd|cg|ch|chanel|channel|charity|chase|chat|cheap|chintai|christmas|chrome|church|ci|cipriani|circle|cisco|citadel|citi|citic|city|cityeats|ck|cl|claims|cleaning|click|clinic|clinique|clothing|cloud|club|clubmed|cm|cn|co|coach|codes|coffee|college|cologne|com|comcast|commbank|community|company|compare|computer|comsec|condos|construction|consulting|contact|contractors|cooking|cookingchannel|cool|coop|corsica|country|coupon|coupons|courses|cpa|cr|credit|creditcard|creditunion|cricket|crown|crs|cruise|cruises|csc|cu|cuisinella|cv|cw|cx|cy|cymru|cyou|cz|dabur|dad|dance|data|date|dating|datsun|day|dclk|dds|de|deal|dealer|deals|degree|delivery|dell|deloitte|delta|democrat|dental|dentist|desi|design|dev|dhl|diamonds|diet|digital|direct|directory|discount|discover|dish|diy|dj|dk|dm|dnp|do|docs|doctor|dog|domains|dot|download|drive|dtv|dubai|duck|dunlop|dupont|durban|dvag|dvr|dz|earth|eat|ec|eco|edeka|edu|education|ee|eg|email|emerck|energy|engineer|engineering|enterprises|epson|equipment|er|ericsson|erni|es|esq|estate|et|etisalat|eu|eurovision|eus|events|exchange|expert|exposed|express|extraspace|fage|fail|fairwinds|faith|family|fan|fans|farm|farmers|fashion|fast|fedex|feedback|ferrari|ferrero|fi|fiat|fidelity|fido|film|final|finance|financial|fire|firestone|firmdale|fish|fishing|fit|fitness|fj|fk|flickr|flights|flir|florist|flowers|fly|fm|fo|foo|food|foodnetwork|football|ford|forex|forsale|forum|foundation|fox|fr|free|fresenius|frl|frogans|frontdoor|frontier|ftr|fujitsu|fun|fund|furniture|futbol|fyi|ga|gal|gallery|gallo|gallup|game|games|gap|garden|gay|gb|gbiz|gd|gdn|ge|gea|gent|genting|george|gf|gg|ggee|gh|gi|gift|gifts|gives|giving|gl|glade|glass|gle|global|globo|gm|gmail|gmbh|gmo|gmx|gn|godaddy|gold|goldpoint|golf|goo|goodyear|goog|google|gop|got|gov|gp|gq|gr|grainger|graphics|gratis|green|gripe|grocery|group|gs|gt|gu|guardian|gucci|guge|guide|guitars|guru|gw|gy|hair|hamburg|hangout|haus|hbo|hdfc|hdfcbank|health|healthcare|help|helsinki|here|hermes|hgtv|hiphop|hisamitsu|hitachi|hiv|hk|hkt|hm|hn|hockey|holdings|holiday|homedepot|homegoods|homes|homesense|honda|horse|hospital|host|hosting|hot|hoteles|hotels|hotmail|house|how|hr|hsbc|ht|hu|hughes|hyatt|hyundai|ibm|icbc|ice|icu|id|ie|ieee|ifm|ikano|il|im|imamat|imdb|immo|immobilien|in|inc|industries|infiniti|info|ing|ink|institute|insurance|insure|int|international|intuit|investments|io|ipiranga|iq|ir|irish|is|ismaili|ist|istanbul|it|itau|itv|jaguar|java|jcb|je|jeep|jetzt|jewelry|jio|jll|jm|jmp|jnj|jo|jobs|joburg|jot|joy|jp|jpmorgan|jprs|juegos|juniper|kaufen|kddi|ke|kerryhotels|kerrylogistics|kerryproperties|kfh|kg|kh|ki|kia|kim|kinder|kindle|kitchen|kiwi|km|kn|koeln|komatsu|kosher|kp|kpmg|kpn|kr|krd|kred|kuokgroup|kw|ky|kyoto|kz|la|lacaixa|lamborghini|lamer|lancaster|lancia|land|landrover|lanxess|lasalle|lat|latino|latrobe|law|lawyer|lb|lc|lds|lease|leclerc|lefrak|legal|lego|lexus|lgbt|li|lidl|life|lifeinsurance|lifestyle|lighting|like|lilly|limited|limo|lincoln|linde|link|lipsy|live|living|lixil|lk|llc|llp|loan|loans|locker|locus|loft|lol|london|lotte|lotto|love|lpl|lplfinancial|lr|ls|lt|ltd|ltda|lu|lundbeck|luxe|luxury|lv|ly|ma|macys|madrid|maif|maison|makeup|man|management|mango|map|market|marketing|markets|marriott|marshalls|maserati|mattel|mba|mc|mckinsey|md|me|med|media|meet|melbourne|meme|memorial|men|menu|merckmsd|mg|mh|miami|microsoft|mil|mini|mint|mit|mitsubishi|mk|ml|mlb|mls|mm|mma|mn|mo|mobi|mobile|moda|moe|moi|mom|monash|money|monster|mormon|mortgage|moscow|moto|motorcycles|mov|movie|mp|mq|mr|ms|msd|mt|mtn|mtr|mu|museum|music|mutual|mv|mw|mx|my|mz|na|nab|nagoya|name|natura|navy|nba|nc|ne|nec|net|netbank|netflix|network|neustar|new|news|next|nextdirect|nexus|nf|nfl|ng|ngo|nhk|ni|nico|nike|nikon|ninja|nissan|nissay|nl|no|nokia|northwesternmutual|norton|now|nowruz|nowtv|np|nr|nra|nrw|ntt|nu|nyc|nz|obi|observer|off|office|okinawa|olayan|olayangroup|oldnavy|ollo|om|omega|one|ong|onl|online|ooo|open|oracle|orange|org|organic|origins|osaka|otsuka|ott|ovh|pa|page|panasonic|paris|pars|partners|parts|party|passagens|pay|pccw|pe|pet|pf|pfizer|pg|ph|pharmacy|phd|philips|phone|photo|photography|photos|physio|pics|pictet|pictures|pid|pin|ping|pink|pioneer|pizza|pk|pl|place|play|playstation|plumbing|plus|pm|pn|pnc|pohl|poker|politie|porn|post|pr|pramerica|praxi|press|prime|pro|prod|productions|prof|progressive|promo|properties|property|protection|pru|prudential|ps|pt|pub|pw|pwc|py|qa|qpon|quebec|quest|racing|radio|raid|re|read|realestate|realtor|realty|recipes|red|redstone|redumbrella|rehab|reise|reisen|reit|reliance|ren|rent|rentals|repair|report|republican|rest|restaurant|review|reviews|rexroth|rich|richardli|ricoh|ril|rio|rip|ro|rocher|rocks|rodeo|rogers|room|rs|rsvp|ru|rugby|ruhr|run|rw|rwe|ryukyu|sa|saarland|safe|safety|sakura|sale|salon|samsclub|samsung|sandvik|sandvikcoromant|sanofi|sap|sarl|sas|save|saxo|sb|sbi|sbs|sc|sca|scb|schaeffler|schmidt|scholarships|school|schule|schwarz|science|scjohnson|scot|sd|se|search|seat|secure|security|seek|select|sener|services|ses|seven|sew|sex|sexy|sfr|sg|sh|shangrila|sharp|shaw|shell|shia|shiksha|shoes|shop|shopping|shouji|show|showtime|si|silk|sina|singles|site|sj|sk|ski|skin|sky|skype|sl|sling|sm|smart|smile|sn|sncf|so|soccer|social|softbank|software|sohu|solar|solutions|song|sony|soy|spa|space|sport|spot|sr|srl|ss|st|stada|staples|star|statebank|statefarm|stc|stcgroup|stockholm|storage|store|stream|studio|study|style|su|sucks|supplies|supply|support|surf|surgery|suzuki|sv|swatch|swiss|sx|sy|sydney|systems|sz|tab|taipei|talk|taobao|target|tatamotors|tatar|tattoo|tax|taxi|tc|tci|td|tdk|team|tech|technology|tel|temasek|tennis|teva|tf|tg|th|thd|theater|theatre|tiaa|tickets|tienda|tiffany|tips|tires|tirol|tj|tjmaxx|tjx|tk|tkmaxx|tl|tm|tmall|tn|to|today|tokyo|tools|top|toray|toshiba|total|tours|town|toyota|toys|tr|trade|trading|training|travel|travelchannel|travelers|travelersinsurance|trust|trv|tt|tube|tui|tunes|tushu|tv|tvs|tw|tz|ua|ubank|ubs|ug|uk|unicom|university|uno|uol|ups|us|uy|uz|va|vacations|vana|vanguard|vc|ve|vegas|ventures|verisign|versicherung|vet|vg|vi|viajes|video|vig|viking|villas|vin|vip|virgin|visa|vision|viva|vivo|vlaanderen|vn|vodka|volkswagen|volvo|vote|voting|voto|voyage|vu|vuelos|wales|walmart|walter|wang|wanggou|watch|watches|weather|weatherchannel|webcam|weber|website|wed|wedding|weibo|weir|wf|whoswho|wien|wiki|williamhill|win|windows|wine|winners|wme|wolterskluwer|woodside|work|works|world|wow|ws|wtc|wtf|xbox|xerox|xfinity|xihuan|xin|xn--11b4c3d|xn--1ck2e1b|xn--1qqw23a|xn--2scrj9c|xn--30rr7y|xn--3bst00m|xn--3ds443g|xn--3e0b707e|xn--3hcrj9c|xn--3pxu8k|xn--42c2d9a|xn--45br5cyl|xn--45brj9c|xn--45q11c|xn--4dbrk0ce|xn--4gbrim|xn--54b7fta0cc|xn--55qw42g|xn--55qx5d|xn--5su34j936bgsg|xn--5tzm5g|xn--6frz82g|xn--6qq986b3xl|xn--80adxhks|xn--80ao21a|xn--80aqecdr1a|xn--80asehdb|xn--80aswg|xn--8y0a063a|xn--90a3ac|xn--90ae|xn--90ais|xn--9dbq2a|xn--9et52u|xn--9krt00a|xn--b4w605ferd|xn--bck1b9a5dre4c|xn--c1avg|xn--c2br7g|xn--cck2b3b|xn--cckwcxetd|xn--cg4bki|xn--clchc0ea0b2g2a9gcd|xn--czr694b|xn--czrs0t|xn--czru2d|xn--d1acj3b|xn--d1alf|xn--e1a4c|xn--eckvdtc9d|xn--efvy88h|xn--fct429k|xn--fhbei|xn--fiq228c5hs|xn--fiq64b|xn--fiqs8s|xn--fiqz9s|xn--fjq720a|xn--flw351e|xn--fpcrj9c3d|xn--fzc2c9e2c|xn--fzys8d69uvgm|xn--g2xx48c|xn--gckr3f0f|xn--gecrj9c|xn--gk3at1e|xn--h2breg3eve|xn--h2brj9c|xn--h2brj9c8c|xn--hxt814e|xn--i1b6b1a6a2e|xn--imr513n|xn--io0a7i|xn--j1aef|xn--j1amh|xn--j6w193g|xn--jlq480n2rg|xn--jlq61u9w7b|xn--jvr189m|xn--kcrx77d1x4a|xn--kprw13d|xn--kpry57d|xn--kput3i|xn--l1acc|xn--lgbbat1ad8j|xn--mgb9awbf|xn--mgba3a3ejt|xn--mgba3a4f16a|xn--mgba7c0bbn0a|xn--mgbaakc7dvf|xn--mgbaam7a8h|xn--mgbab2bd|xn--mgbah1a3hjkrd|xn--mgbai9azgqp6j|xn--mgbayh7gpa|xn--mgbbh1a|xn--mgbbh1a71e|xn--mgbc0a9azcg|xn--mgbca7dzdo|xn--mgbcpq6gpa1a|xn--mgberp4a5d4ar|xn--mgbgu82a|xn--mgbi4ecexp|xn--mgbpl2fh|xn--mgbt3dhd|xn--mgbtx2b|xn--mgbx4cd0ab|xn--mix891f|xn--mk1bu44c|xn--mxtq1m|xn--ngbc5azd|xn--ngbe9e0a|xn--ngbrx|xn--node|xn--nqv7f|xn--nqv7fs00ema|xn--nyqy26a|xn--o3cw4h|xn--ogbpf8fl|xn--otu796d|xn--p1acf|xn--p1ai|xn--pgbs0dh|xn--pssy2u|xn--q7ce6a|xn--q9jyb4c|xn--qcka1pmc|xn--qxa6a|xn--qxam|xn--rhqv96g|xn--rovu88b|xn--rvc1e0am3e|xn--s9brj9c|xn--ses554g|xn--t60b56a|xn--tckwe|xn--tiq49xqyj|xn--unup4y|xn--vermgensberater-ctb|xn--vermgensberatung-pwb|xn--vhquv|xn--vuq861b|xn--w4r85el8fhu5dnra|xn--w4rs40l|xn--wgbh1c|xn--wgbl6a|xn--xhq521b|xn--xkc2al3hye2a|xn--xkc2dl3a5ee0h|xn--y9a3aq|xn--yfro4i67o|xn--ygbi2ammx|xn--zfr164b|xxx|xyz|yachts|yahoo|yamaxun|yandex|ye|yodobashi|yoga|yokohama|you|youtube|yt|yun|za|zappos|zara|zero|zip|zm|zone|zuerich|zw";
			string linkdetect = @"[A-Za-zÁÀȦÂÄǞǍĂĀÃÅǺǼǢĆĊĈČĎḌḐḒÉÈĖÊËĚĔĒẼE̊ẸǴĠĜǦĞG̃ĢĤḤáàȧâäǟǎăāãåǻǽǣćċĉčďḍḑḓéèėêëěĕēẽe̊ẹǵġĝǧğg̃ģĥḥÍÌİÎÏǏĬĪĨỊĴĶǨĹĻĽĿḼM̂M̄ʼNŃN̂ṄN̈ŇN̄ÑŅṊÓÒȮȰÔÖȪǑŎŌÕȬŐỌǾƠíìiîïǐĭīĩịĵķǩĺļľŀḽm̂m̄ŉńn̂ṅn̈ňn̄ñņṋóòôȯȱöȫǒŏōõȭőọǿơP̄ŔŘŖŚŜṠŠȘṢŤȚṬṰÚÙÛÜǓŬŪŨŰŮỤẂẀŴẄÝỲŶŸȲỸŹŻŽẒǮp̄ŕřŗśŝṡšşṣťțṭṱúùûüǔŭūũűůụẃẁŵẅýỳŷÿȳỹźżžẓǯßœŒçÇ]+\.(" + domainList + @")(?: |\/|:|$|\\)";
			string ipdetect = @"|\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b";
			string message = args["message"].ToString();
			RegexOptions regexOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase;
			foreach (Match m in Regex.Matches(message, linkdetect + ipdetect, regexOptions))
			{
				bool allowed = false;
				foreach (string whitelistUrl in whitelist)
				{
					string whitelistPattern = Regex.Escape(whitelistUrl);
					if (Regex.IsMatch(message, whitelistPattern, RegexOptions.IgnoreCase))
					{
						allowed = true;
						break;
					}
				}
				if (!allowed)
				{
					// Message before the Username, set here or in Arguments
					string linkblockmessage1 = args["linkblockmessage1"].ToString();
					// Message before the Username, set here or in Arguments
					string linkblockmessage2 = args["linkblockmessage2"].ToString();
					// Do you want Timeout? set here or in arguments "yes" or "no"
					bool linkblocktimeout = bool.Parse(args["linkblocktimeout"].ToString());
					// check if its Chatters First Message
					bool firstMessage = bool.Parse(args["firstMessage"].ToString());
					// Do you want to ban first word offenders?
					bool banfirstwordoffender = bool.Parse(args["banfirstwordoffender"].ToString());
					if (firstMessage && banfirstwordoffender)
					{
						CPH.TwitchBanUser(chatterName, "Spambot", false);
					}
					else if (linkblocktimeout)
					{
						// Reason for Timeout for your Logs, set here or in arguments
						string linkblocktimeoutreason = args["linkblocktimeoutreason"].ToString();
						// Timeout Time set to wanted Value in arguments or here. Whatever floats your boat
						int timeouttime = Convert.ToInt32(args["timeouttime"].ToString());
						CPH.TwitchTimeoutUser(chatterName, timeouttime, linkblocktimeoutreason, false);
						CPH.SendMessage(linkblockmessage1 + chatterName + linkblockmessage2, botmessages);
					}
					else
					{
						// Get Message Id
						string messageId = args["msgId"].ToString();
						CPH.TwitchDeleteChatMessage(messageId, false);
						CPH.SendMessage(linkblockmessage1 + chatterName + linkblockmessage2, botmessages);
					}
				}
			}
		}
		return true;
	}
	// Extracted Code for the Listbadwords
	private bool Listbadwords(bool botmessages)
	{
		// Get User Name
		string user = args["userName"].ToString();
		// Get Path to .txt file set in Global Variable
		string filePath = args["BadWordListPath"].ToString();
		// Prepend String
		string prepend = "Here are the BadWords: ";
		// Open the original file for reading
		using (StreamReader reader = new StreamReader(filePath))
		{
			StringBuilder messageBuilder = new StringBuilder();
			string line;

			// Read lines from the file
			while ((line = reader.ReadLine()) != null)
			{
				// Remove newline characters from the line
				line = line.Replace("\n", "").Replace("\r", "");

				// Check if adding the line to the message would exceed 500 characters
				if (messageBuilder.Length + prepend.Length + line.Length + 1 > 500)
				{
					// Output the current message
					CPH.SendMessage(prepend + messageBuilder.ToString(), botmessages);

					// Start a new message with the current line
					messageBuilder.Clear();
					messageBuilder.Append(line);
				}
				else
				{
					// Add the line to the current message
					if (messageBuilder.Length > 0)
					{
						messageBuilder.Append(';');
						messageBuilder.Append(' '); // Add separator if needed
					}
					messageBuilder.Append(line);
				}
			}
			// Output the last message if any
			if (messageBuilder.Length > 0)
			{
				CPH.SendMessage(prepend + messageBuilder.ToString(), botmessages);
			}
		}
		return true;
	}
	// Extracted Code for the Removebadwords
	private bool Removebadword(bool botmessages)
	{
		// Get raw input of the Bad Word
		string rawInput = args["rawInput"].ToString();
		// Get User Name
		string user = args["userName"].ToString();
		// Get Path to .txt file set in Global Variable
		string filePath = args["BadWordListPath"].ToString();
		// Set a temporary Path to for a .txt file
		string tempFilePath = Path.GetTempFileName();
		bool lineDeleted = false;
		// Open the original file for reading
		using (StreamReader reader = new StreamReader(filePath))
		{
			// Open a temporary file for writing
			using (StreamWriter writer = new StreamWriter(tempFilePath))
			{
				string line;
				bool foundStringToDelete = false;
				// Read lines from the original file and write to the temporary file,
				// excluding lines that match the string to delete
				while ((line = reader.ReadLine()) != null)
				{
					if (!foundStringToDelete && line.Equals(rawInput, StringComparison.OrdinalIgnoreCase))
					{
						foundStringToDelete = true;
						lineDeleted = true;
						continue;
					}
					if (line.Trim() != "") // Skip empty lines
					{
						writer.WriteLine(line);
					}
				}
			}
		}
		// Replace the original file with the temporary file
		File.Delete(filePath);
		File.Move(tempFilePath, filePath);
		// Send confirmation message
		if (lineDeleted)
		{
			CPH.SendMessage(user + " deleted \"" + rawInput + "\" from the BadWordList", botmessages);
			return true;
		}
		else
		{
			CPH.SendMessage("Word or Phrase not found", botmessages);
			return true;
		}
		return true;
	}
	// Extracted Code for the Addbadwords
	private bool Addbadword(bool botmessages)
	{
		// Get raw input of the Bad Word
		string rawInput = args["rawInput"].ToString();
		// Get User Name
		string user = args["userName"].ToString();
		// Get Path to .txt file set in Global Variable
		string filePath = args["BadWordListPath"].ToString();
		using (StreamWriter outputFile = new StreamWriter(filePath, true))
		{
			// Write new BadWord in File
			outputFile.WriteLine(rawInput);
		}
		// Send confirmation message
		CPH.SendMessage(user + " added \"" + rawInput + "\" to the BadWordList", botmessages);
		return true;
	}
	// Extracted Code for the Permit
	private bool Permit(bool botmessages)
	{
		// Get the Time a permit is going to last
		int permittime = Convert.ToInt32(args["permittime"].ToString());
		// Get the Username that the permit is given to
		string targetuser = args["rawInput"].ToString();
		// Get name of the use currently being permitted
		string currentpermit = CPH.GetGlobalVar<string>("permitUser", false);
		if(currentpermit == null || currentpermit == "")
		{
			CPH.SetGlobalVar("permitUser", targetuser, false);
			CPH.SendMessage("/me " + targetuser + " you have permission to post links for " + permittime + " seconds.", botmessages);
		}
		else
		{
			CPH.SendMessage("/me Someone else is being permitted to post links currently.", botmessages);
		}
		// Wait for the rest of the permit time
		CPH.Wait(1000 * permittime);
		// Remove user from permit
		CPH.SetGlobalVar("permitUser", "", false);
		return true;
	}
	// Example Code iff i want to add more features
	/*
	private bool Copy()
	{
		return true;
	}*/
}
