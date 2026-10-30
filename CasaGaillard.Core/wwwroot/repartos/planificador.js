// ---- CONSTANTS & DATA ----
const DRIVERS=[
  {id:1,name:'SEBASTIÀ',canTractora:true,canCamion:true,canFurgon:true},
  {id:2,name:'PEDRO',canTractora:true,canCamion:true,canFurgon:true},
  {id:3,name:'CARLOS',canTractora:true,canCamion:true,canFurgon:true},
  {id:4,name:'PACO',canTractora:false,canCamion:true,canFurgon:true},
  {id:5,name:'PAQUITO',canTractora:true,canCamion:true,canFurgon:true},
  {id:6,name:'PEP',canTractora:false,canCamion:false,canFurgon:true}, // solo furgoneta ≤3500kg
  {id:7,name:'ROBERTO',canTractora:false,canCamion:false,canFurgon:true}, // solo furgoneta ≤3500kg
  {id:8,name:'TUGORES',canTractora:true,canCamion:true,canFurgon:true}
];
const VEHICLES=[
  {id:'T1',name:'7015HMR',type:'tractora'},
  {id:'T2',name:'9606KJW',type:'tractora'},
  {id:'T3',name:'5920MJK',type:'tractora'},
  {id:'T4',name:'5759KRT',type:'tractora'},
  {id:'C1',name:'5818GHD',type:'camion'},
  {id:'C2',name:'2904MJS',type:'camion'},
  {id:'F1',name:'1821KVZ',type:'furgon'},
  {id:'FV',name:'0094FVC',type:'furgoneta'}
];
const ZONES=['Palma','Raiguer / Interior','Nord','Llevant','Migjorn','Ponent','Tramuntana'];
 
// ---- CATALOGO DE ARTICULOS ----
const ART_NAMES={1000:'G.R.G. PLASTICO "TANK-GO" 1000 LIT.',2000:'G.R.G.ACERO INOXIDABLE 1000 LIT.',3000:'BIDON 200 L. PLASTICO TAPA FIJA',4000:'BIDON 120 L. PLASTICO ABERTURA TOTAL',4001:'BIDON 120 L. PLASTICO AZUL USADO',5000:'G.R.G. PLASTICO "TANK-GO" 1000 LIT. A.OX',10119:'BIDON 20 K. DISOLVENTE LIMPIEZA',90000:'ACIDO CLORHIDRICO 32% - 33%',90001:'ACIDO CLORHIDRICO 35%',90002:'ACIDO SULFURICO 98/99%',90004:'ACIDO NITRICO',90006:'ACIDO ACETICO 99%-GLACIAL',90007:'ACIDO ACETICO 80%',90008:'ÁCIDO PERACÉTICO 5% (DAO SQ 5 )',90010:'ACIDO FLUORHIDRICO 70%',90011:'AMONIACO SOLUCION 24\'5%',90012:'AMONIACO SOLUCIÓN 30 %',90013:'ACIDO CLORHIDRICO 5% G.R.G',90014:'ADBLUE JERRICAN',90015:'CLORITO SODICO 300 GRS./L.',90016:'NITRATO CALCICO SOLIDO',90018:'ÁCIDO CLORHÍDRICO 18%',90020:'NITRATO CÁLCICO SOLUCION',90023:'SOSA CAUSTICA LIQUIDA 30%',90024:'SOSA CAUSTICA LIQUIDA 20%',90025:'SOSA CAUSTICA LIQUIDA 50 %',90026:'SOSA CAUSTICA LIQUIDA 25%',90027:'SOSA CAUSTICA PERLAS',90028:'SOSA CAUSTICA LIQUIDA 15%',90029:'SOSA CAUSTICA LIQUIDA 32%',90030:'POTASA CAUSTICA ESCAMAS',90031:'POTASA CAUSTICA LÍQUIDA 50%',90033:'TIOSULFATO SÓDICO DISOLUCIÓN',90040:'CARBONATO SODICO LIGERO',90042:'BICARBONATO SODICO ALIMENT.',90043:'BICARBONATO AMONICO',90045:'CARBONATO POTASICO',90050:'ACIDO LACTICO 90%',90055:'ACIDO FORMICO 85%',90056:'ACIDO FORMICO 40%',90060:'ACIDO SULFURICO 40 %',90061:'ACIDO SULFURICO 15%',90075:'HIPOCLORITO SODICO 75 G/L CG-LEJIA',90080:'HIPOCLORITO SODICO 50 G/L CG-LEJIA',90090:'FOSFATO TRISODICO LIQUIDO 12%',90099:'AGUA OSMOTIZADA A GRANEL',90100:'ACIDO BORICO',90101:'ACIDO OXALICO',90102:'ACIDO CITRICO',90103:'ACIDO CITRICO SOLUCION 50%',90104:'ACIDO SULFAMICO',90107:'BORAX DECAHIDRATADO',90109:'METABISULFITO SODICO',90110:'BISULFITO SODICO LIQ.35%',90111:'BISULFATO SODICO',90113:'CARBOXIMETILCELULOSA',90114:'FOSFATO DIAMONICO',90115:'DEXTROSA',90116:'CLORURO CALCICO',90117:'FOSFATO MONOAMONICO',90118:'FOSFATO MONOSODICO ANHIDRO',90119:'FOSFATO BISODICO',90120:'FOSFATO TRISODICO',90121:'HIPOCLORITO SODICO 15% CG',90122:'HEXAMETAFOSFATO SODICO TECNICO',90123:'TIOSULFATO SÓDICO',90124:'ACIDO FOSFORICO 75%',90125:'METABISULFITO POTASICO',90126:'METASILICATO SODICO ANHIDRO',90127:'HEXAMETAFOSFATO SODICO ALIMENTICIO',90128:'HIPOCLORITO CALCICO',90129:'SILICATO SOSA NEUTRO',90130:'SULFATO DE SOSA',90131:'SULFATO ALUMINA SOLIDO',90132:'SULFATO COBRE',90133:'SULFATO HIERRO',90134:'SULFATO MAGNESICO',90135:'SULFURO SODICO',90136:'TRIPOLIFOSFATO SODICO',90138:'ETOXI-PROPANOL',90139:'METOXI-PROPANOL',90140:'FENOXIETANOL',90141:'BUTIL-GLICOL',90142:'BUTIL-DI-GLICOL',90143:'PROPILENGLICOL USP ALIMENTICIO',90144:'MONOETILENGLICOL(ETILENGLICOL)',90145:'PROPILENGLICOL TECNICO',90146:'AGUA OXIGENADA 30%',90147:'AGUA OXIGENADA 49\'5%',90148:'AGUA OXIGENADA 35%',90150:'METILATO SÓDICO 30%',90151:'TRIETANOLAMINA',90152:'DIETANOLAMINA',90153:'ALCOHOL BENZÍLICO',90154:'TRICLOROETILENO',90155:'TETRADECANO     (*)',90156:'ALCOHOL ISOPROPILICO',90157:'ALCOHOL ISOBUTILICO',90158:'METANOL',90159:'FORMOL 40%',90160:'CLOROFORMO',90161:'BUTANOL',90162:'PARAFINA BLOQUE 5 K.',90163:'MONALCOL 96º',90164:'MOSSTANOL \'L\'',90165:'ACETONA',90166:'N-DECANO (WS INODORO) *',90167:'HEPTANO     (*)',90168:'NITRATO POTASICO',90169:'NITRATO SODICO',90170:'NITRITO SODICO',90171:'AGUA OXIGENADA 12%',90172:'PIROFOSFATO TETRAPOTASICO',90174:'HIDROSULFITO SODICO(DITIONITO)',90176:'DABEERSEN NA4 TD(POLVO)',90179:'PERCLORETILENO',90180:'CLORURO MAGNESICO',90181:'CLORURO DE POTASA',90182:'CLORURO SODICO Q.P.',90183:'ACETATO SODICO Q.P.',90185:'POLICLORURO ALUMINIO-PAX 18-',90186:'CLORURO FERRICO LIQUIDO',90187:'SULFATO ALUMINA LIQUIDO',90188:'CHEMIFLOC CV/4080',90189:'FLUOSILICATO MAGNESIO',90190:'PERBORATO SODICO',90191:'GISCLOR-150',90192:'PERMANGANATO POTASICO',90193:'PERSULFATO AMONICO',90194:'PERCARBONATO SODICO',90195:'GLICERINA VEG USP',90196:'BIFLUORURO AMONICO',90198:'GLUCONATO SODICO',90199:'UREA TECNICA',90205:'ACETATO DE BUTILO',90206:'ACETATO DE ETILO',90207:'ACETATO METOXI PROPANOL',90215:'CLORURO DE METILENO',90245:'METIL-ETIL-CETONA',90250:'NAFTA LIGERA 16/18-PETROSOL(*)',90255:'WHITE SPIRIT D-40',90275:'TOLUENO     (*)',90276:'XILENO',90280:'WHITE-SPIRIT (*)',90290:'MEZCLA M-120/MON-AC.ETILO320',90500:'HIDROXIDO CALCICO',90501:'HIDROXIDO CALCICO EN SACOS',90506:'LITOPON',90510:'TALCO GRANEL',90520:'GREENPLUS GYPSUM GR',90521:'VERDE-CAL G GREENS SGN 90',90525:'CALCITA 1/3 MM',90530:'MARTOXID MDS-6',90535:'ALUMINA ACTIVADA F-200 1/4"',90550:'GRAFITO IMPALPABLE',90555:'SEPIOLITA 15/30',90600:'D.K.FLOC 1596',90605:'FLOCULANTE AS-23',90607:'SUPERFLOC 8398 -POLIELECTROLITO',90608:'KEMOPTI A-AC',90610:'ADICLENE EV-2',90611:'ADIC RO-20',90620:'LIMPIADOR P3 OSMOTECH 2575',90621:'LIMPIADOR P3 OSMOTECH 2691',90622:'LIMPIADOR P-3 ULTRAPERM 053',90625:'P-3 ULTRASIL 75',90626:'P-3 ULTRASIL 141',90627:'P-3 ULTRASIL 53',90628:'SACO 22\'5 K. P-3 ULTRASIL 53',90630:'ESTABILIZANTE DELSAZIN 5 K.',90650:'ANTI-INCRUSTANTE FLOCON 135',90655:'ANTI-INCRUSTANTE INCUS-CTR/3',90660:'NALCO 3D TRASAR 3DT 179',90690:'RESTIN 41 -HIDRAZINA 15% -',90705:'ANTICORROSIVO QMP-33',90710:'INHIBIDOR \'RODINE\' 31-A',90715:'INHIBIDOR \'RODINE\' XL-1060',90720:'INHIBIDOR RODINE 213',90725:'SEQUION 10-H-60',90726:'SEQUION CLR',90805:'CHEMIFLOC CH/80',90806:'CHEMIFLOC CH/82',91000:'BSA.1 K.SOSA CAUSTICA',91001:'FCO.SOSA CAUSTICA PERLAS 1 K.',91005:'BSA.1 K.ACIDO CITRICO',91013:'PERBORATO 750 G.',91018:'AZUFRE POLVO 750 GRS.',91020:'ALCOHOL ETILICO DESNAT.25 L.',91022:'METANOL 25 L.',91023:'ALCOHOL DE QUEMAR 1 LIT.',91025:'VASELINA LIQUIDA 1 L.',91030:'SOSA CAUSTICA LIQUIDA 1 LIT.',91050:'CART.FILT.POLIPROP.30"- 10 MICRAS',91051:'CART.FILT. SWRO.ZS 05-40',91098:'DIMETIL-FORMAMIDA',91099:'COBALTO 6%',92005:'ARLYPON VPC',92010:'ANTIESPUMANTE ZA-30',92013:'CONTRAPEN-BD/56 AW',92014:'CONTRAPEN CUR/818',92015:'CONTRAPEN 200 PLUS',92016:'ETILDIGLICOL',92020:'CADEPOL B5 (COMPERLAN COD/AC-44)',92035:'E.D.T.A.SOLUCION 40%',92037:'NTA-NA3-SOLUCION 40%',92038:'NTA NA 3 POLVO',92045:'HYDRODIS WP 562 N',92046:'HYDRODIS ADW 3814/N',92047:'HYDRODIS LW 3875',92050:'CRAFOL AP-60',92053:'DEHYDOL LT-7',92054:'DEMELAN LT-7/90',92055:'DEHYDOL PO-5',92056:'DEHYPON LS-54',92058:'DEHYPOUND LLD 60',92060:'SAL ENISAL-TABLETAS (MINERAL)',92061:'SAL TRAMPOLIN-TABLETAS (MARINA)',92065:'SAL ENISAL GRANO-PISCINAS (MARINA)',92066:'SAL GRANO FINA (ZONA S)',92080:'CATIOSAN 3085 (DEHYQUART AU-35)',92082:'DEHYQUART CC7/BZ',92093:'CLORURO DE BENZALCONIO 50%',92094:'Q-QUAT 50',92096:'DEHYTON PK 45',92097:'EUPERLAN AT-18',92099:'XYNO RESIN S-295 (EUPERLAN PO/N-(ATO-92)',92108:'SURCO CB-60',92109:'COMPUESTO CG-60',92110:'HUMECTANTE CG',92113:'DEMELAN CB-70',92117:'SURCO 1990 ( ABANS DEMELAN )',92120:'DEMELAN CM-33/BIO',92125:'DEMELAN CM-95',92135:'DEMELAN F-90',92175:'GLUADIN WLM BENZ',92180:'MILLONIC AG-210 (GLUCOPON 215)',92185:'GLUCOPON 650',92190:'LAMESOFT PO 65',92213:'ACIDO DODECIL-BENCENO SULFONICO (MARANIL',92220:'MARANIL PA-40/ZAO-02',92231:'POLYQUART AMPHO 149 (XYNOCRYL)',92233:'PLURAFAC LF-403',92234:'PLANTAPON SF-N',92235:'SULFOPON 101 UP',92236:'SULFOPON 1218G',92257:'DEHYTON PL',92260:'TEXAL V.P.',92261:'TEXAL SK7',92265:'TEXAPON N70 (PL-34)',92272:'TEXAPON NS0 MIT (PL-18)',92273:'LAURIL ETER SULFATO SODICO 27% (TEXAPON)',92275:'TEXAPON T/42-F',92285:'SOKALAN PA-40 ( DEMELAN PA-10 )',93000:'WHITE ALMOND',93001:'ALMENDRA 3091-2',93002:'GEL MAGNUM PB 26652',93003:'VERBENA 3038 V.P.INCOLORA',93004:'CUCUMBER WATER',93005:'MAGNOLIA 34660',93006:'FLOR DE LOTO',93007:'AZAHAR 3953 V.P.',93008:'TE ARBOL 24571',93009:'BITTER CHOCOLATE 26260',93010:'VIOLETA 14060 V.P.',93011:'CITRONELA SUMMER 20440',93012:'MANZANA 7000',93013:'MANZANA VERDE 11661',93014:'NEVASCA 2824',93015:'CHRISTMAS NIGHTS 55386',93016:'MIMOSA 26020',93017:'CITRONELA 3955 V.P',93018:'CITRONAL 23820',93019:'CITRUS 26560',93020:'CIMA 2548',93021:'CARAMELINA',93022:'MENTALIPTUS 21073',93023:'EUCALIPTUS HIDRO 23660',93024:'AMARGY 33490',93025:'FRESA 18234',93026:'FLEUR ORANGE 32950',93027:'FLOR DE ALMENDRO',93028:'MANGO FRUITY 33531',93029:'MELOCOTON HIDRO 20905',93030:'CHERIE 152',93031:'LAVANDA 7 HIERBAS',93032:'JAZMIN 36790',93033:'ROSABEL 31631',93034:'SEA SHORE',93035:'CANELA 10050 V.P.',93036:'FLORES FRESCAS NEW 20130',93037:'LAVANDA CONC V.P. 29240',93038:'LAVANDA NATURAL HIDRO 24490',93039:'SUAVI-LAVANDA 29360',93040:'FROSTED CRANBERRY 51895',93041:'LIMON 3961 V.P.',93042:'LIMON 33631',93043:'LAVANDA 3959 VP',93044:'CLEAN COTTON',93045:'LAVANDA 11950',93046:'CANELA-ORANGE 10120 V.P.',93047:'MANZANA VERDE 20541',93048:'CHOCOLATE VP 26451',93049:'PINO 23790',93050:'PINO 2701',93051:'HOME FOR CHRISTMAS',93052:'LIMAO 23930',93053:'LIMON 19500',93054:'LEMONGRASS & SUGARCANE',93055:'MELOCOTON 19090',93056:'VAINILLA 42250',93057:'TE VERDE 37772',93058:'CREMFRUIT 29271',93059:'FRESOTE 47501',93060:'LEDON RCO.',93061:'PATCHOULY EXTRA',93062:'AMBAR ESENCIA',93063:'CANELA 3899 V.P.',93064:'LAVANDA PROVENZA 30299',93065:'WHITE MUSK 41240',93066:'FIORI DI ARANCIO',93067:'JAZMIN DE INDONESIA',93068:'BRAVIO 6168',93069:'TUBEROSA 37980',93070:'JAZMIN 41280',93071:'AMBI LIMA-MANDARINA',93072:'FADA 37640',93073:'SWEET CHRISTMAS 52944',93074:'HIERBAS 2917',93075:'MORA 3965 VP',93076:'ROSA 1220',93077:'KIWI & LIME',93078:'ROSA SOLUBLE 5014',93079:'RABAT 53321',93080:'ROSA FRESCA 13560',93081:'ROSA 4053',93082:'RAIZ ANGELICA 49761',93083:'MULTIVERDE',93084:'MULTIMAGICO',93085:'MULTINEUTRO',93086:'POMELOFRESH',93087:'GREEN FIG 36931',93088:'SUAVIJER 6163',93089:'HYL 18832',93090:'LIMPIASUELOS 2600',93091:'MARSELLA 16182 L',93092:'RED MUSK',93093:'POMELO 32621',93094:'MARSELLA 21203',93095:'TAC 19182',93096:'MULTI LAVANDA',93097:'VAJILLAS 2414 F',93098:'CRISTALJER 2106',93099:'SUAVIJER 2675',93100:'PERFUME 50851',93101:'PERFUME 11902',93102:'ONE 29540',93103:'PERFUME 2910',93104:'LAVANDA 29751',93105:'CONDITIONER P-7',93106:'CIPRES 10802 V.P.',93107:'CHRISTMAS TREE',93108:'GARDENIA 3957 V.P.',93254:'PNC-400',93255:'DIPROPILENE GLICOL',93256:'CAMOMILA 26330',93260:'KOROLAN 6000',93264:'SANDALO 36022',93265:'PERFUME 44153',93266:'PERFUME 58130',93267:'REPELENTE INSECTOS 1911',93268:'REPELENTE INSECTOS 3662',93269:'REPELENTE PET 34391',93273:'MANGO 11010 V.P.',93274:'NARANJA 3784 V.P',93275:'CAFE V.P.31981',93276:'PINO V.P.3967',93277:'FRUTOS ROJOS 40753',93278:'GIN & ROSEWATER VP',93279:'FRUTOS ROJOS V.P.',93280:'ORQUIDEAS BLANCAS',93285:'ASIAN LOTUS',93286:'FLUORESCEINA SODICA 30%',93290:'MARINO 18310',93291:'LIMPIO 11440',93292:'GREEN APPLE 14986',93293:'DETERCRISTAL 24990',93294:'GLARY 25050',93295:'SOFT GREEN 23944',93296:'RUDA 21130',93297:'HECHIZO 30260',93301:'ECO FRESCOR 4619',93508:'AMARILLO HISPACID MG165%',93509:'AMARILLO LIMON T 85%',93510:'AZUL ACOSIL 151 CONC.16970',93511:'AZUL A LA GRASA B 95%',93512:'AZUL HISPACRIL G-5 300%',93514:'AZUL BRILLAN.85%-CONACERT',93517:'AZUL METILENO 2BF 140%',93518:'AZUL AQUAMAC 151',93527:'BRUN HISPACID RGN 120%',93528:'CARAMELINA BA',93530:'CARMESI HISPACIT\'G\'190%',93532:'MANDARINA PG 28732',93536:'TE VERDE 21065',93537:'ROSA BRILL.HISPACIT B-250%',93538:'AZUL BRILL.HISPACID BR-260%',93543:'AMARANTO 85%-CONACERT-E123',93544:'CARMOISINA 85% CONACERT',93550:'TURQUESA HISPALUZ 115%',93551:'CRISOL 14432',93650:'VERDE MENTA 23 CONACERT',93651:'VERDE MENTA AL AGUA TV 66%',93654:'VERDE MENTA HISP.ACT-155%',93660:'SUNSET YELLOW 85%-CONACER',93661:'INDIGO CARMIN CONACERT',93662:'PONCEAU 4R CONACERT',93701:'MAGNOLIA 3782 V.P.',93702:'OPTIBLANC NL',93703:'AMARILLO QUINOLEINA 70%',93704:'GEL DOVEN SA 22299',93705:'PINO DUCK 30355',93707:'CITRIC 7K',93708:'CITRICO 4020 CERA',93709:'ROSE & BERGAMOT 60960',93710:'FLORFRESH 2493',93711:'FLORAL 23810',93712:'FLORA 11900',93713:'ESPIGA LAVANDA 15951',93714:'PERFUME PET 34393',93715:'MENTA NATURAL HIDRO 24520',93716:'CITRONELA 12453',93717:'LOVELY PB 27362',93718:'TE VERDE 14760 V.P.',93719:'TE BLANCO NEW',93720:'ENEBRO EXTRA',93724:'NINETTE 31140',93725:'HIGH WIND 26990',93726:'MULTI TALCO',93727:'COCOA BUTTER NUTMEG & CARDAMON',93728:'SANDALO 21370',93729:'NINS 33670',93730:'ROSE 27090',93731:'APPLE SOAP 27750',93732:'CHOCOLATE 31411',93740:'MEDITERRANEO 17240',93741:'MENTA HIDRO 23740',93742:'BERGAMOTA NEW 10891',93743:'CEDRO 3897 V.P.',93744:'EUCALIPTUS ULTRA 22660',93745:'MENTA SN 4417',93750:'ROMERO 14540 V.P.',93751:'VAINILLA 20600',93755:'MANDARIN-ORANGE HIDROSOLUBLE 18560',93756:'SUAVI FRESCOR 41352',93757:'SWEET BERRY 38310',93758:'COCO 16260',93759:'EUCALIPTUS HIDRO 19921',93760:'TE ARBOL EXTRA',93761:'SUAVI MARSELLA VIP',93762:'ROSA 11450 VP',93763:'SUAVIBLUE BRISA 29400',93764:'SUAVIBLUE 23926',93765:'SUAVISOFT 23800',93766:'MANZANILLA EXTRA',93767:'SUAVIFRESH 24791',93768:'ESPLIEGO 11840 V.P.',93769:'MANDARINA EXTRA',93770:'LIMON EXTRA',93771:'LODILO 20771',93772:'LAVANDA EXTRA',93773:'LIMON SN 4471',93774:'ROMERO NATURAL HIDRO 24560',93775:'PERFUME 47280',93776:'NARANJA HIDRO 23720',93777:'ROSA HIDRO 23730',93778:'HIDRO MINT 15672',93779:'LAVANDA HIDRO 22360',93780:'EUCALIPTUS NATURAL HIDRO 24470',93781:'HIDRO-EUCALIPTUS 23660',93782:'HIDRO MARACUYA 11003',93783:'HIDRO CAMOMILA 20261',93784:'LIMON 28822',93785:'MENTALIPTUS P.G. 35160',93786:'PINO 29720',93787:'HIDRO ORQUIDEA 18091',93788:'COCO FRESH 28472',93789:'PERFUME 47420',93790:'AZAHAR RCO.',93791:'AZAHAR 18280 VP.INCO',93792:'HEALTH 25070',93793:'PERFUME 47300',93794:'LIMON HIDRO 23700',93795:'VAINILLA 10030 V.P.',93796:'COCO 3974 V.P.',93797:'ANTIESTRES HIDRO 23710',93798:'LAUREL 16710 V.P.',93799:'FRESA 14050 V.P.',94000:'DIOXPURE-REACTIVOS A',94005:'DIOXPURE-ACTIVADOR B',94010:'DIOXPURE 0\'75 %',95001:'TAPON ADAPTADOR 1 1/2" R 150-CAP',95002:'TAPON ADAPTADOR 2" R 200-CAP',95003:'ADAPTADOR R.H. 1 1/2" -R.150 A',95004:'ADAPTADOR R.H. 2" - R.200 A',95005:'TAPON ADAPTADOR 3" R.300-CAP',95011:'TAPON ACOPLE 1 1/2" R 150-PL',95012:'TAPON ACOPLE 2" R 200-PL',95013:'ACOPLAMIENTO R.H.1 1/2"-R.150D',95014:'ACOPLAMIENTO R.H. 2"-R.200 D',95015:'ACOPLAMIENTO R.H. 3"R.300-D',95090:'CONECTOR MANGUERA HB-150',95091:'CONECTOR MANGUERA 2"-50MM R.HB-200',99981:'BIDON PLASTICO 220 L.',99990:'CONTENEDOR 1000 L-P.E.HOMOLOG.',99993:'CONTENEDOR 1000L.\'UN SOLO USO\' USADO',99994:'CONTENEDOR 1000L.\'UN SOLO USO\'',99995:'JERRICAN PLASTICO 20 L. RECICLADO',99996:'JERRICAN PLASTICO 20 L. USADO',99997:'TAPON PTO. K-60 C/V.D-17',99998:'TAPON K-60 ROJO ESTANCO C/ARANDELA',99999:'JERRICAN PLASTICO 20 L.'};
const ART_FORMAT_LABEL={1:'Cisterna',2:'GRG',3:'Jerricane'};
function getArticleName(cod){
  const code=String(cod||'').trim();
  if(!/^\d+$/.test(code))return code;
  if(code.length>=6){
    const base=Number(code.slice(0,-1));
    const fmt=code.slice(-1);
    const name=ART_NAMES[base];
    const fmtLabel=ART_FORMAT_LABEL[fmt];
    if(name)return fmtLabel?`${name} (${fmtLabel})`:name;
  }else{
    const name=ART_NAMES[Number(code)];
    if(name)return name;
  }
  return code;
}
// El último dígito de un código de artículo de 6+ cifras indica el formato (ver ART_FORMAT_LABEL):
// '1' = Cisterna. Se usa para autoseleccionar Tipo/Descarga al introducir el producto en un reparto.
function isCisternaFormatCode(cod){
  const code=String(cod||'').trim();
  return /^\d{6,}$/.test(code)&&code.slice(-1)==='1';
}
 
// Extended population → zone mapping (normalized to uppercase for matching)
const POB_ZONE = {
  'PALMA':['Palma'],'PALMA DE MALLORCA':['Palma'],'PALMA MALLORCA':['Palma'],
  'COLL D\'EN RABASSA':['Palma'],'COLL D\'EN RABASSA-MALLORCA':['Palma'],
  'SON FERRIOL':['Palma'],'SANT JORDI':['Palma'],'S\'ARENAL':['Palma'],
  'MARRATXI':['Raiguer / Interior'],'MARRATXÍ':['Raiguer / Interior'],
  'SANTA MARIA':['Raiguer / Interior'],'SANTA MARIA-MALLORCA':['Raiguer / Interior'],'SANTA MARIA DEL CAMI':['Raiguer / Interior'],
  'CONSELL':['Raiguer / Interior'],'BINISSALEM':['Raiguer / Interior'],'BINISSALEM-MALLORCA':['Raiguer / Interior'],
  'LLOSETA':['Raiguer / Interior'],'LLOSETA-MALLORCA':['Raiguer / Interior'],
  'INCA':['Raiguer / Interior'],'INCA-MALLORCA':['Raiguer / Interior'],
  'SELVA':['Raiguer / Interior'],'CAMPANET':['Raiguer / Interior'],
  'CAIMARI':['Raiguer / Interior'],
  'PONT D\'INCA':['Raiguer / Interior'],'PONT D\'INCA-MALLORCA':['Raiguer / Interior'],
  'SINEU':['Raiguer / Interior'],'MARIA DE LA SALUT':['Raiguer / Interior'],
  'SANT JOAN':['Raiguer / Interior'],'ALGAIDA':['Raiguer / Interior'],'MONTUIRI':['Raiguer / Interior'],'MONTUÏRI':['Raiguer / Interior'],
  'PETRA':['Llevant'],
  'ALCUDIA':['Nord'],'ALCÚDIA':['Nord'],'POLLENSA':['Nord'],'POLLENÇA':['Nord'],
  'SA POBLA':['Nord'],'SA POBLA-MALLORCA':['Nord'],
  'MURO':['Nord'],'MURO-MALLORCA':['Nord'],
  'SANTA MARGALIDA':['Nord'],'CAN PICAFORT':['Nord'],'PORT D\'ALCUDIA':['Nord'],'PUERTO POLLENSA':['Nord'],'PUERTO POLLENÇA':['Nord'],
  'MANACOR':['Llevant'],'MANACOR-MALLORCA':['Llevant'],
  'ARTA':['Llevant'],'ARTÀ':['Llevant'],
  'SANT LLORENÇ':['Llevant'],'SON SERVERA':['Llevant'],
  'CAPDEPERA':['Llevant'],'FELANITX':['Llevant'],'FELANITX-MALLORCA':['Llevant'],
  'VILAFRANCA DE BONANY':['Llevant'],
  'LLUCMAJOR':['Migjorn'],'LLUCMAJOR-MALLORCA':['Migjorn'],
  'CAMPOS':['Migjorn'],'CAMPOS-MALLORCA':['Migjorn'],
  'SES SALINES':['Migjorn'],'SANTANYI':['Migjorn'],'SANTANYÍ':['Migjorn'],
  'COLONIA DE SANT JORDI':['Migjorn'],'CALA EGOS':['Migjorn'],'CALA EGOS (SANTANYI)':['Migjorn'],
  'ES LLOMBARDS':['Migjorn'],'ES LLOMBARDS-MALLORC':['Migjorn'],
  'CALVIA':['Ponent'],'CALVIÀ':['Ponent'],'ANDRATX':['Ponent'],'SANTA PONSA':['Ponent'], 'SANTA PONÇA':['Ponent'],
  'PUIGPUNYENT':['Ponent'],'MAGALUF':['Ponent'],'CAPDELLA':['Ponent'],
  'SOLLER':['Tramuntana'],'SÓLLER':['Tramuntana'],
  'VALLDEMOSSA':['Tramuntana'],'DEIA':['Tramuntana'],'DEIÀ':['Tramuntana'],
  'ESPORLES':['Tramuntana'],'BUNYOLA':['Tramuntana'],
  'POL.SON CASTELLO':['Palma'],'POL. SON CASTELLO':['Palma'],
};
 
const MUNI_ZONE={
  'Palma':['Palma'],'Marratxí':['Raiguer / Interior'],'Santa Maria del Camí':['Raiguer / Interior'],
  'Consell':['Raiguer / Interior'],'Binissalem':['Raiguer / Interior'],'Lloseta':['Raiguer / Interior'],
  'Inca':['Raiguer / Interior'],'Selva':['Raiguer / Interior'],'Campanet':['Raiguer / Interior'],
  'Búger':['Raiguer / Interior'],'Sencelles':['Raiguer / Interior'],'Costitx':['Raiguer / Interior'],
  'Lloret de Vistalegre':['Raiguer / Interior'],'Sineu':['Raiguer / Interior'],
  'Maria de la Salut':['Raiguer / Interior'],'Sant Joan':['Raiguer / Interior'],
  'Algaida':['Raiguer / Interior'],'Montuïri':['Raiguer / Interior'],'Petra':['Llevant'],'Porreres':['Migjorn'],
  'Alcúdia':['Nord'],'Pollença':['Nord'],'Sa Pobla':['Nord'],'Muro':['Nord'],
  'Santa Margalida':['Nord'],'Can Picafort':['Nord'],"Port d'Alcúdia":['Nord'],
  'Manacor':['Llevant'],'Artà':['Llevant'],'Sant Llorenç des Cardassar':['Llevant'],
  'Son Servera':['Llevant'],'Capdepera':['Llevant'],'Felanitx':['Llevant'],
  'Vilafranca de Bonany':['Llevant'],'Ariany':['Llevant'],
  'Llucmajor':['Migjorn'],'Campos':['Migjorn'],'Ses Salines':['Migjorn'],
  'Santanyí':['Migjorn'],"Colònia de Sant Jordi":['Migjorn'],
  'Calvià':['Ponent'],'Andratx':['Ponent'],'Puigpunyent':['Ponent'],
  'Estellencs':['Ponent'],'Banyalbufar':['Ponent'],'Magaluf':['Ponent'],
  'Sóller':['Tramuntana'],'Valldemossa':['Tramuntana'],'Deià':['Tramuntana'],
  'Esporles':['Tramuntana'],'Bunyola':['Tramuntana'],'Fornalutx':['Tramuntana'],'Escorca':['Tramuntana']
};
const ALL_MUNIS=Object.keys(MUNI_ZONE).sort();
const DAYS_ES=['Dom','Lun','Mar','Mié','Jue','Vie','Sáb'];
const MONTHS_ES=['ene','feb','mar','abr','may','jun','jul','ago','sep','oct','nov','dic'];
const ORDINALS=['1ª','2ª','3ª','4ª','5ª'];
const STORAGE_KEY='planificador_reparto_v6';
const DB_NAME='planificador_reparto_db';
const DB_VERSION=1;
const CURRENT_STORE='current';
const HISTORY_STORE='history';
const DRIVER_BY_ID=new Map(DRIVERS.map(d=>[d.id,d]));
const VEHICLE_BY_ID=new Map(VEHICLES.map(v=>[v.id,v]));
const POB_ZONE_KEYS=Object.keys(POB_ZONE);
const OUTSIDE_KW=['MADRID','BARCELONA','VALENCIA','SEVILLA','MENORCA','IBIZA','FORMENTERA','PENINSULA'];
 
// ---- GESTION DE FLOTA DE CISTERNAS (stock físico: cubas + plataformas) ----
const PORT_LABELS={BCN:'Barcelona',VLC:'Valencia'};
const PORT_CODES=['BCN','VLC'];
const BOAT_TRANSIT_DAYS=1; // días de barco Península → Baleares (ajustable si cambia la naviera)
const CISTERNA_STOCK_STORAGE_KEY='planificador_cisternas_stock_v1';
const PRODUCT_PORT_STORAGE_KEY='planificador_product_port_v1';
const REGISTRO_MOV_STORAGE_KEY='planificador_registro_movimientos_v1';
const HISTORY_STORAGE_KEY='planificador_history_v1';
const REGISTRO_MOV_CUTOFF_DATE='2026-08-01'; // El registro de arrastres solo se contabiliza a partir de esta fecha (indicación de Roberto)
 
let cisternaStock=[]; // registro de cubas físicas (CUBA + PLATAFORMA), importado de "Situación actual cubas"
let productPort={};   // { 'NOMBRE PRODUCTO NORMALIZADO': 'BCN' | 'VLC' } — puerto de carga fijo por producto
let registroMovimientos=[]; // arrastres/movimientos importados de "Registro movimientos", uno por GA
 
function normProduct(p){return normalizeStr(p);}
function isPortCode(loc){return PORT_CODES.includes(String(loc||'').trim().toUpperCase());}
function cisternaKey(cuba){return String(cuba||'').trim().toUpperCase();}
 
function getPortForProduct(producto){
  const key=normProduct(producto);
  if(!key)return 'BCN';
  if(productPort[key])return productPort[key];
  const words=key.split(' ').filter(w=>w.length>2);
  const found=Object.keys(productPort).find(k=>words.some(w=>k.includes(w)));
  return found?productPort[found]:'BCN';
}
function setPortForProduct(producto,port){
  const key=normProduct(producto);
  if(!key)return;
  productPort[key]=port;
  saveProductPort();
}
async function loadProductPort(){
  try{
    const r=await storageAdapter.get(PRODUCT_PORT_STORAGE_KEY);
    productPort=r&&r.value?JSON.parse(r.value):{};
  }catch(e){productPort={};}
}
async function saveProductPort(){
  try{await storageAdapter.set(PRODUCT_PORT_STORAGE_KEY,JSON.stringify(productPort));}catch(e){}
}
 
// Fecha ISO en que una cuba estará disponible (vacía) en un puerto peninsular
// Una cuba está "en isla" cuando su localización actual es conocida y NO es un puerto
// peninsular (BCN/VLC): eso significa que ya ha llegado a Mallorca, esté cargada o vacía.
function cisternaEnIsla(c){
  return !!c.localizacion&&!isPortCode(c.localizacion);
}
function cisternaFechaDisponible(c){
  if(isPortCode(c.localizacion))return c.fechaDisponible||localDateKey(new Date());
  // Si ya está en la isla, está disponible desde ya: "llegadaBarco" es la fecha estimada de
  // VUELTA a península (un viaje posterior), no una condición para usarla hoy en un reparto
  // dentro de Mallorca — por eso no hay que esperar a esa fecha ni descontarle tránsito de barco.
  if(cisternaEnIsla(c))return c.fechaDisponible||localDateKey(new Date());
  return c.llegadaBarco||null; // sin localización conocida: solo disponible cuando llegue el barco
}
// Ubicación actual DENTRO de Mallorca ('puerto' o 'base'), inferida del último movimiento conocido
// de la cuba — no de un campo estático. Reglas (indicación de Roberto):
//  - En la fecha de llegada del barco a Palma, la cuba está "en puerto" hasta que se le asigna un
//    arrastre o se asigna directamente a un servicio.
//  - Tras servir a un cliente, la cuba está "en base" hasta que se programa/asigna el arrastre de
//    vuelta al puerto (o el propio servicio ya tenía marcada la vuelta al puerto).
// Devuelve null si la cuba aún no ha llegado a la isla (está en péninsula o sin localización).
function cisternaMallorcaLocation(c){
  if(!cisternaEnIsla(c))return null;
  const events=[];
  const llegada=c.llegadaBarco||c.fechaDisponible;
  if(llegada)events.push({date:llegada,loc:'puerto',order:0});
  services.forEach(s=>{
    if(!s.day||cisternaKey(s.cisternaCuba)!==cisternaKey(c.cuba))return;
    if(s.type==='arrastre'){
      const trayecto=arrastreTrayectoField(s);
      events.push({date:s.day,loc:trayecto==='puerto_base'?'base':'puerto',order:1});
    }else if(s.type==='cisterna'){
      // Servicio de cisterna: entrega en cliente → base, salvo que el propio pedido tenga
      // programada la vuelta directa al puerto (destinoTrayecto).
      events.push({date:s.day,loc:s.destinoTrayecto==='puerto'?'puerto':'base',order:1});
    }
  });
  if(!events.length)return null;
  events.sort((a,b)=>a.date.localeCompare(b.date)||a.order-b.order);
  return events[events.length-1].loc;
}
function cisternaLocationLabel(c){
  const loc=cisternaMallorcaLocation(c);
  if(loc==='puerto')return 'Puerto de Palma';
  if(loc==='base')return 'Base';
  return c.localizacion||'—';
}

// Una asignación se considera "vigente" solo si la fecha del pedido al que está ligada la cuba
// no ha pasado ya. Si ese pedido era de un día anterior a hoy, se entiende que la entrega ya se
// hizo y la cuba ha vuelto a estar libre, aunque nadie haya pulsado "Liberar" a mano (caso típico:
// la cuba llega, se vacía en el cliente y se reembarca el mismo día para otro servicio).
function cisternaAsignadaVigente(c){
  if(!c.asignadoServicioId)return false;
  const todayKey=localDateKey(new Date());
  return !c.fechaEntregaAsignada||c.fechaEntregaAsignada>=todayKey;
}
function cisternaEstaDisponible(c){
  if(c.estadoManual==='fuera_servicio')return false;
  if(cisternaAsignadaVigente(c))return false;
  return isPortCode(c.localizacion)||cisternaEnIsla(c)||!!c.llegadaBarco;
}
function docCaducado(fechaISO,refISO){
  if(!fechaISO)return false; // sin dato: no bloquea la asignación, pero se marca como aviso
  return fechaISO<refISO;
}
function docsValidosHasta(c,refISO){
  return !docCaducado(c.itvCaduca,refISO)&&!docCaducado(c.adrCaduca,refISO)&&!docCaducado(c.imdgCaduca,refISO);
}
// Compara dos nombres de producto ya normalizados con solapamiento de palabras (>2 letras),
// igual que hace getPortForProduct: permite que "CLORHIDRICO" case con "ACIDO CLORHIDRICO 35%".
function productNamesMatch(a,b){
  if(!a||!b)return false;
  if(a===b)return true;
  const wordsA=a.split(' ').filter(w=>w.length>2);
  const wordsB=b.split(' ').filter(w=>w.length>2);
  if(!wordsA.length||!wordsB.length)return false;
  return wordsA.some(w=>b.includes(w))||wordsB.some(w=>a.includes(w));
}
function cisternaAptaParaProducto(c,producto){
  if(!producto)return true;
  const prodNorm=normProduct(producto);
  if(!c.productosAptos||!c.productosAptos.length)return productNamesMatch(normProduct(c.producto),prodNorm);
  return c.productosAptos.some(p=>productNamesMatch(normProduct(p),prodNorm));
}
 
// Fecha límite en la que la cuba debe estar cargada en el puerto de origen,
// restando el tránsito en barco a la fecha de entrega solicitada por el cliente
function fechaLimiteCarga(fechaEntregaISO){
  const d=new Date(fechaEntregaISO+'T00:00:00');
  d.setDate(d.getDate()-BOAT_TRANSIT_DAYS);
  return localDateKey(d);
}
 
// Candidatas ordenadas (mejor primero) para un producto + fecha de entrega solicitada
function getCisternaCandidates(producto,fechaEntregaISO,excludeCuba){
  if(!fechaEntregaISO)return [];
  const puerto=getPortForProduct(producto);
  const fechaLimite=fechaLimiteCarga(fechaEntregaISO);
  return cisternaStock
    .filter(c=>cisternaKey(c.cuba)!==cisternaKey(excludeCuba))
    .filter(c=>cisternaEstaDisponible(c))
    .filter(c=>cisternaAptaParaProducto(c,producto))
    .map(c=>{
      const disponible=cisternaFechaDisponible(c);
      const enIsla=cisternaEnIsla(c);
      // Si ya está en la isla no necesita tránsito de barco para este reparto: se considera
      // disponible siempre a tiempo, sin descontarle el día de barco ni exigir puerto correcto.
      const enPuertoCorrecto=enIsla||(isPortCode(c.localizacion)&&c.localizacion.toUpperCase()===puerto);
      const llegaraATiempo=enIsla||(!!disponible&&disponible<=fechaLimite);
      const docsOk=docsValidosHasta(c,fechaEntregaISO);
      return {...c,_puerto:puerto,_fechaLimite:fechaLimite,_disponible:disponible,_enIsla:enIsla,_enPuertoCorrecto:enPuertoCorrecto,_llegaraATiempo:llegaraATiempo,_docsOk:docsOk};
    })
    .filter(c=>c._llegaraATiempo)
    .sort((a,b)=>{
      if(a._enIsla!==b._enIsla)return a._enIsla?-1:1; // priorizar las que ya están en la isla (cero espera)
      if(a._enPuertoCorrecto!==b._enPuertoCorrecto)return a._enPuertoCorrecto?-1:1;
      if(a._docsOk!==b._docsOk)return a._docsOk?-1:1;
      return String(a._disponible).localeCompare(String(b._disponible)); // FIFO: la que antes esté lista
    });
}
 
// Candidatas para el SELECTOR MANUAL del formulario de pedido: aquí el único criterio de
// exclusión es que la cuba esté asignada a OTRO servicio (asignadoServicioId). No filtramos por
// disponibilidad (localización/llegada de barco), por tener un GA de un movimiento anterior, por
// aptitud de producto ni por si llegaría a tiempo — esas condiciones solo se muestran como aviso
// (⚠) en el propio option, para que Roberto pueda decidir y forzar la asignación si hace falta.
// getCisternaCandidates() se mantiene tal cual para el auto-asignado automático, que sí debe ser estricto.
function getCisternaSelectorOptions(producto,fechaEntregaISO,currentCuba){
  const puerto=getPortForProduct(producto);
  const fechaLimite=fechaEntregaISO?fechaLimiteCarga(fechaEntregaISO):null;
  return cisternaStock
    .filter(c=>c.estadoManual!=='fuera_servicio'||cisternaKey(c.cuba)===cisternaKey(currentCuba))
    .filter(c=>!cisternaAsignadaVigente(c)||cisternaKey(c.cuba)===cisternaKey(currentCuba))
    .map(c=>{
      const disponible=cisternaFechaDisponible(c);
      const enIsla=cisternaEnIsla(c);
      // Ya en la isla: no depende del puerto ni del tránsito de barco para este reparto.
      const enPuertoCorrecto=enIsla||(isPortCode(c.localizacion)&&c.localizacion.toUpperCase()===puerto);
      const llegaraATiempo=enIsla||!fechaLimite||!disponible?true:disponible<=fechaLimite;
      const docsOk=fechaEntregaISO?docsValidosHasta(c,fechaEntregaISO):true;
      const apta=cisternaAptaParaProducto(c,producto);
      return {...c,_puerto:puerto,_fechaLimite:fechaLimite,_disponible:disponible,_enIsla:enIsla,_enPuertoCorrecto:enPuertoCorrecto,_llegaraATiempo:llegaraATiempo,_docsOk:docsOk,_apta:apta,_actual:cisternaKey(c.cuba)===cisternaKey(currentCuba)};
    })
    .sort((a,b)=>{
      if(a._actual!==b._actual)return a._actual?-1:1;
      if(a._enIsla!==b._enIsla)return a._enIsla?-1:1;
      const scoreA=(a._apta?0:1)+(a._enPuertoCorrecto?0:1)+(a._llegaraATiempo?0:1)+(a._docsOk?0:1);
      const scoreB=(b._apta?0:1)+(b._enPuertoCorrecto?0:1)+(b._llegaraATiempo?0:1)+(b._docsOk?0:1);
      if(scoreA!==scoreB)return scoreA-scoreB;
      return String(a._disponible||'').localeCompare(String(b._disponible||''));
    });
}
 
function assignCisternaToService(serviceId,cuba){
  const s=services.find(x=>x.id===serviceId);
  if(!s)return false;
  if(s.cisternaCuba)releaseCisternaFromService(s.id,false);
  const c=cisternaStock.find(x=>cisternaKey(x.cuba)===cisternaKey(cuba));
  if(!c)return false;
  s.cisternaCuba=c.cuba;
  s.cisternaPlataforma=c.plataforma;
  c.asignadoServicioId=s.id;
  c.fechaEntregaAsignada=s.day;
  c.clienteAsignado=s.client;
  c.choferAsignado=DRIVER_BY_ID.get(s.driver)?.name||'';
  return true;
}
function releaseCisternaFromService(serviceId,doSave=true){
  const s=services.find(x=>x.id===serviceId);
  if(!s||!s.cisternaCuba)return;
  const c=cisternaStock.find(x=>cisternaKey(x.cuba)===cisternaKey(s.cisternaCuba));
  if(c&&c.asignadoServicioId===s.id){
    c.asignadoServicioId=null;c.fechaEntregaAsignada=null;c.clienteAsignado=null;c.choferAsignado=null;
  }
  s.cisternaCuba=null;s.cisternaPlataforma=null;
  if(doSave){
    writeCisternaStock();
    saveData('cisterna_liberada',{summary:`Cuba liberada de ${s.pedidoNum?'#'+s.pedidoNum:(s.client||'pedido')}`,day:s.day});
    renderCalendar();
    renderCisternaFleetIfOpen();
  }
}
 
// Limpia (persistiendo) las asignaciones de cubas que hayan quedado "colgadas" de un pedido de
// un día ya pasado, para que la ficha de la cuba (estado, cliente/fecha asignados) no muestre
// datos obsoletos aunque nadie haya pulsado "Liberar" a mano. No toca el pedido histórico, que
// conserva su cisternaCuba como registro de qué cuba se usó aquel día.
function releaseStaleCisternaAssignments(){
  const todayKey=localDateKey(new Date());
  let liberadas=0;
  cisternaStock.forEach(c=>{
    if(c.asignadoServicioId&&c.fechaEntregaAsignada&&c.fechaEntregaAsignada<todayKey){
      c.asignadoServicioId=null;c.fechaEntregaAsignada=null;c.clienteAsignado=null;c.choferAsignado=null;
      liberadas++;
    }
  });
  return liberadas;
}
 
function autoAssignCisternas(){
  const days=[-1,0,1,2,3].flatMap(getDaysForWeek).filter(d=>!isWeekend(d));
  const todayKey=localDateKey(new Date());
  const dayKeys=new Set(days.map(dateKey));
  const pending=services
    .filter(s=>s.type==='cisterna'&&!s.cisternaCuba&&s.day>=todayKey&&dayKeys.has(s.day))
    .sort((a,b)=>a.day.localeCompare(b.day));
  let asignadas=0;
  pending.forEach(s=>{
    const producto=(s.articles&&s.articles[0]&&getArticleName(s.articles[0].artCod))||s.client||'';
    const best=getCisternaCandidates(producto,s.day)[0];
    if(best){assignCisternaToService(s.id,best.cuba);asignadas++;}
  });
  return asignadas;
}
 
async function readCisternaStock(){
  if(window.planificadorServerStorage){
    const stored=await storageAdapter.get(CISTERNA_STOCK_STORAGE_KEY);
    if(stored&&stored.value)return JSON.parse(stored.value);
  }
  const db=await openDatabase();
  if(db){
    const current=await idbTx(db,CURRENT_STORE,'readonly',store=>idbRequest(store.get(CISTERNA_STOCK_STORAGE_KEY)));
    if(current&&Array.isArray(current.stock)){
      if(window.planificadorServerStorage)try{await storageAdapter.set(CISTERNA_STOCK_STORAGE_KEY,JSON.stringify(current.stock));}catch(error){window.planificadorStorageError=error?.message||'No se pudo migrar el stock al servidor.';}
      return current.stock;
    }
  }
  const r=await storageAdapter.get(CISTERNA_STOCK_STORAGE_KEY);
  if(r&&r.value)return JSON.parse(r.value);
  return [];
}
async function writeCisternaStock(){
  const value=JSON.stringify(cisternaStock);
  let remoteError=null;
  if(window.planificadorServerStorage){
    try{await storageAdapter.set(CISTERNA_STOCK_STORAGE_KEY,value);}catch(error){remoteError=error;}
  }
  const db=await openDatabase();
  if(db){
    await idbTx(db,CURRENT_STORE,'readwrite',store=>{
      store.put({key:CISTERNA_STOCK_STORAGE_KEY,stock:JSON.parse(value),updatedAt:new Date().toISOString()});
    });
  }
  if(remoteError)throw remoteError;
  if(!window.planificadorServerStorage)await storageAdapter.set(CISTERNA_STOCK_STORAGE_KEY,value);
}
 
// ---- IMPORTACION DE FLOTA (hoja "Situación actual cubas") ----
function parseSituacionCubasSheet(ws){
  const aoa=XLSX.utils.sheet_to_json(ws,{header:1,defval:null,raw:true});
  const records=[];
  let i=0;
  while(i<aoa.length){
    const label=String(aoa[i]?.[0]||'').trim().toUpperCase();
    if(label==='CUBA'){
      const block={};
      let j=i;
      while(j<aoa.length&&String(aoa[j]?.[0]||'').trim()!==''){
        const rowLabel=String(aoa[j][0]).trim().toUpperCase();
        if(!block[rowLabel])block[rowLabel]=[];
        block[rowLabel].push(aoa[j].slice(1));
        j++;
      }
      const cubaRow=block['CUBA']?.[0]||[];
      cubaRow.forEach((cuba,c)=>{
        if(!cuba||!String(cuba).trim())return;
        records.push({
          cuba:String(cuba).trim(),
          plataforma:String((block['PLATAFORMA']?.[0]||[])[c]||'').trim(),
          localizacion:String((block['LOCALIZACIÓN ACTUAL']?.[0]||[])[c]||'').trim().toUpperCase(),
          trayecto:String((block['TRAYECTO']?.[0]||[])[c]||'').trim(),
          producto:String((block['PRODUCTO']?.[0]||[])[c]||'').trim(),
          retorno:String((block['RETORNO']?.[0]||[])[c]||'').trim(),
          fechaDisponible:valueDateToISO((block['SALIDA']?.[0]||[])[c])||localDateKey(new Date()),
          llegadaBarco:valueDateToISO((block['LLEGADA']?.[1]||[])[c])||null,
          ga:String((block['GA']?.[0]||[])[c]||'').trim()
        });
      });
      i=j+1;
    }else i++;
  }
  return records;
}
function mergeCisternaStock(imported){
  dedupeCisternaStock();
  let nuevos=0,actualizados=0;
  imported.forEach(rec=>{
    const existing=cisternaStock.find(c=>cisternaKey(c.cuba)===cisternaKey(rec.cuba));
    if(existing){
      existing.plataforma=rec.plataforma||existing.plataforma;
      existing.localizacion=rec.localizacion||existing.localizacion;
      existing.trayecto=rec.trayecto||existing.trayecto;
      existing.producto=rec.producto||existing.producto;
      existing.retorno=rec.retorno||existing.retorno;
      existing.fechaDisponible=rec.fechaDisponible||existing.fechaDisponible;
      existing.llegadaBarco=rec.llegadaBarco||existing.llegadaBarco;
      existing.ga=rec.ga||existing.ga;
      actualizados++;
    }else{
      cisternaStock.push({
        ...rec,
        productosAptos:rec.producto?[rec.producto]:[],
        estadoManual:null,
        asignadoServicioId:null,
        fechaEntregaAsignada:null,
        clienteAsignado:null,
        choferAsignado:null,
        itvCaduca:null,
        adrCaduca:null,
        imdgCaduca:null
      });
      nuevos++;
    }
  });
  return {nuevos,actualizados};
}
function dedupeCisternaStock(){
  const seen=new Map();
  cisternaStock.forEach(c=>{
    const k=cisternaKey(c.cuba);
    if(!k)return;
    if(!seen.has(k)){seen.set(k,c);}
    else{
      const kept=seen.get(k);
      ['asignadoServicioId','fechaEntregaAsignada','clienteAsignado','choferAsignado','itvCaduca','adrCaduca','imdgCaduca','estadoManual','plataforma','localizacion','trayecto','producto','retorno','fechaDisponible','llegadaBarco','ga'].forEach(f=>{
        if((kept[f]===null||kept[f]===undefined||kept[f]==='')&&c[f])kept[f]=c[f];
      });
      if((!kept.productosAptos||!kept.productosAptos.length)&&c.productosAptos&&c.productosAptos.length)kept.productosAptos=c.productosAptos;
    }
  });
  const deduped=Array.from(seen.values());
  const removed=cisternaStock.length-deduped.length;
  cisternaStock=deduped;
  return removed;
}
function handleImportFlota(workbook){
  const sheetName=workbook.SheetNames.find(n=>normalizeHeader(n).includes('situacion')&&normalizeHeader(n).includes('cuba'));
  if(!sheetName)return null;
  const records=parseSituacionCubasSheet(workbook.Sheets[sheetName]);
  if(!records.length)return null;
  const result=mergeCisternaStock(records);
  writeCisternaStock();
  return result;
}
 
// ---- REGISTRO DE MOVIMIENTOS / ARRASTRES (hoja "Registro movimientos") ----
// La hoja es una tabla plana (una fila = un GA/movimiento de cuba). Columnas relevantes:
//  CUBA, PLATAFORMA, ORDEN(=GA), PRODUCTO, TRAYECTO, CARGA, BARCO, LLEGADA, HORA, DESTINO,
//  OBSER., RETORNO, CHÓFER (¡ojo: cargos extra en €/ADR, NO nombre de conductor!),
//  dos columnas sin cabecera tras CHÓFER = ARRASTRE 1 (chófer+fecha) y ARRASTRE 2 (chófer+fecha),
//  MES, DÍA DESCARGA, DESCARGADO EN, OPERARIO, AYUDANTE, 2ª DESCARGA, OPERARIO, AYUDANTE.
const DRIVER_NAMES_UPPER=new Set(DRIVERS.map(d=>normalizeStr(d.name)));
 
// Códigos tipo "M-30-12" o "X-08-01 CBAR" (letra día semana - día - mes [- destino]).
function parseDayMonthCode(text){
  const raw=String(text||'').trim();
  const m=raw.match(/^[A-Za-zÀ-ÿ]?\.?\s*-?\s*(\d{1,2})\s*[-\/]\s*(\d{1,2})(.*)$/);
  if(!m)return null;
  const day=Number(m[1]),month=Number(m[2]);
  if(day<1||day>31||month<1||month>12)return null;
  return {day,month,rest:(m[3]||'').trim()};
}
// Resuelve un código día-mes (sin año) al ISO más plausible, usando una fecha ancla cercana.
function resolveDateCodeToISO(text,anchorDate){
  const parsed=parseDayMonthCode(text);
  if(!parsed)return null;
  const anchor=anchorDate instanceof Date&&!isNaN(anchorDate)?anchorDate:new Date();
  let best=null,bestDiff=Infinity;
  [-1,0,1].forEach(dy=>{
    const y=anchor.getFullYear()+dy;
    const cand=new Date(Date.UTC(y,parsed.month-1,parsed.day));
    if(isNaN(cand))return;
    const diff=Math.abs(cand.getTime()-Date.UTC(anchor.getFullYear(),anchor.getMonth(),anchor.getDate()));
    if(diff<bestDiff){bestDiff=diff;best=cand;}
  });
  return best?localDateKey(best):null;
}
// Resuelve cualquier valor de celda de fecha: Date real, serial numérico de Excel (raw:true no los
// convierte a Date), texto ISO/D-M-A, o código "L-30-12" sin año (se resuelve contra una fecha ancla
// de la misma fila). OJO: no usamos el `new Date(string)` genérico de JS como último recurso — es
// demasiado permisivo y interpreta mal códigos como "V-02-01" (los toma como año 2001).
function resolveFieldDateISO(raw,anchorDate){
  if(raw===null||raw===undefined||raw==='')return null;
  if(raw instanceof Date&&!isNaN(raw))return localDateKey(raw);
  if(typeof raw==='number'&&isFinite(raw))return excelDateToISO(raw);
  if(typeof raw==='string'){
    const trimmed=raw.trim();
    const iso=trimmed.match(/^(\d{4})-(\d{1,2})-(\d{1,2})/);
    if(iso)return `${iso[1]}-${pad2(iso[2])}-${pad2(iso[3])}`;
    const dmy=trimmed.match(/^(\d{1,2})[\/.](\d{1,2})[\/.](\d{2,4})$/); // solo separador / o . con año explícito
    if(dmy){
      const year=Number(dmy[3].length===2?'20'+dmy[3]:dmy[3]);
      return `${year}-${pad2(dmy[2])}-${pad2(dmy[1])}`;
    }
    return resolveDateCodeToISO(trimmed,anchorDate); // código "L-30-12" tipo día semana-día-mes, sin año
  }
  return null;
}
// Extrae la mejor fecha real disponible (como objeto Date) entre varios valores de una fila, para
// usar como ancla de año al resolver códigos "L-30-12" sin año. Solo cuentan valores autosuficientes
// (Date real, serial de Excel o texto ISO) — nunca un código día-semana que a su vez necesitaría ancla.
function pickAnchorDate(...values){
  for(const v of values){
    let iso=null;
    if(v instanceof Date&&!isNaN(v))iso=localDateKey(v);
    else if(typeof v==='number'&&isFinite(v))iso=excelDateToISO(v);
    else if(typeof v==='string'&&/^\d{4}-\d{1,2}-\d{1,2}/.test(v.trim()))iso=resolveFieldDateISO(v,null);
    if(iso)return new Date(iso+'T00:00:00');
  }
  return null;
}
 
function parseRegistroMovimientosSheet(ws,fallbackAnchor){
  const aoa=XLSX.utils.sheet_to_json(ws,{header:1,defval:null,raw:true});
  const headerIdx=aoa.findIndex(row=>row.some(c=>String(c||'').trim().toUpperCase()==='CUBA'));
  if(headerIdx<0)return [];
  let runningAnchor=fallbackAnchor instanceof Date&&!isNaN(fallbackAnchor)?fallbackAnchor:new Date();
  const records=[];
  for(let r=headerIdx+1;r<aoa.length;r++){
    const row=aoa[r];
    const cuba=row[0];
    if(!cuba||!String(cuba).trim())continue;
    const ga=row[2];
    const diaDescargaRaw=row[19];  // col T
    const arr1FechaRaw=row[15];    // col P
    const arr2FechaRaw=row[17];    // col R
    // Ancla de año: preferimos una fecha real de esta misma fila; si no hay, usamos la última conocida.
    const anchor=pickAnchorDate(diaDescargaRaw,arr1FechaRaw,arr2FechaRaw)||runningAnchor;
    const cargaRaw=row[6],llegadaRaw=row[8],retornoRaw=row[12]; // G, I, M
    const cargaISO=resolveFieldDateISO(cargaRaw,anchor);
    const llegadaISO=resolveFieldDateISO(llegadaRaw,anchor);
    const retornoParsed=retornoRaw!=null?String(retornoRaw).trim():'';
    const retornoISO=retornoParsed?resolveFieldDateISO(retornoRaw,anchor):null;
    const retornoDestMatch=retornoParsed.match(/^[A-Za-zÀ-ÿ]?\.?\s*-?\s*\d{1,2}\s*[-\/]\s*\d{1,2}\s*(.*)$/);
    const retornoDestino=retornoDestMatch?retornoDestMatch[1].trim():'';
    const diaDescargaISO=resolveFieldDateISO(diaDescargaRaw,anchor);
    const arrastre1FechaISO=resolveFieldDateISO(arr1FechaRaw,anchor);
    const arrastre2FechaISO=resolveFieldDateISO(arr2FechaRaw,anchor);
    // Fecha más fiable de esta fila, para propagar el ancla de año a las filas siguientes sin fecha real.
    const bestRowDate=diaDescargaISO||llegadaISO||cargaISO||arrastre1FechaISO||arrastre2FechaISO;
    if(bestRowDate)runningAnchor=new Date(bestRowDate+'T00:00:00');
    const chofer1Raw=row[13]!=null?String(row[13]).trim():''; // col N = "CHÓFER" (cargos extra, no nombre)
    const isKnownDriver=name=>!!name&&DRIVER_NAMES_UPPER.has(normalizeStr(name));
    const isChofer1Driver=isKnownDriver(chofer1Raw);
    let arrastre1ChoferRaw=row[14]!=null?String(row[14]).trim():''; // col O
    let arrastre2ChoferRaw=row[16]!=null?String(row[16]).trim():''; // col Q
    const strayNotes=[];
    if(arrastre1ChoferRaw&&!isKnownDriver(arrastre1ChoferRaw)){strayNotes.push(arrastre1ChoferRaw);arrastre1ChoferRaw='';}
    if(arrastre2ChoferRaw&&!isKnownDriver(arrastre2ChoferRaw)){strayNotes.push(arrastre2ChoferRaw);arrastre2ChoferRaw='';}
    const record={
      ga:ga?String(ga).trim():'',
      refId:row[3]!=null?String(row[3]).trim():'',
      cuba:String(cuba).trim(),
      plataforma:row[1]!=null?String(row[1]).trim():'',
      producto:row[4]!=null?String(row[4]).trim():'',
      trayecto:row[5]!=null?String(row[5]).trim():'',
      cargaFecha:cargaISO,
      barco:row[7]!=null?String(row[7]).trim():'',
      llegadaFecha:llegadaISO,
      hora:row[9]!=null?String(row[9]).trim():'',
      destino:row[10]!=null?String(row[10]).trim():'',
      observaciones:row[11]!=null?String(row[11]).trim():'',
      retornoFecha:retornoISO,
      retornoDestino:retornoDestino,
      // OJO: la columna "CHÓFER" del Excel casi siempre trae cargos extra (€, ADR), no un nombre.
      // Solo la tratamos como conductor si coincide literalmente con un nombre de DRIVERS.
      choferColRaw:chofer1Raw,
      cargoExtra:Array.from(new Set([chofer1Raw&&!isChofer1Driver?chofer1Raw:'',...strayNotes].filter(Boolean))).join(' · '),
      arrastre1Chofer:arrastre1ChoferRaw,
      arrastre1Fecha:arrastre1FechaISO,
      arrastre2Chofer:arrastre2ChoferRaw,
      arrastre2Fecha:arrastre2FechaISO,
      diaDescargaFecha:diaDescargaISO,
      descargadoEn:row[20]!=null?String(row[20]).trim():'',
      operario:row[21]!=null?String(row[21]).trim():'',
      ayudante:row[22]!=null?String(row[22]).trim():'',
      segundaDescarga:row[24]!=null?String(row[24]).trim():'',
      operario2:row[25]!=null?String(row[25]).trim():'',
      ayudante2:row[26]!=null?String(row[26]).trim():''
    };
    // Solo contabilizamos el registro de arrastres a partir de REGISTRO_MOV_CUTOFF_DATE.
    if(isRegistroMovCounted(record))records.push(record);
  }
  return records;
}
// Fecha más representativa de un movimiento, para decidir si cae antes/después del corte de contabilización.
function registroMovRelevantDate(rec){
  return rec.diaDescargaFecha||rec.llegadaFecha||rec.cargaFecha||rec.arrastre1Fecha||rec.arrastre2Fecha||rec.retornoFecha||'';
}
// Solo contabilizamos movimientos con fecha conocida a partir de REGISTRO_MOV_CUTOFF_DATE.
// Movimientos sin ninguna fecha reconocible se descartan también (no se pueden verificar como posteriores al corte).
function isRegistroMovCounted(rec){
  const d=registroMovRelevantDate(rec);
  return !!d&&d>=REGISTRO_MOV_CUTOFF_DATE;
}
// Elimina del registro ya cargado en memoria cualquier movimiento anterior al corte (p.ej. datos importados
// antes de establecerse esta regla). Devuelve cuántos se han eliminado.
function purgeRegistroMovBeforeCutoff(){
  const before=registroMovimientos.length;
  registroMovimientos=registroMovimientos.filter(isRegistroMovCounted);
  return before-registroMovimientos.length;
}
function registroMovKey(rec){return `${rec.ga||''}__${cisternaKey(rec.cuba)}`;}
 
// ---- SUGERENCIA DE CONDUCTOR/CLIENTE DESDE EL REGISTRO DE ARRASTRES (por Nº pedido = GA) ----
// Busca el movimiento importado del Excel cuyo GA coincide (comparación exacta, sin distinguir mayúsculas).
function getRegistroMovForGA(ga){
  const key=String(ga||'').trim().toUpperCase();
  if(!key)return null;
  return registroMovimientos.find(r=>String(r.ga||'').trim().toUpperCase()===key)||null;
}
// Comparación flexible de nombres de cliente: el Excel suele traer una forma abreviada
// (p.ej. "PEDROSA") de un nombre más completo ya usado en el planificador (p.ej. "PEDROSA VENY").
function clientNameFuzzyMatch(a,b){
  const na=normalizeStr(a),nb=normalizeStr(b);
  if(!na||!nb)return false;
  if(na===nb)return true;
  return na.includes(nb)||nb.includes(na);
}
// Busca entre los clientes ya usados en el planificador el nombre más completo que case con el
// valor (posiblemente abreviado) que trae el Excel; si no encuentra nada mejor, deja el original.
function expandClientName(raw){
  if(!raw)return raw;
  const candidatos=Array.from(new Set(services.map(s=>s.client).filter(Boolean)))
    .filter(c=>clientNameFuzzyMatch(c,raw));
  if(!candidatos.length)return raw;
  candidatos.sort((a,b)=>b.length-a.length);
  return candidatos[0];
}
// Al introducir/perder el foco el Nº de pedido (GA) en un servicio de cisterna nuevo, si ese GA ya
// tiene chófer/cliente registrados en el Excel importado, se sugieren (y rellenan) esos datos.
function onPedidoBlur(){
  if(isEditing)return; // no pisar datos de un servicio ya existente
  const type=document.getElementById('f-type').value;
  if(type!=='cisterna')return;
  const pedido=document.getElementById('f-pedido').value.trim();
  if(!pedido)return;
  const rec=getRegistroMovForGA(pedido);
  if(!rec)return;
  const choferName=rec.arrastre2Chofer||rec.arrastre1Chofer||'';
  const driverMatch=choferName?DRIVERS.find(d=>normalizeStr(d.name)===normalizeStr(choferName)):null;
  const clienteRaw=rec.descargadoEn||rec.destino||'';
  const clienteSugerido=clienteRaw?expandClientName(clienteRaw):'';
  const applied=[];
  if(driverMatch){
    document.getElementById('f-driver').value=driverMatch.id;
    onDriverOrDayChange();
    applied.push(`conductor <strong>${escapeHtml(driverMatch.name)}</strong>`);
  }
  const clientEl=document.getElementById('f-client');
  if(clienteSugerido&&!clientEl.value.trim()){
    clientEl.value=clienteSugerido;
    applied.push(`cliente <strong>${escapeHtml(clienteSugerido)}</strong>`);
  }
  if(applied.length){
    const box=document.getElementById('suggestion-box');
    box.innerHTML=`<i class="ti ti-bulb" aria-hidden="true" style="font-size:13px;vertical-align:-1px;margin-right:6px;"></i>Detectado en el registro de arrastres para <strong>GA ${escapeHtml(pedido)}</strong>: se ha rellenado ${applied.join(' y ')}. Revisa que sea correcto.`;
    box.style.display='block';
  }
}
// Cubas ya asignadas a un pedido (GA) en el planificador que todavía no aparecen en el Excel de
// Registro movimientos importado — para poder trasladarlas manualmente al Excel maestro.
function getPendingCisternaAssignments(){
  return services
    .filter(s=>s.type==='cisterna'&&s.cisternaCuba&&s.pedidoNum)
    .filter(s=>!getRegistroMovForGA(s.pedidoNum))
    .map(s=>({ga:s.pedidoNum,cuba:s.cisternaCuba,chofer:DRIVER_BY_ID.get(s.driver)?.name||'',cliente:s.client||'',fecha:s.day}));
}
function mergeRegistroMovimientos(imported){
  const existingKeys=new Map(registroMovimientos.map(r=>[registroMovKey(r),r]));
  let nuevos=0,actualizados=0;
  imported.forEach(rec=>{
    const key=registroMovKey(rec);
    if(existingKeys.has(key)){
      Object.assign(existingKeys.get(key),rec);
      actualizados++;
    }else{
      registroMovimientos.push(rec);
      existingKeys.set(key,rec);
      nuevos++;
    }
  });
  registroMovimientos.sort((a,b)=>String(a.ga||'').localeCompare(String(b.ga||''),undefined,{numeric:true}));
  return {nuevos,actualizados};
}
async function readRegistroMovimientos(){
  if(window.planificadorServerStorage){
    const stored=await storageAdapter.get(REGISTRO_MOV_STORAGE_KEY);
    if(stored&&stored.value)return JSON.parse(stored.value);
  }
  const db=await openDatabase();
  if(db){
    const current=await idbTx(db,CURRENT_STORE,'readonly',store=>idbRequest(store.get(REGISTRO_MOV_STORAGE_KEY)));
    if(current&&Array.isArray(current.registro)){
      if(window.planificadorServerStorage)try{await storageAdapter.set(REGISTRO_MOV_STORAGE_KEY,JSON.stringify(current.registro));}catch(error){window.planificadorStorageError=error?.message||'No se pudo migrar el registro al servidor.';}
      return current.registro;
    }
  }
  const r=await storageAdapter.get(REGISTRO_MOV_STORAGE_KEY);
  if(r&&r.value)return JSON.parse(r.value);
  return [];
}
async function writeRegistroMovimientos(){
  const value=JSON.stringify(registroMovimientos);
  let remoteError=null;
  if(window.planificadorServerStorage){
    try{await storageAdapter.set(REGISTRO_MOV_STORAGE_KEY,value);}catch(error){remoteError=error;}
  }
  const db=await openDatabase();
  if(db){
    await idbTx(db,CURRENT_STORE,'readwrite',store=>{
      store.put({key:REGISTRO_MOV_STORAGE_KEY,registro:JSON.parse(value),updatedAt:new Date().toISOString()});
    });
  }
  if(remoteError)throw remoteError;
  if(!window.planificadorServerStorage)await storageAdapter.set(REGISTRO_MOV_STORAGE_KEY,value);
}
function handleImportRegistroMovimientos(workbook){
  const sheetName=workbook.SheetNames.find(n=>normalizeHeader(n).includes('registro')&&normalizeHeader(n).includes('movimiento'));
  if(!sheetName)return null;
  const records=parseRegistroMovimientosSheet(workbook.Sheets[sheetName]);
  if(!records.length)return null;
  const result=mergeRegistroMovimientos(records);
  writeRegistroMovimientos();
  return result;
}
 
// ---- UI: SELECTOR DE CUBA EN EL FORMULARIO ----
function refreshCisternaSelect(){
  const row=document.getElementById('row-cisterna-asig');
  if(!row)return;
  const type=document.getElementById('f-type').value;
  if(type!=='cisterna'){row.style.display='none';return;}
  row.style.display='';
  if(releaseStaleCisternaAssignments())writeCisternaStock();
  const day=document.getElementById('f-day').value;
  const productos=readProductos();
  const producto=productos.length?getArticleName(productos[0].artCod):'';
  const sel=document.getElementById('f-cisterna-cuba');
  const currentCuba=isEditing?services.find(s=>s.id===editingId)?.cisternaCuba:null;
  sel.innerHTML='<option value="">— sin asignar —</option>';
  document.getElementById('cisterna-asig-info').textContent='';
  // Selector permisivo: se listan TODAS las cubas no asignadas a otro servicio (aunque tengan
  // un GA de un movimiento anterior, no estén "disponibles" según el flujo automático, o no
  // encajen del todo por producto/puerto/tiempo — eso solo se marca como aviso, no bloquea).
  const candidates=getCisternaSelectorOptions(producto,day,currentCuba);
  candidates.forEach(c=>{
    const o=document.createElement('option');
    o.value=c.cuba;
    const flags=[];
    if(c._actual)flags.push('actual');
    if(c._enIsla&&!c._actual){
      const loc=cisternaMallorcaLocation(c);
      flags.push(loc==='puerto'?'en Puerto de Palma':loc==='base'?'en base':'en Mallorca');
    }
    if(!c._apta)flags.push('⚠ producto distinto');
    if(!c._enPuertoCorrecto)flags.push(c._disponible?`llega ${c._disponible}`:'fuera de puerto');
    if(day&&!c._llegaraATiempo)flags.push('⚠ no llega a tiempo');
    if(day&&!c._docsOk)flags.push('⚠ docs');
    if(c.ga)flags.push(`GA ${c.ga}`);
    o.textContent=`${c.cuba} (${c.plataforma||'sin plataforma'})${flags.length?' — '+flags.join(', '):''}`;
    sel.appendChild(o);
  });
  sel.value=currentCuba||'';
  onCisternaCubaChange();
}
function onCisternaCubaChange(){
  const cuba=document.getElementById('f-cisterna-cuba').value;
  const info=document.getElementById('cisterna-asig-info');
  const productos=readProductos();
  const producto=productos.length?getArticleName(productos[0].artCod):'';
  const day=document.getElementById('f-day').value;
  const puerto=getPortForProduct(producto);
  const parts=[`Puerto de carga: ${PORT_LABELS[puerto]||puerto}`];
  if(day)parts.push(`carga antes del ${fechaLimiteCarga(day)} (barco: +${BOAT_TRANSIT_DAYS} día)`);
  if(cuba){
    const c=cisternaStock.find(x=>cisternaKey(x.cuba)===cisternaKey(cuba));
    if(c&&day&&!docsValidosHasta(c,day))parts.push('⚠ ITV/ADR/IMDG caduca antes de la entrega');
  }
  info.textContent=parts.join(' · ');
}
// El origen/destino del trayecto (Base o Puerto) se lee directamente al guardar/renderizar
// (afecta a calcSalidaDuration y a los badges del calendario); no hace falta recalcular nada aquí.
function onTrayectoCisternaChange(){}
 
// ---- MODAL: FLOTA DE CISTERNAS ----
// Último movimiento conocido de una cuba, cruzando con el registro de arrastres importado.
function getLastMovimientoForCuba(cuba){
  const key=cisternaKey(cuba);
  let best=null,bestDate='';
  registroMovimientos.forEach(r=>{
    if(cisternaKey(r.cuba)!==key)return;
    const candidates=[
      {date:r.retornoFecha,label:`Retorno${r.retornoDestino?' '+r.retornoDestino:''}`},
      {date:r.diaDescargaFecha,label:`Descarga${r.descargadoEn?' en '+r.descargadoEn:''}`},
      {date:r.arrastre2Fecha,label:`Arrastre${r.arrastre2Chofer?' ('+r.arrastre2Chofer+')':''}`},
      {date:r.arrastre1Fecha,label:`Arrastre${r.arrastre1Chofer?' ('+r.arrastre1Chofer+')':''}`},
      {date:r.llegadaFecha,label:'Llegada barco'},
      {date:r.cargaFecha,label:'Carga'}
    ];
    candidates.forEach(c=>{if(c.date&&c.date>bestDate){bestDate=c.date;best={ga:r.ga,date:c.date,label:c.label};}});
  });
  return best;
}
function estadoCisterna(c){
  if(c.estadoManual==='fuera_servicio')return 'fuera de servicio';
  if(cisternaAsignadaVigente(c))return 'asignada';
  if(isPortCode(c.localizacion))return 'disponible';
  const loc=cisternaMallorcaLocation(c);
  if(loc==='puerto')return 'en Puerto de Palma';
  if(loc==='base')return 'en base';
  return 'en isla / tránsito';
}
function openCisternaFleet(){renderCisternaFleet();document.getElementById('cisterna-fleet-modal').classList.add('open');}
function closeCisternaFleet(){document.getElementById('cisterna-fleet-modal').classList.remove('open');}
function renderCisternaFleetIfOpen(){if(document.getElementById('cisterna-fleet-modal')?.classList.contains('open'))renderCisternaFleet();}
function renderCisternaFleet(){
  dedupeCisternaStock();
  if(releaseStaleCisternaAssignments())writeCisternaStock();
  const todayKey=localDateKey(new Date());
  const disponibles=cisternaStock.filter(c=>cisternaEstaDisponible(c)).length;
  const asignadas=cisternaStock.filter(c=>c.asignadoServicioId).length;
  const transito=cisternaStock.length-disponibles-asignadas;
  document.getElementById('fleet-summary').innerHTML=
    `<strong>${cisternaStock.length}</strong> cubas registradas · <span style="color:#27500A;">${disponibles} disponibles</span> · <span style="color:#0C447C;">${asignadas} asignadas</span> · <span style="color:#633806;">${transito} en tránsito/isla</span>`;
  const rows=cisternaStock.slice().sort((a,b)=>a.cuba.localeCompare(b.cuba)).map(c=>{
    const docWarn=['itvCaduca','adrCaduca','imdgCaduca'].some(f=>c[f]&&c[f]<todayKey);
    const aptos=(c.productosAptos||[]).join(', ');
    const lastMov=getLastMovimientoForCuba(c.cuba);
    const lastMovCell=lastMov?`${lastMov.date} · ${escapeHtml(lastMov.label)} <span style="opacity:0.55;">(${escapeHtml(lastMov.ga)})</span>`:'<span style="opacity:0.5;">—</span>';
    return `<tr${docWarn?' style="background:#FCEBEB;"':''}>
      <td>${escapeHtml(c.cuba)}</td>
      <td>${escapeHtml(c.plataforma||'—')}</td>
      <td>${escapeHtml(cisternaLocationLabel(c))}</td>
      <td>${estadoCisterna(c)}${c.asignadoServicioId?` <span style="opacity:0.6;">(${c.fechaEntregaAsignada||''} ${escapeHtml(c.clienteAsignado||'')})</span>`:''}</td>
      <td style="font-size:11px;">${lastMovCell}</td>
      <td><input type="text" value="${escapeHtml(aptos)}" style="width:150px;font-size:11px;" onchange="updateCisternaField('${c.cuba.replace(/'/g,"\\'")}','productosAptos',this.value.split(',').map(s=>s.trim()).filter(Boolean))"></td>
      <td><input type="date" value="${c.itvCaduca||''}" style="font-size:11px;" onchange="updateCisternaField('${c.cuba.replace(/'/g,"\\'")}','itvCaduca',this.value)"></td>
      <td><input type="date" value="${c.adrCaduca||''}" style="font-size:11px;" onchange="updateCisternaField('${c.cuba.replace(/'/g,"\\'")}','adrCaduca',this.value)"></td>
      <td><input type="date" value="${c.imdgCaduca||''}" style="font-size:11px;" onchange="updateCisternaField('${c.cuba.replace(/'/g,"\\'")}','imdgCaduca',this.value)"></td>
      <td>${c.asignadoServicioId?`<button class="btn-secondary" style="padding:2px 6px;font-size:11px;" onclick="releaseCisternaFromService(${JSON.stringify(c.asignadoServicioId)});renderCisternaFleet();">Liberar</button>`:''}</td>
    </tr>`;
  }).join('');
  document.getElementById('fleet-tbody').innerHTML=rows||'<tr><td colspan="10">Sin cubas importadas. Usa "Importar Excel" con el archivo de movimiento de cubas (hoja "Situación actual cubas").</td></tr>';
  const portRows=Object.keys(productPort).sort().map(p=>`<tr><td>${escapeHtml(p)}</td><td>
    <select onchange="setPortForProduct('${p.replace(/'/g,"\\'")}',this.value);renderCisternaFleet();">
      <option value="BCN"${productPort[p]==='BCN'?' selected':''}>Barcelona</option>
      <option value="VLC"${productPort[p]==='VLC'?' selected':''}>Valencia</option>
    </select></td></tr>`).join('');
  document.getElementById('port-tbody').innerHTML=portRows||'<tr><td colspan="2" style="color:var(--color-text-tertiary);">Añade productos abajo para fijar su puerto de carga (por defecto: Barcelona).</td></tr>';
}
function updateCisternaField(cuba,field,value){
  const c=cisternaStock.find(x=>cisternaKey(x.cuba)===cisternaKey(cuba));
  if(!c)return;
  c[field]=value;
  writeCisternaStock();
}
function addProductPortRow(){
  const input=document.getElementById('new-product-port');
  const name=input.value.trim();
  if(!name)return;
  setPortForProduct(name,'BCN');
  input.value='';
  renderCisternaFleet();
}
function runAutoAssignCisternas(){
  const n=autoAssignCisternas();
  writeCisternaStock();
  saveData('auto_asignacion',{summary:`${n} cisterna(s) auto-asignada(s)`,affectedCount:n});
  renderCalendar();
  renderCisternaFleetIfOpen();
  alert(n?`${n} pedido(s) de cisterna asignados automáticamente.`:'No hay cubas disponibles que encajen (por producto, puerto o documentación) con los pedidos de cisterna pendientes.');
}
function exportCisternaFleet(){
  if(!cisternaStock.length){alert('No hay cubas registradas para exportar.');return;}
  const rows=[['Cuba','Plataforma','Ubicación','Estado','Productos aptos','ITV caduca','ADR caduca','IMDG caduca','Asignado (fecha)','Cliente asignado','Chófer asignado']];
  cisternaStock.forEach(c=>{
    rows.push([c.cuba,c.plataforma||'',c.localizacion||'',estadoCisterna(c),(c.productosAptos||[]).join(', '),c.itvCaduca||'',c.adrCaduca||'',c.imdgCaduca||'',c.fechaEntregaAsignada||'',c.clienteAsignado||'',c.choferAsignado||'']);
  });
  if(typeof XLSX!=='undefined'){
    const ws=XLSX.utils.aoa_to_sheet(rows);
    ws['!cols']=[{wch:16},{wch:12},{wch:10},{wch:16},{wch:26},{wch:12},{wch:12},{wch:12},{wch:14},{wch:20},{wch:16}];
    const wb=XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb,ws,'Flota cisternas');
    const wbout=XLSX.write(wb,{bookType:'xlsx',type:'array'});
    const blob=new Blob([wbout],{type:'application/octet-stream'});
    const url=URL.createObjectURL(blob);
    const a=document.createElement('a');a.href=url;a.download='flota_cisternas.xlsx';document.body.appendChild(a);a.click();a.remove();
    setTimeout(()=>URL.revokeObjectURL(url),1000);
    return;
  }
  const html=`<!doctype html><html><head><meta charset="utf-8"></head><body><table border="1">${rows.map((row,i)=>`<tr>${row.map(cell=>i===0?`<th>${escapeHtml(String(cell))}</th>`:`<td>${escapeHtml(String(cell))}</td>`).join('')}</tr>`).join('')}</table></body></html>`;
  downloadFile('flota_cisternas.xls',html,'application/vnd.ms-excel;charset=utf-8');
}
// ---- MODAL: REGISTRO DE ARRASTRES ----
let registroMovFilters={chofer:'',desde:'',hasta:''};
function getArrastreChoferOptions(){
  const set=new Set(DRIVERS.map(d=>d.name));
  registroMovimientos.forEach(r=>{
    if(r.arrastre1Chofer)set.add(r.arrastre1Chofer);
    if(r.arrastre2Chofer)set.add(r.arrastre2Chofer);
  });
  return Array.from(set).sort();
}
// Fecha de referencia para ordenar/filtrar el registro de arrastres: prioriza las fechas de
// arrastre real (que es el objeto de esta tabla) y solo cae a carga/llegada/descarga/retorno
// cuando no hay ningún arrastre registrado para esa fila.
function registroMovSortDate(r){
  return r.arrastre1Fecha||r.arrastre2Fecha||r.cargaFecha||r.llegadaFecha||r.diaDescargaFecha||r.retornoFecha||'';
}
// Única hora de reloj disponible en la hoja (llegada del barco) — se usa como desempate dentro del
// mismo día para lograr un orden realmente cronológico, no solo por fecha. La columna HORA del Excel
// suele traer solo la hora en entero (p.ej. 8, 7), sin minutos ni formato HH:MM.
function registroMovTimeKey(r){
  const raw=String(r.hora||'').trim();
  if(!raw)return '';
  const hhmm=raw.match(/^(\d{1,2}):(\d{2})/);
  if(hhmm)return `${pad2(hhmm[1])}:${hhmm[2]}`;
  const hourOnly=raw.match(/^(\d{1,2})$/);
  if(hourOnly){
    const h=Number(hourOnly[1]);
    if(h>=0&&h<=23)return `${pad2(h)}:00`;
  }
  return '';
}
function filteredRegistroMov(){
  return registroMovimientos.filter(r=>{
    if(registroMovFilters.chofer){
      const c=normalizeStr(registroMovFilters.chofer);
      const match=normalizeStr(r.arrastre1Chofer)===c||normalizeStr(r.arrastre2Chofer)===c;
      if(!match)return false;
    }
    const refDate=registroMovSortDate(r);
    if(registroMovFilters.desde&&refDate&&refDate<registroMovFilters.desde)return false;
    if(registroMovFilters.hasta&&refDate&&refDate>registroMovFilters.hasta)return false;
    return true;
  }).sort((a,b)=>{
    const da=registroMovSortDate(a),db=registroMovSortDate(b);
    if(da!==db)return db.localeCompare(da); // día más reciente primero
    // Mismo día: orden cronológico real dentro del día (por hora si la conocemos)
    const ta=registroMovTimeKey(a),tb=registroMovTimeKey(b);
    if(ta!==tb){
      if(!ta)return 1;
      if(!tb)return -1;
      return ta.localeCompare(tb);
    }
    return String(a.ga||'').localeCompare(String(b.ga||''),undefined,{numeric:true}); // desempate estable
  });
}
function setRegistroMovFilter(field,value){registroMovFilters[field]=value;renderRegistroMov();}
function openRegistroMov(){renderRegistroMov();document.getElementById('registro-mov-modal').classList.add('open');}
function closeRegistroMov(){document.getElementById('registro-mov-modal').classList.remove('open');}
function renderRegistroMovIfOpen(){if(document.getElementById('registro-mov-modal')?.classList.contains('open'))renderRegistroMov();}
function renderRegistroMov(){
  const choferSel=document.getElementById('registro-mov-chofer');
  if(choferSel&&!choferSel.dataset.built){
    choferSel.innerHTML='<option value="">— todos los chóferes —</option>'+getArrastreChoferOptions().map(n=>`<option value="${escapeHtml(n)}">${escapeHtml(n)}</option>`).join('');
    choferSel.dataset.built='1';
  }
  const desdeInput=document.getElementById('registro-mov-desde');
  if(desdeInput&&!desdeInput.dataset.built){
    desdeInput.value=REGISTRO_MOV_CUTOFF_DATE;
    desdeInput.min=REGISTRO_MOV_CUTOFF_DATE;
    desdeInput.dataset.built='1';
    registroMovFilters.desde=REGISTRO_MOV_CUTOFF_DATE;
  }
  const rows=filteredRegistroMov();
  const cargosExtra=registroMovimientos.filter(r=>r.cargoExtra).length;
  const pendientes=getPendingCisternaAssignments();
  document.getElementById('registro-mov-summary').innerHTML=
    `<strong>${registroMovimientos.length}</strong> movimientos importados · <strong>${rows.length}</strong> mostrados`+
    (cargosExtra?` · <span style="color:#633806;">${cargosExtra} con cargo extra (lavado/paralización/ADR) en columna CHÓFER del Excel</span>`:'')+
    (pendientes.length?` · <span style="color:#791F1F;">${pendientes.length} cuba(s) asignada(s) en el planificador aún sin reflejar en el Excel</span>`:'')+
    ` · <span style="opacity:0.7;">Solo se contabilizan movimientos desde el 01/08/2026</span>`;
  const pendingRows=pendientes.map(p=>`<tr style="background:#FAEEDA;">
      <td>${escapeHtml(p.ga)} <span style="font-size:9px;color:#633806;">⚠ pendiente</span></td>
      <td>${escapeHtml(p.cuba)}</td>
      <td>—</td>
      <td>—</td>
      <td>—</td>
      <td>${escapeHtml(p.chofer||'—')}${p.fecha?' · '+p.fecha:''}</td>
      <td>—</td>
      <td>—</td>
      <td>${escapeHtml(p.cliente||'—')}<div style="font-size:9px;opacity:0.7;">Asignada en el planificador; falta trasladarla al Excel maestro</div></td>
    </tr>`).join('');
  const body=rows.map(r=>{
    const arr1=r.arrastre1Chofer?`${escapeHtml(r.arrastre1Chofer)}${r.arrastre1Fecha?' · '+r.arrastre1Fecha:''}`:'—';
    const arr2=r.arrastre2Chofer?`${escapeHtml(r.arrastre2Chofer)}${r.arrastre2Fecha?' · '+r.arrastre2Fecha:''}`:'—';
    const cargaLlegada=`${r.cargaFecha||'?'} → ${r.llegadaFecha||'?'}`;
    const retorno=r.retornoFecha?`${r.retornoFecha}${r.retornoDestino?' '+escapeHtml(r.retornoDestino):''}`:'—';
    const descarga=r.diaDescargaFecha?`${r.diaDescargaFecha}${r.descargadoEn?' · '+escapeHtml(r.descargadoEn):''}${r.operario?' · '+escapeHtml(r.operario):''}`:'—';
    const cargoTag=r.cargoExtra?` <span style="font-size:10px;background:#FAEEDA;color:#633806;border-radius:3px;padding:1px 5px;">${escapeHtml(r.cargoExtra)}</span>`:'';
    return `<tr>
      <td>${escapeHtml(r.ga)}</td>
      <td>${escapeHtml(r.cuba)}</td>
      <td>${escapeHtml(r.producto)}</td>
      <td>${escapeHtml(r.trayecto)}</td>
      <td>${cargaLlegada}</td>
      <td>${arr1}</td>
      <td>${arr2}</td>
      <td>${retorno}</td>
      <td>${descarga}${cargoTag}</td>
    </tr>`;
  }).join('');
  document.getElementById('registro-mov-tbody').innerHTML=(pendingRows+body)||(pendingRows?'':'<tr><td colspan="9">Sin movimientos importados. Usa "Importar Excel" con el archivo que incluya la hoja "Registro movimientos".</td></tr>');
}
function exportRegistroMovimientos(){
  const rows=filteredRegistroMov();
  const pendientes=getPendingCisternaAssignments();
  if(!rows.length&&!pendientes.length){alert('No hay movimientos para exportar.');return;}
  const aoa=[['GA','Cuba','Plataforma','Producto','Trayecto','Carga','Llegada','Destino','Arrastre 1 chófer','Arrastre 1 fecha','Arrastre 2 chófer','Arrastre 2 fecha','Retorno fecha','Retorno destino','Día descarga','Descargado en','Operario','Ayudante','Cargo extra (col. CHÓFER Excel)','Estado']];
  rows.forEach(r=>{
    aoa.push([r.ga,r.cuba,r.plataforma,r.producto,r.trayecto,r.cargaFecha||'',r.llegadaFecha||'',r.destino,r.arrastre1Chofer||'',r.arrastre1Fecha||'',r.arrastre2Chofer||'',r.arrastre2Fecha||'',r.retornoFecha||'',r.retornoDestino||'',r.diaDescargaFecha||'',r.descargadoEn||'',r.operario||'',r.ayudante||'',r.cargoExtra||'','Excel']);
  });
  // Cubas ya asignadas en el planificador para un GA que el Excel maestro todavía no recoge: se
  // añaden al final marcadas como pendientes, para que Roberto pueda trasladarlas a mano.
  pendientes.forEach(p=>{
    aoa.push([p.ga,p.cuba,'','','','','','',p.chofer||'',p.fecha||'','','','','',p.fecha||'',p.cliente||'','','','','PENDIENTE (asignado en planificador, falta en Excel)']);
  });
  if(typeof XLSX!=='undefined'){
    const ws=XLSX.utils.aoa_to_sheet(aoa);
    const wb=XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb,ws,'Arrastres');
    const wbout=XLSX.write(wb,{bookType:'xlsx',type:'array'});
    const blob=new Blob([wbout],{type:'application/octet-stream'});
    const url=URL.createObjectURL(blob);
    const a=document.createElement('a');a.href=url;a.download='registro_arrastres.xlsx';document.body.appendChild(a);a.click();a.remove();
    setTimeout(()=>URL.revokeObjectURL(url),1000);
    return;
  }
  const html=`<!doctype html><html><head><meta charset="utf-8"></head><body><table border="1">${aoa.map((row,i)=>`<tr>${row.map(cell=>i===0?`<th>${escapeHtml(String(cell))}</th>`:`<td>${escapeHtml(String(cell))}</td>`).join('')}</tr>`).join('')}</table></body></html>`;
  downloadFile('registro_arrastres.xls',html,'application/vnd.ms-excel;charset=utf-8');
}
 
const storageAdapter={
  async get(key){
    if(window.planificadorRemoteStorage&&typeof window.planificadorRemoteStorage.get==='function'){
      try{
        const remote=await window.planificadorRemoteStorage.get(key);
        if(remote)return remote;
      }catch(error){window.planificadorStorageError=error?.message||'No se pudo leer el almacenamiento del servidor.';}
    }
    const fallback=window.storage&&typeof window.storage.get==='function'
      ?await window.storage.get(key)
      :(window.localStorage?((value=>value?{value}:null)(localStorage.getItem(key))):null);
    if(fallback){
      if(window.planificadorRemoteStorage&&typeof window.planificadorRemoteStorage.set==='function')
        try{await window.planificadorRemoteStorage.set(key,typeof fallback.value==='string'?fallback.value:JSON.stringify(fallback.value));}catch(error){window.planificadorStorageError=error?.message||'No se pudo migrar el almacenamiento local.';}
      return fallback;
    }
    return null;
  },
  async set(key,value){
    if(window.planificadorRemoteStorage&&typeof window.planificadorRemoteStorage.set==='function'){
      await window.planificadorRemoteStorage.set(key,value);
      return;
    }
    if(window.storage&&typeof window.storage.set==='function')return window.storage.set(key,value);
    if(window.localStorage)localStorage.setItem(key,value);
  }
};
 
let services=[];
let currentWeek=0;
let editingId=null;
let isEditing=false;
let importRows=[];
let dbPromise=null;
let selectedTimelineDay=null;
let currentTimelineDayKey = null;
 
// Excel serial date → ISO string
function excelDateToISO(serial) {
  const excelEpoch=Date.UTC(1899,11,30);
  const d=new Date(excelEpoch+Math.round(serial*86400*1000));
  return d.toISOString().slice(0,10);
}
function pad2(n){return String(n).padStart(2,'0');}
function localDateKey(d){return `${d.getFullYear()}-${pad2(d.getMonth()+1)}-${pad2(d.getDate())}`;}
function valueDateToISO(value){
  if(value instanceof Date&&!isNaN(value))return localDateKey(value);
  if(typeof value==='number'&&isFinite(value))return excelDateToISO(value);
  const raw=String(value||'').trim();
  if(!raw)return '';
  const iso=raw.match(/^(\d{4})-(\d{1,2})-(\d{1,2})/);
  if(iso)return `${iso[1]}-${pad2(iso[2])}-${pad2(iso[3])}`;
  const dmy=raw.match(/^(\d{1,2})[\/.-](\d{1,2})[\/.-](\d{2,4})$/);
  if(dmy){
    const year=Number(dmy[3].length===2?'20'+dmy[3]:dmy[3]);
    return `${year}-${pad2(dmy[2])}-${pad2(dmy[1])}`;
  }
  const parsed=new Date(raw);
  return isNaN(parsed)?'':localDateKey(parsed);
}
 
function normalizeStr(s){ return s?s.toUpperCase().normalize('NFD').replace(/[\u0300-\u036f]/g,'').trim().replace(/\s+/g,' '):''; }
function normalizeHeader(s){return normalizeStr(s).toLowerCase().replace(/[^a-z0-9]+/g,' ').trim();}
function getImportValue(row, tokenGroups){
  for(const tokens of tokenGroups){
    const found=Object.keys(row).find(k=>{
      const key=normalizeHeader(k);
      return tokens.every(token=>key.includes(token));
    });
    if(found&&row[found]!==undefined&&row[found]!==null&&String(row[found]).trim()!=='')return row[found];
  }
  return '';
}
function sheetToImportRows(ws){
  const aoa=XLSX.utils.sheet_to_json(ws,{header:1,defval:'',raw:true});
  const headerIdx=aoa.findIndex(row=>{
    const headers=row.map(normalizeHeader);
    return headers.some(h=>h.includes('pedido'))&&headers.some(h=>h.includes('fecha'));
  });
  if(headerIdx<0)throw new Error('No encuentro la fila de encabezados del Excel. Debe contener columnas de pedido y fecha.');
  const headers=aoa[headerIdx].map(h=>String(h||'').trim());
  return aoa.slice(headerIdx+1).map(values=>{
    const row={};
    headers.forEach((header,i)=>{if(header)row[header]=values[i];});
    return row;
  });
}
 
function detectZone(pobEntrega, pobCliente) {
  const candidates = [pobEntrega, pobCliente].filter(Boolean);
  for(const pob of candidates){
    const key=normalizeStr(pob);
    if(POB_ZONE[key]) return POB_ZONE[key];
    const base=key.split('-')[0].trim();
    const found=POB_ZONE_KEYS.find(k=>key.startsWith(k)||k.startsWith(base));
    if(found)return POB_ZONE[found];
  }
  return [];
}
 
function cleanPob(pob){
  if(!pob) return '';
  return pob.replace(/-MALLORCA$/i,'').replace(/ MALLORCA$/i,'').replace(/ DE MALLORCA$/i,'').trim();
}
 
 
 
// ---- MUNICIPALITY COORDINATES (Mallorca) ----
// lat/lng for distance calculation
const MUNI_COORDS = {
  'Palma':[39.5696,2.6502],'Marratxí':[39.6197,2.7303],'Santa Maria del Camí':[39.6407,2.7714],
  'Consell':[39.6597,2.8114],'Binissalem':[39.6836,2.8494],'Lloseta':[39.7194,2.8736],
  'Inca':[39.7197,2.9119],'Selva':[39.7714,2.8847],'Campanet':[39.7897,2.9153],
  'Búger':[39.7836,2.9458],'Sencelles':[39.6514,2.8997],'Costitx':[39.6836,2.9319],
  'Lloret de Vistalegre':[39.6336,2.9653],'Sineu':[39.6453,2.9972],'Maria de la Salut':[39.6753,3.0208],
  'Sant Joan':[39.6253,3.0203],'Algaida':[39.5597,2.8986],'Montuïri':[39.5697,2.9814],
  'Petra':[39.6053,3.1069],'Porreres':[39.5153,2.9986],
  'Alcúdia':[39.8542,3.1236],'Pollença':[39.8769,3.0142],'Sa Pobla':[39.7669,3.0214],
  'Muro':[39.7336,3.0503],'Santa Margalida':[39.7069,3.0875],'Can Picafort':[39.7592,3.1564],
  "Port d'Alcúdia":[39.8419,3.1319],
  'Manacor':[39.5669,3.2119],'Artà':[39.6953,3.3503],'Sant Llorenç des Cardassar':[39.6236,3.2869],
  'Son Servera':[39.6203,3.3569],'Capdepera':[39.7069,3.4353],'Felanitx':[39.4753,3.1486],
  'Vilafranca de Bonany':[39.5703,3.0819],'Ariany':[39.6336,3.0619],
  'Llucmajor':[39.4836,2.8897],'Campos':[39.4297,2.9986],'Ses Salines':[39.3453,3.0503],
  'Santanyí':[39.3586,3.1253],"Colònia de Sant Jordi":[39.3153,2.9853],
  'Calvià':[39.5586,2.5136],'Andratx':[39.5753,2.3819],'Puigpunyent':[39.6136,2.5397],
  'Estellencs':[39.6503,2.4653],'Banyalbufar':[39.6869,2.5136],'Magaluf':[39.5053,2.5369],
  'Sóller':[39.7653,2.7153],'Valldemossa':[39.7136,2.6236],'Deià':[39.7469,2.6486],
  'Esporles':[39.6486,2.6097],'Bunyola':[39.6836,2.6986],'Fornalutx':[39.7803,2.7319],
  'Escorca':[39.8219,2.8236],
  // Common variants and import spellings (without accents, with suffixes)
  'Pont d\'Inca':[39.6197,2.7303],'Son Ferriol':[39.5736,2.6986],'Caimari':[39.7836,2.9003],
  'Porto Cristo':[39.5219,3.3336],'Cala Egos':[39.3603,3.1703],'Es Llombards':[39.3619,3.0819],
  'Paguera':[39.5369,2.4636],'Santa Ponsa':[39.5119,2.4836],'Capdellà':[39.5586,2.4669],
  // Without accents (as imported from Excel)
  'Calvia':[39.5586,2.5136],'Alcudia':[39.8542,3.1236],'Pollensa':[39.8769,3.0142],
  'Santanyi':[39.3586,3.1253],'Soller':[39.7653,2.7153],'Montuiri':[39.5697,2.9814],
  'Manacor':[39.5669,3.2119],'Felanitx':[39.4753,3.1486],'Llucmajor':[39.4836,2.8897],
  'Inca':[39.7197,2.9119],'Sineu':[39.6453,2.9972],'Campos':[39.4297,2.9986],
  'Petra':[39.6053,3.1069],'Muro':[39.7336,3.0503],'Selva':[39.7714,2.8847],
  // With -MALLORCA suffix (as imported)
  'Alcudia-Mallorca':[39.8542,3.1236],'Binissalem-Mallorca':[39.6836,2.8494],
  'Lloseta-Mallorca':[39.7194,2.8736],'Inca-Mallorca':[39.7197,2.9119],
  'Campos-Mallorca':[39.4297,2.9986],'Felanitx-Mallorca':[39.4753,3.1486],
  'Manacor-Mallorca':[39.5669,3.2119],'Llucmajor-Mallorca':[39.4836,2.8897],
  'Sa Pobla-Mallorca':[39.7669,3.0214],'Muro-Mallorca':[39.7336,3.0503],
  'Santa Maria-Mallorca':[39.6407,2.7714],'Selva-Mallorca':[39.7714,2.8847],
  // Polígono / industrial areas
  'Pol.Son Castello':[39.5785,2.6712],'Son Castello':[39.5785,2.6712],
};
// Depot coordinates (base in Palma area)
const DEPOT_COORDS = [39.5785, 2.6712]; // Gremi Boters 29, 07009 Palma (Polígon Son Castelló)
const PALMA_PORT_COORDS = [39.5675, 2.6320]; // Estación Marítima, Puerto de Palma (zona Moll Vell / Botavant)
const SPEED_KMH = 50;
const ROAD_FACTOR = 1.30; // Mallorca road distance vs straight line
const SAME_MUNI_MIN = 22; // tiempo mínimo entre dos paradas del mismo municipio
 
function haversineKm(c1, c2) {
  const R = 6371;
  const dLat = (c2[0]-c1[0]) * Math.PI/180;
  const dLon = (c2[1]-c1[1]) * Math.PI/180;
  const a = Math.sin(dLat/2)**2 + Math.cos(c1[0]*Math.PI/180)*Math.cos(c2[0]*Math.PI/180)*Math.sin(dLon/2)**2;
  const straightLine = R * 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
  return straightLine * ROAD_FACTOR; // apply road distance correction
}
 
function getMuniCoords(muni) {
  if(!muni) return null;
  const g = orsCache.geo[normalizeStr(muni)];
  return g || getMuniCoordsStatic(muni);
}
function getMuniCoordsStatic(muni) {
  if(!muni) return null;
  // Direct match
  if(MUNI_COORDS[muni]) return MUNI_COORDS[muni];
  // Case-insensitive + strip accents for matching
  const norm = s => s.toLowerCase()
    .replace(/[àáâã]/g,'a').replace(/[èéêë]/g,'e').replace(/[ìíîï]/g,'i')
    .replace(/[òóôõ]/g,'o').replace(/[ùúûü]/g,'u').replace(/ç/g,'c').replace(/ñ/g,'n')
    .replace(/-mallorca$/i,'').replace(/ mallorca$/i,'').replace(/ de mallorca$/i,'')
    .replace(/pol\.?son /i,'').trim();
  const muniNorm = norm(muni);
  // Try normalised match
  const match = Object.keys(MUNI_COORDS).find(k => norm(k) === muniNorm);
  if(match) return MUNI_COORDS[match];
  // Try partial: muni contains key or key contains muni
  const partial = Object.keys(MUNI_COORDS).find(k => {
    const kn = norm(k);
    return muniNorm.startsWith(kn) || kn.startsWith(muniNorm) || muniNorm.includes(kn);
  });
  return partial ? MUNI_COORDS[partial] : null;
}
 
// ==================================================================================
// ---- RUTAS REALES POR CARRETERA (OpenRouteService, gratuito con clave) ----
// ==================================================================================
// Estrategia: geocodificar cada población UNA vez y precalcular una matriz de km/min
// entre todas (más Base y Puerto). Todo se guarda en caché, así calcSalidaDuration()
// sigue siendo síncrona y, si falta algún dato, usa el cálculo antiguo (haversine).
const ORS_BASE = 'https://api.openrouteservice.org';
const ORS_PROFILE = 'driving-hgv'; // camión; cambia a 'driving-car' si lo prefieres
const ORS_CACHE_KEY = 'planificador_ors_cache_v1';
const ORS_KEY_STORAGE = 'planificador_ors_key';
const ORS_MATRIX_CHUNK = 25; // orígenes por petición (25 x ~80 destinos = ~2000 rutas)
let orsCache = { geo: {}, legs: {}, routes: {}, geom: {}, matrixIds: '' }; // legs: {"A|B":[km,min]}
let orsPointIndex = null; // {points:[{id,coords}], byCoord: Map}
let orsBusy = false;
function getOrsKey(){ try{ return (window.localStorage && localStorage.getItem(ORS_KEY_STORAGE)) || ''; }catch(e){ return ''; } }
function setOrsKey(){
  const cur = getOrsKey();
  const v = prompt('Clave de OpenRouteService (api.openrouteservice.org → Dev Dashboard):', cur);
  if(v === null) return;
  try{ localStorage.setItem(ORS_KEY_STORAGE, v.trim()); }catch(e){}
  if(v.trim()) ensureOrsData(true);
}
function orsSleep(ms){ return new Promise(r => setTimeout(r, ms)); }
async function loadOrsCache(){
  try{
    const r = await storageAdapter.get(ORS_CACHE_KEY);
    if(r && r.value){
      const c = JSON.parse(r.value);
      orsCache = { geo: c.geo || {}, legs: c.legs || {}, routes: c.routes || {}, geom: c.geom || {}, matrixIds: c.matrixIds || '' };
    }
  }catch(e){}
  orsPointIndex = null;
}
function saveOrsCache(){
  try{ storageAdapter.set(ORS_CACHE_KEY, JSON.stringify(orsCache)); }catch(e){}
}
// Puntos conocidos (poblaciones estáticas + geocodificadas + Base + Puerto), sin coordenadas repetidas
function getOrsPoints(){
  if(orsPointIndex) return orsPointIndex;
  const points = [], byCoord = new Map(), ids = new Set();
  const add = (id, coords) => {
    if(!coords) return;
    const ck = coords.join(',');
    if(byCoord.has(ck) || ids.has(id)) return;
    byCoord.set(ck, id);
    ids.add(id);
    points.push({ id, coords });
  };
  add('BASE', DEPOT_COORDS);
  add('PUERTO', PALMA_PORT_COORDS);
  Object.keys(orsCache.geo).forEach(k => add(k, orsCache.geo[k])); // primero: correcciones/geocodificadas
  Object.keys(MUNI_COORDS).forEach(k => add(normalizeStr(k), MUNI_COORDS[k]));
  orsPointIndex = { points, byCoord };
  return orsPointIndex;
}
function orsIdFor(coords, muni){
  const id = getOrsPoints().byCoord.get(coords.join(','));
  return id || normalizeStr(muni);
}
// Lectura SÍNCRONA de un tramo: caché ORS o, si falta, estimación por línea recta x factor carretera
function routeLegSync(idA, coordsA, idB, coordsB){
  const leg = orsCache.legs[idA + '|' + idB];
  if(leg) return { km: leg[0], min: leg[1], real: true };
  const km = haversineKm(coordsA, coordsB);
  return { km, min: Math.round((km / SPEED_KMH) * 60), real: false };
}
async function geocodeMuni(text){
  const key = normalizeStr(text);
  if(!key) return null;
  if(orsCache.geo[key]) return orsCache.geo[key];
  const apiKey = getOrsKey();
  if(!apiKey) return null;
  const url = `${ORS_BASE}/geocode/search?api_key=${encodeURIComponent(apiKey)}`
    + `&text=${encodeURIComponent(text + ', Mallorca, España')}`
    + `&boundary.country=ES&size=1`;
  const res = await fetch(url);
  if(!res.ok) throw new Error('Geocodificación ORS: HTTP ' + res.status);
  const data = await res.json();
  const c = data.features && data.features[0] && data.features[0].geometry.coordinates; // [lng, lat]
  if(!c) return null;
  orsCache.geo[key] = [c[1], c[0]];
  orsPointIndex = null;
  saveOrsCache();
  return orsCache.geo[key];
}
async function buildOrsMatrix(points){
  const apiKey = getOrsKey();
  const locations = points.map(p => [p.coords[1], p.coords[0]]);
  const allIdx = points.map((_, i) => i);
  for(let start = 0; start < points.length; start += ORS_MATRIX_CHUNK){
    const sources = allIdx.slice(start, start + ORS_MATRIX_CHUNK);
    const res = await fetch(`${ORS_BASE}/v2/matrix/${ORS_PROFILE}`, {
      method: 'POST',
      headers: { 'Authorization': apiKey, 'Content-Type': 'application/json' },
      body: JSON.stringify({ locations, sources, destinations: allIdx, metrics: ['distance', 'duration'], units: 'km' })
    });
    if(!res.ok) throw new Error('Matriz ORS: HTTP ' + res.status);
    const data = await res.json();
    sources.forEach((si, r) => {
      allIdx.forEach((dj, c) => {
        if(si === dj) return;
        const d = data.distances[r][c], t = data.durations[r][c];
        if(d == null || t == null) return; // sin ruta: se usará la estimación antigua
        orsCache.legs[points[si].id + '|' + points[dj].id] = [Math.round(d * 10) / 10, Math.round(t / 60)];
      });
    });
    await orsSleep(1600); // respeta el límite por minuto
  }
}
// Geocodifica poblaciones desconocidas y reconstruye la matriz si aparecen puntos nuevos
async function ensureOrsData(force){
  if(orsBusy || !getOrsKey()) return;
  orsBusy = true;
  const ind = document.getElementById('save-indicator');
  try{
    const pending = new Set();
    services.forEach(s => { if(s.muni && !getMuniCoords(s.muni)) pending.add(s.muni.trim()); });
    for(const m of pending){
      if(ind) ind.textContent = 'Localizando ' + m + '…';
      await geocodeMuni(m);
      await orsSleep(700);
    }
    if(ind) ind.textContent = 'Ubicando direcciones…';
    await autoPinFromAddresses();
    const { points } = getOrsPoints();
    const ids = points.map(p => p.id).sort().join(',');
    if(force || orsCache.matrixIds !== ids){
      if(ind) ind.textContent = 'Calculando rutas por carretera…';
      await buildOrsMatrix(points);
      orsCache.matrixIds = ids;
      saveOrsCache();
    }
    if(ind){ ind.textContent = '✓ rutas reales cargadas'; setTimeout(() => { ind.textContent = ''; }, 3000); }
    renderCalendar();
  }catch(e){
    console.warn('ORS:', e);
    if(ind) ind.textContent = '⚠ ORS: ' + e.message + ' (se usa estimación)';
  }finally{
    orsBusy = false;
  }
}

// ==================================================================================
// ---- UBICACIÓN EXACTA DE CLIENTES (pin en mapa, por cliente + obra) ----
// ==================================================================================
const CLIENT_PINS_KEY = 'planificador_client_pins_v1';
let clientPins = {};  // { CLAVE_CLIENTE: {lat,lng,source:'manual'|'auto',label,ts} }
let pinsTried = {};   // claves de cliente cuya dirección ya se intentó geocodificar solas
function getClientPin(client){ return client ? (clientPins[normalizeStr(client)] || null) : null; }
function getStopCoords(item){
  const p = getClientPin(item.client);
  return p ? [p.lat, p.lng] : getMuniCoords(item.muni);
}
async function loadClientPins(){
  try{
    const r = await storageAdapter.get(CLIENT_PINS_KEY);
    if(r && r.value){ const d = JSON.parse(r.value); clientPins = d.pins || {}; pinsTried = d.tried || {}; }
  }catch(e){}
}
function saveClientPins(){
  try{ storageAdapter.set(CLIENT_PINS_KEY, JSON.stringify({ pins: clientPins, tried: pinsTried })); }catch(e){}
}
function inMallorca(c){ return c && c[0] > 39.2 && c[0] < 40.05 && c[1] > 2.2 && c[1] < 3.6; }
// Una dirección se considera "completa" si lleva número y tiene cierta longitud
function isCompleteAddress(d){ const t = String(d || '').trim(); return t.length >= 8 && /\d/.test(t); }

// ---- Rutas con paradas exactas: 1 petición /directions por salida, en caché ----
const orsRouteQueue = new Map();
const orsRouteFailed = new Set();
let orsRouteTimer = null, orsRouteBusy = false;
function orsRouteKey(wp){ return ORS_PROFILE + ':' + wp.map(c => c[0].toFixed(5) + ',' + c[1].toFixed(5)).join('|'); }
function queueOrsRoute(key, wp){
  if(!getOrsKey() || orsRouteFailed.has(key) || orsRouteQueue.has(key)) return;
  orsRouteQueue.set(key, wp);
  if(!orsRouteTimer) orsRouteTimer = setTimeout(processOrsRouteQueue, 1500);
}
async function processOrsRouteQueue(){
  orsRouteTimer = null;
  if(orsRouteBusy) return;
  orsRouteBusy = true;
  let changed = false;
  try{
    while(orsRouteQueue.size){
      const [key, wp] = orsRouteQueue.entries().next().value;
      orsRouteQueue.delete(key);
      try{
        const res = await fetch(`${ORS_BASE}/v2/directions/${ORS_PROFILE}`, {
          method: 'POST',
          headers: { 'Authorization': getOrsKey(), 'Content-Type': 'application/json' },
          body: JSON.stringify({ coordinates: wp.map(c => [c[1], c[0]]), units: 'km', radiuses: wp.map(() => -1) })
        });
        if(!res.ok) throw new Error('HTTP ' + res.status);
        const data = await res.json();
        const segs = data.routes[0].segments;
        if(segs.length !== wp.length - 1) throw new Error('segmentos inesperados');
        orsCache.routes[key] = segs.map(sg => [Math.round(sg.distance * 10) / 10, Math.round(sg.duration / 60)]);
        changed = true;
      }catch(e){ orsRouteFailed.add(key); console.warn('ORS ruta:', e); }
      await orsSleep(1600);
    }
  }finally{ orsRouteBusy = false; }
  if(changed){
    const keys = Object.keys(orsCache.routes);
    if(keys.length > 800) keys.slice(0, 200).forEach(k => delete orsCache.routes[k]);
    saveOrsCache();
    renderCalendar();
    omRefreshIfOpen();
  }
}

// ---- Marca visual en el calendario ----
function pinTag(item){
  if(!item.client) return '';
  const p = getClientPin(item.client);
  if(p && p.source === 'manual') return '';
  const click = `onclick="event.stopPropagation();openPinPickerForService(${JSON.stringify(item.id)})"`;
  const st = 'font-size:10px;cursor:pointer;flex-shrink:0;';
  return p
    ? `<span ${click} style="${st}opacity:0.9;" title="Ubicación propuesta automáticamente por la dirección: pulsa para confirmarla o ajustarla">📍~</span>`
    : `<span ${click} style="${st}opacity:0.55;" title="Sin ubicación exacta (se usa el centro de la población): pulsa para ubicar en el mapa">📍?</span>`;
}

// ---- Selector en mapa (Leaflet + OpenStreetMap) ----
let pinMap = null, pinMarker = null, pinCtx = null, pinCurrent = null, pinCandidates = [];
function updatePinStatus(){
  const el = document.getElementById('pin-status');
  if(!el) return;
  const c = document.getElementById('f-client').value.trim();
  if(!c){ el.textContent = ''; return; }
  const p = getClientPin(c);
  el.textContent = p ? (p.source === 'manual' ? '📍 ubicación exacta guardada' : '📍 propuesta automática (sin confirmar)')
                     : 'sin ubicación exacta (se usa el centro de la población)';
}
function openPinPickerFromForm(){
  const client = document.getElementById('f-client').value.trim();
  if(!client){ alert('Escribe primero el cliente / referencia: la ubicación se guarda por cliente.'); return; }
  openPinPicker({ client, muni: document.getElementById('f-muni').value.trim(), direccion: document.getElementById('f-direccion').value.trim(), cp: (services.find(x => x.id === editingId) || {}).cp || '', onSave: updatePinStatus });
}
function openPinPickerForService(id){
  const s = services.find(x => x.id === id);
  if(!s || !s.client) return;
  openPinPicker({ client: s.client, muni: s.muni || '', direccion: s.direccion || '', cp: s.cp || '', onSave: () => {} });
}
function setPinMarker(coords, note, zoom){
  pinCurrent = coords;
  if(!pinMarker){
    pinMarker = L.marker(coords, { draggable: true }).addTo(pinMap);
    pinMarker.on('dragend', () => { const p = pinMarker.getLatLng(); setPinMarker([p.lat, p.lng], 'Punto ajustado a mano'); });
  }else pinMarker.setLatLng(coords);
  if(zoom) pinMap.setView(coords, zoom);
  document.getElementById('pin-info').textContent = `${note} · ${coords[0].toFixed(5)}, ${coords[1].toFixed(5)}`;
}
function openPinPicker(ctx){
  if(typeof L === 'undefined'){ alert('No se ha cargado el mapa (Leaflet). Comprueba la conexión a internet.'); return; }
  pinCtx = ctx; pinCtx.key = ctx.refKey || normalizeStr(ctx.client);
  pinCurrent = null; pinCandidates = [];
  document.getElementById('pin-title').textContent = ctx.title || ('Ubicar en mapa — ' + ctx.client);
  document.getElementById('pin-results').innerHTML = '';
  document.getElementById('pin-info').textContent = '';
  const query = [ctx.direccion, ctx.cp, ctx.muni].filter(Boolean).join(', ');
  document.getElementById('pin-search').value = ctx.searchText || query;
  const existing = ctx.refKey ? { lat: ctx.current[0], lng: ctx.current[1], source: 'manual' } : clientPins[pinCtx.key];
  document.getElementById('pin-btn-remove').style.display = (existing && !ctx.refKey) ? '' : 'none';
  document.getElementById('pin-modal').classList.add('open');
  setTimeout(() => {
    if(!pinMap){
      pinMap = L.map('pin-map').setView([39.62, 2.98], 9);
      const calles = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', { maxZoom: 19, attribution: 'Mapa © Esri, HERE, Garmin, © OpenStreetMap contributors' });
      const satelite = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', { maxZoom: 19, attribution: 'Imágenes © Esri' });
      // OpenStreetMap solo funciona si el planificador se abre por http(s) (necesita Referer), no desde file://
      const osm = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19, attribution: '© OpenStreetMap contributors' });
      calles.addTo(pinMap);
      L.control.layers({ 'Calles': calles, 'Satélite': satelite, 'OpenStreetMap': osm }).addTo(pinMap);
      pinMap.on('click', e => setPinMarker([e.latlng.lat, e.latlng.lng], 'Punto elegido a mano'));
    }
    pinMap.invalidateSize();
    if(pinMarker){ pinMap.removeLayer(pinMarker); pinMarker = null; }
    if(existing){
      setPinMarker([existing.lat, existing.lng], ctx.refKey ? 'Ubicación actual (arrastra el pin o haz clic para moverla; usa el selector de capas → Satélite para ver el muelle)' : (existing.source === 'manual' ? 'Ubicación guardada' : 'Propuesta automática (confirma o ajusta)'), ctx.refKey ? 17 : 16);
    }else{
      const mc = getMuniCoords(ctx.muni);
      pinMap.setView(mc || [39.62, 2.98], mc ? 13 : 9);
      if(getOrsKey() && ctx.direccion) pinSearch(true);
      else document.getElementById('pin-info').textContent = 'Haz clic en el mapa para colocar el punto (o busca una dirección). Puedes arrastrar el pin para afinar.';
    }
  }, 60);
}
async function pinSearch(auto){
  const text = document.getElementById('pin-search').value.trim();
  if(!text) return;
  if(!getOrsKey()){ alert('Para buscar direcciones hace falta la clave de ORS ("🗺 Clave rutas"). También puedes hacer clic directamente en el mapa.'); return; }
  const box = document.getElementById('pin-results');
  box.textContent = 'Buscando…';
  try{
    pinCandidates = await geocodeCandidates(text);
    if(!pinCandidates.length){
      box.textContent = 'Sin resultados. Prueba con menos datos (calle y población) o haz clic en el mapa.';
      return;
    }
    box.innerHTML = pinCandidates.map((c, i) => {
      const exact = c.accuracy === 'point';
      return `<div style="padding:3px 6px;font-size:12px;cursor:pointer;border-bottom:0.5px solid var(--color-border-tertiary);" onclick="pinChooseCandidate(${i})">${escapeHtml(c.label)} <span style="font-size:10px;padding:0 4px;border-radius:3px;background:${exact ? '#DCF3EC' : '#FAEEDA'};color:${exact ? '#0B6E4F' : '#633806'};">${exact ? 'punto exacto' : 'aproximado'}</span></div>`;
    }).join('');
    const best = pinCandidates[0];
    if(auto === true && best.accuracy === 'point' && inMallorca(best.coords)){
      setPinMarker(best.coords, 'Propuesta por la dirección — confirma o ajusta', 17);
    }else if(auto === true){
      document.getElementById('pin-info').textContent = 'La dirección es incompleta o aproximada: elige un resultado o haz clic en el mapa para colocar el punto.';
    }
  }catch(e){ box.textContent = 'No se pudo buscar: ' + e.message; }
}
function pinChooseCandidate(i){
  const c = pinCandidates[i];
  if(!c) return;
  setPinMarker(c.coords, c.accuracy === 'point' ? 'Resultado exacto — confirma o ajusta' : 'Resultado aproximado — ajústalo en el mapa', c.accuracy === 'point' ? 17 : 15);
}
function savePin(){
  if(!pinCtx || !pinCurrent){ alert('Coloca primero el punto en el mapa.'); return; }
  if(pinCtx.refKey){ const k = pinCtx.refKey, cb0 = pinCtx.onSave; setRefPoint(k, pinCurrent); closePinPicker(); if(cb0) cb0(); return; }
  const mc = getMuniCoords(pinCtx.muni);
  if(mc && haversineKm(mc, pinCurrent) > 30 && !confirm('El punto está a más de 30 km del centro de ' + pinCtx.muni + '. ¿Guardarlo de todas formas?')) return;
  clientPins[pinCtx.key] = { lat: pinCurrent[0], lng: pinCurrent[1], source: 'manual', label: document.getElementById('pin-search').value.trim(), ts: new Date().toISOString() };
  delete pinsTried[pinCtx.key];
  saveClientPins();
  const cb = pinCtx.onSave;
  closePinPicker();
  if(cb) cb();
  renderCalendar();
}
function removePin(){
  if(!pinCtx) return;
  if(!confirm('Se quita la ubicación exacta de este cliente y se volverá a usar el centro de la población. ¿Continuar?')) return;
  delete clientPins[pinCtx.key];
  pinsTried[pinCtx.key] = 1; // no volver a proponerla automáticamente
  saveClientPins();
  const cb = pinCtx.onSave;
  closePinPicker();
  if(cb) cb();
  renderCalendar();
}
function closePinPicker(){ document.getElementById('pin-modal').classList.remove('open'); pinCtx = null; }

// Propuesta automática de pines para direcciones completas (se confirman luego en el mapa)
async function autoPinFromAddresses(){
  const todo = [];
  services.forEach(s => {
    if(!s.client || !s.direccion || !isCompleteAddress(s.direccion)) return;
    const k = normalizeStr(s.client);
    if(clientPins[k] || pinsTried[k] || todo.some(t => t.k === k)) return;
    todo.push({ k, s });
  });
  let added = 0;
  for(const t of todo.slice(0, 40)){
    pinsTried[t.k] = 1;
    try{
      const c = await geocodeCandidates([t.s.direccion, t.s.cp, t.s.muni, 'Mallorca'].filter(Boolean).join(', '));
      const best = c[0];
      if(best && best.accuracy === 'point' && (best.confidence == null || best.confidence >= 0.7) && inMallorca(best.coords)){
        clientPins[t.k] = { lat: best.coords[0], lng: best.coords[1], source: 'auto', label: best.label, ts: new Date().toISOString() };
        added++;
      }
    }catch(e){ delete pinsTried[t.k]; break; }
    await orsSleep(700);
  }
  if(todo.length) saveClientPins();
  return added;
}

// ==================================================================================
// ---- MAPA DE PEDIDOS PENDIENTES: seleccionar en el mapa y crear una ruta ----
// ==================================================================================
let omMap = null, omLayer = null, omNumLayer = null, omRouteLine = null, omFitted = false;
let omMarkers = new Map(), omSelectable = new Set(), omSel = [];
let omAreaMode = false, omAreaStart = null, omAreaRect = null;
const omForm = { day: '', driver: '', vehicle: '', hora: '', dayManual: false, crewTouched: false };

function addBaseLayers(map){
  const calles = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', { maxZoom: 19, attribution: 'Mapa © Esri, HERE, Garmin, © OpenStreetMap contributors' });
  const satelite = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', { maxZoom: 19, attribution: 'Imágenes © Esri' });
  const osm = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19, attribution: '© OpenStreetMap contributors' });
  calles.addTo(map);
  L.control.layers({ 'Calles': calles, 'Satélite': satelite, 'OpenStreetMap': osm }).addTo(map);
}
function omDayOptions(){
  return [-2, -1, 0, 1, 2].flatMap(getDaysForWeek).filter(d => !isWeekend(d)).map(dateKey);
}
function omBasePending(){
  const incl = document.getElementById('om-incl').checked;
  return services.filter(s => s.type === 'reparto' && !s.servido && !s.noEntregado && (incl || !s.driver));
}
function omItems(){ return omSel.map(id => services.find(x => x.id === id)).filter(Boolean); }
function omOrderLabel(s){ return `${s.pedidoNum ? '#' + s.pedidoNum + ' · ' : ''}${s.client || '(sin cliente)'}`; }

function openOrdersMap(){
  if(typeof L === 'undefined'){ alert('No se ha cargado el mapa (Leaflet). Comprueba la conexión a internet.'); return; }
  omSel = []; omFitted = false;
  omForm.day = ''; omForm.driver = ''; omForm.vehicle = ''; omForm.hora = ''; omForm.dayManual = false; omForm.crewTouched = false;
  document.getElementById('orders-map-modal').classList.add('open');
  setTimeout(() => {
    if(!omMap){
      omMap = L.map('orders-map').setView([39.62, 2.98], 9);
      addBaseLayers(omMap);
      omLayer = L.layerGroup().addTo(omMap);
      omNumLayer = L.layerGroup().addTo(omMap);
      registerRefMarker('DEPOT_COORDS', L.circleMarker(DEPOT_COORDS, { radius: 7, color: '#000', fillColor: '#fff', fillOpacity: 1, weight: 3 }).bindTooltip('Base').addTo(omMap));
      registerRefMarker('PALMA_PORT_COORDS', L.circleMarker(PALMA_PORT_COORDS, { radius: 6, color: '#0C447C', fillColor: '#fff', fillOpacity: 1, weight: 3 }).bindTooltip('Puerto de Palma').addTo(omMap));
      omMap.on('mousedown', e => {
        if(!omAreaMode) return;
        omAreaStart = e.latlng;
        omAreaRect = L.rectangle([e.latlng, e.latlng], { color: '#185FA5', weight: 1, fillOpacity: 0.1 }).addTo(omMap);
      });
      omMap.on('mousemove', e => { if(omAreaRect) omAreaRect.setBounds([omAreaStart, e.latlng]); });
      omMap.on('mouseup', () => {
        if(!omAreaRect) return;
        const b = omAreaRect.getBounds();
        omMap.removeLayer(omAreaRect); omAreaRect = null;
        omMarkers.forEach((m, id) => { if(omSelectable.has(id) && b.contains(m.getLatLng()) && !omSel.includes(id)) omSel.push(id); });
        renderOrdersMap(false);
      });
    }
    omMap.invalidateSize();
    renderOrdersMap(true);
  }, 60);
}
function closeOrdersMap(){
  document.getElementById('orders-map-modal').classList.remove('open');
  if(omAreaMode) toggleAreaSelect();
}
function toggleAreaSelect(){
  omAreaMode = !omAreaMode;
  const b = document.getElementById('om-area-btn');
  b.style.background = omAreaMode ? '#185FA5' : 'var(--color-background-secondary)';
  b.style.color = omAreaMode ? '#fff' : 'var(--color-text-primary)';
  if(omMap){ omAreaMode ? omMap.dragging.disable() : omMap.dragging.enable(); }
}
function omMarkerStyle(s, pinned, selected){
  if(selected) return { radius: 10, color: '#0C447C', fillColor: '#185FA5', fillOpacity: 0.95, weight: 3, dashArray: null };
  if(s.driver) return { radius: 8, color: '#777', fillColor: '#bbb', fillOpacity: 0.6, weight: 1, dashArray: null };
  return { radius: 9, color: pinned ? '#7A3200' : '#8A5A00', fillColor: pinned ? '#D55E00' : '#E69F00', fillOpacity: 0.85, weight: 2, dashArray: pinned ? null : '3' };
}
function renderOrdersMap(fit){
  if(!omMap) return;
  // desplegable de días (se conserva la selección)
  const daySel = document.getElementById('om-day');
  const cur = daySel.value;
  const allDays = Array.from(new Set(omBasePending().map(s => s.day))).sort();
  daySel.innerHTML = '<option value="">Todos los días</option>' + allDays.map(d => {
    const dt = new Date(d + 'T00:00:00');
    return `<option value="${d}">${DAYS_ES[dt.getDay()]} ${dt.getDate()} ${MONTHS_ES[dt.getMonth()]}</option>`;
  }).join('');
  daySel.value = allDays.includes(cur) ? cur : '';
  const dayF = daySel.value;
  const list = omBasePending().filter(s => !dayF || s.day === dayF);

  omLayer.clearLayers(); omMarkers.clear(); omSelectable.clear();
  const unmapped = [], seen = {};
  list.forEach(s => {
    const coords = getStopCoords(s);
    if(!coords){ unmapped.push(s); return; }
    const k = coords[0].toFixed(4) + ',' + coords[1].toFixed(4);
    const n = (seen[k] = (seen[k] || 0) + 1) - 1;
    let pos = coords;
    if(n > 0){ const ang = n * 2.4, r = 0.0025 * Math.sqrt(n); pos = [coords[0] + r * Math.sin(ang), coords[1] + r * Math.cos(ang) / 0.78]; }
    const pinned = !!getClientPin(s.client);
    const dr = s.driver ? DRIVER_BY_ID.get(s.driver) : null;
    const arts = (s.articles || []).map(a => getArticleName(a.artCod) + (a.qty ? ' ×' + a.qty : '')).join('<br>');
    const m = L.circleMarker(pos, omMarkerStyle(s, pinned, omSel.includes(s.id)));
    m.bindTooltip(`<strong>${escapeHtml(omOrderLabel(s))}</strong><br>${escapeHtml(s.muni || '')} · ${s.day}${s.direccion ? '<br>' + escapeHtml(s.direccion) : ''}<br><em>${pinned ? 'ubicación exacta' : 'centro de la población'}</em>${dr ? '<br>asignado a ' + dr.name : ''}${arts ? '<br>' + arts : ''}`);
    m.on('click', () => toggleMapOrder(s.id));
    m.addTo(omLayer);
    omMarkers.set(s.id, m);
    if(!s.driver) omSelectable.add(s.id);
  });
  omSel = omSel.filter(id => omSelectable.has(id));
  if(fit && !omFitted && omMarkers.size){
    omMap.fitBounds(L.featureGroup(Array.from(omMarkers.values())).getBounds().pad(0.15));
    omFitted = true;
  }
  document.getElementById('om-legend').innerHTML =
    `<span style="color:#D55E00;">●</span> exacta sin asignar &nbsp; <span style="color:#E69F00;">◌</span> aprox. (centro población) sin asignar &nbsp; <span style="color:#185FA5;">●</span> seleccionado &nbsp; <span style="color:#999;">●</span> ya asignado · ${omSelectable.size} seleccionables`;
  omRefreshPanel(unmapped);
}
function toggleMapOrder(id){
  if(!omSelectable.has(id)) return; // los ya asignados no se pueden seleccionar
  const i = omSel.indexOf(id);
  if(i >= 0) omSel.splice(i, 1); else omSel.push(id);
  renderOrdersMap(false);
}
function omMove(id, dir){
  const i = omSel.indexOf(id), j = i + dir;
  if(i < 0 || j < 0 || j >= omSel.length) return;
  [omSel[i], omSel[j]] = [omSel[j], omSel[i]];
  renderOrdersMap(false);
}
function omRemove(id){ omSel = omSel.filter(x => x !== id); renderOrdersMap(false); }
function omClear(){ omSel = []; omForm.crewTouched = false; omForm.dayManual = false; renderOrdersMap(false); }
function omOptimize(){
  const items = omItems();
  const rest = items.filter(s => getStopCoords(s)), noCoords = items.filter(s => !getStopCoords(s));
  const out = []; let last = DEPOT_COORDS;
  while(rest.length){
    let bi = 0, bd = Infinity;
    rest.forEach((s, i) => { const d = haversineKm(last, getStopCoords(s)); if(d < bd){ bd = d; bi = i; } });
    const s = rest.splice(bi, 1)[0]; out.push(s); last = getStopCoords(s);
  }
  omSel = out.concat(noCoords).map(s => s.id);
  renderOrdersMap(false);
}
function omPickDay(items){
  const opts = omDayOptions(), todayKey = localDateKey(new Date());
  const dayF = document.getElementById('om-day').value;
  let want = dayF || items.map(s => s.day).sort()[0] || todayKey;
  if(want < todayKey) want = todayKey;
  return opts.includes(want) ? want : (opts.find(d => d >= want) || opts[0]);
}
function omSuggestCrew(items, day){
  const veh = decideRouteVehicle(items);
  const routeMin = routeTotalMin(items);
  const usage = initDriverUsageMap(day);
  const a = findDriverForRoute(routeMin, veh, usage, false);
  if(a){ omForm.driver = String(a.driverId); omForm.vehicle = a.vehicleId || ''; omForm.hora = a.horaSalida || ''; }
  else { omForm.driver = ''; omForm.vehicle = veh.vehicleId || ''; omForm.hora = ''; }
}
function omRefreshPanel(unmapped){
  const panel = document.getElementById('om-panel');
  omNumLayer.clearLayers();
  if(omRouteLine){ omMap.removeLayer(omRouteLine); omRouteLine = null; }
  // pedidos sin población localizable
  const unmappedHtml = (unmapped && unmapped.length)
    ? `<div style="margin-top:10px;font-size:11px;"><strong style="color:#791F1F;">Sin ubicar (${unmapped.length})</strong> — falta población/dirección:` +
      unmapped.slice(0, 8).map(s => `<div style="cursor:pointer;padding:2px 4px;background:#FBE4D5;border-radius:3px;margin-top:2px;" onclick="closeOrdersMap();openEdit(${JSON.stringify(s.id)})">${escapeHtml(omOrderLabel(s))}</div>`).join('') +
      (unmapped.length > 8 ? `<div style="opacity:0.7;">…y ${unmapped.length - 8} más</div>` : '') + '</div>'
    : '';
  const items = omItems();
  if(!items.length){
    panel.innerHTML = `<div style="font-size:12px;color:var(--color-text-secondary);">Haz clic en los pedidos del mapa para añadirlos a la ruta, o usa <strong>Seleccionar área</strong> para arrastrar un rectángulo sobre varios.</div>${unmappedHtml}`;
    return;
  }
  // números sobre los marcadores + línea de recorrido
  const path = [DEPOT_COORDS];
  items.forEach((s, i) => {
    const m = omMarkers.get(s.id); if(!m) return;
    const p = m.getLatLng(); path.push([p.lat, p.lng]);
    L.marker(p, { interactive: false, icon: L.divIcon({ className: '', iconSize: [20, 20], html: `<div style="width:20px;height:20px;line-height:20px;text-align:center;font-size:11px;font-weight:600;color:#fff;">${i + 1}</div>` }) }).addTo(omNumLayer);
  });
  path.push(DEPOT_COORDS);
  omRouteLine = L.polyline(path, { color: '#185FA5', weight: 2, dashArray: '6 6', opacity: 0.8 }).addTo(omMap);

  const useReal = document.getElementById('om-real').checked;
  const dur = calcSalidaDuration({ items, driver: useReal ? 1 : null, origenTrayecto: 'base', destinoTrayecto: 'base' });
  const total = ROUTE_CARGA_MIN + dur.totalMin;
  const over = total > ROUTE_BUDGET_MIN;
  const veh = decideRouteVehicle(items);
  if(!omForm.dayManual) omForm.day = omPickDay(items);
  if(!omForm.crewTouched) omSuggestCrew(items, omForm.day);
  const dayOpts = omDayOptions().map(d => { const dt = new Date(d + 'T00:00:00'); return `<option value="${d}" ${omForm.day === d ? 'selected' : ''}>${DAYS_ES[dt.getDay()]} ${dt.getDate()} ${MONTHS_ES[dt.getMonth()]}</option>`; }).join('');
  const stopsHtml = items.map((s, i) => `<div style="display:flex;align-items:center;gap:4px;font-size:11px;padding:2px 0;border-bottom:0.5px solid var(--color-border-tertiary);">
      <span style="width:16px;font-weight:600;">${i + 1}</span>
      <span style="flex:1;min-width:0;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;" title="${escapeHtml(omOrderLabel(s))}">${escapeHtml(omOrderLabel(s))} <span style="opacity:0.6;">${escapeHtml(s.muni || '')}</span></span>
      <button class="item-order-btn" onclick="omMove(${JSON.stringify(s.id)},-1)">↑</button>
      <button class="item-order-btn" onclick="omMove(${JSON.stringify(s.id)},1)">↓</button>
      <button class="item-order-btn" onclick="omRemove(${JSON.stringify(s.id)})">✕</button></div>`).join('');
  panel.innerHTML = `
    <div style="font-weight:600;font-size:13px;margin-bottom:4px;">Ruta (${items.length} parada${items.length > 1 ? 's' : ''})</div>
    <div style="max-height:190px;overflow-y:auto;margin-bottom:6px;">${stopsHtml}</div>
    <div style="display:flex;gap:6px;margin-bottom:8px;">
      <button class="btn-secondary" style="padding:3px 8px;font-size:11px;" onclick="omOptimize()">Ordenar por cercanía</button>
      <button class="btn-secondary" style="padding:3px 8px;font-size:11px;" onclick="omClear()">Vaciar</button>
    </div>
    <div style="font-size:12px;padding:6px 8px;border-radius:var(--border-radius-md);background:${over ? '#FBE4D5' : '#EEF6FD'};border:0.5px solid ${over ? '#F3C9A0' : '#B5D4F4'};margin-bottom:8px;">
      <strong>${formatDuration(total)}</strong> en total (${ROUTE_CARGA_MIN} min carga + ${formatDuration(dur.driveMin)} conducción + ${formatDuration(dur.descargaMin)} descarga) · ${dur.km} km<br>
      <span style="font-size:11px;${over ? 'color:#7A3200;' : 'opacity:0.7;'}">${over ? '⚠ supera la jornada de 7:00–14:00 (' + formatDuration(ROUTE_BUDGET_MIN) + ' útiles)' : 'cabe en la jornada (' + formatDuration(ROUTE_BUDGET_MIN) + ' útiles)'}${dur.missingCoords ? ' · ⚠ alguna parada sin ubicar' : ''}${useReal ? '' : ' · estimación'}</span><br>
      <span style="font-size:11px;opacity:0.75;">Vehículo recomendado: ${veh.vehicleId ? VEHICLE_BY_ID.get(veh.vehicleId).name : 'tractora/camión'}</span>
    </div>
    <div class="form-row"><label>Día</label><select onchange="omForm.day=this.value;omForm.dayManual=true;omForm.crewTouched=false;renderOrdersMap(false)">${dayOpts}</select></div>
    <div class="form-row"><label>Conductor</label><select onchange="omForm.driver=this.value;omForm.crewTouched=true;omForm.vehicle='';renderOrdersMap(false)"><option value="">— elegir —</option>${buildDriverOptionsHtml(omForm.driver, false)}</select></div>
    <div class="form-row"><label>Vehículo</label><select onchange="omForm.vehicle=this.value;omForm.crewTouched=true"><option value="">— elegir —</option>${omForm.driver ? buildVehicleOptionsHtml(omForm.vehicle, omForm.driver) : ''}</select></div>
    <div class="form-row"><label>Hora de salida</label><input type="time" value="${omForm.hora || ''}" onchange="omForm.hora=this.value;omForm.crewTouched=true" style="width:100%;"></div>
    <button class="btn-primary" style="width:100%;" onclick="createRouteFromMap()">Crear ruta con ${items.length} pedido${items.length > 1 ? 's' : ''}</button>
    ${unmappedHtml}`;
}
function createRouteFromMap(){
  const items = omItems();
  if(!items.length){ alert('Selecciona al menos un pedido en el mapa.'); return; }
  const day = omForm.day, driver = parseInt(omForm.driver), vehicle = omForm.vehicle, hora = omForm.hora;
  if(!day || !driver || !vehicle || !hora){ alert('Elige día, conductor, vehículo y hora de salida.'); return; }
  const vh = VEHICLE_BY_ID.get(vehicle), dr = DRIVER_BY_ID.get(driver);
  if(vh && vh.type === 'tractora' && !dr.canTractora){ alert(dr.name + ' no puede usar tractoras.'); return; }
  const dur = calcSalidaDuration({ items, origenTrayecto: 'base', destinoTrayecto: 'base' });
  const end = addMinutes(hora, dur.totalMin);
  const conflict = getPersonBusyIntervals(day, driver, null, null).find(iv => intervalsOverlap(hora, end, iv.start, iv.end));
  if(conflict && !confirm(`${dr.name} ya tiene otra salida entre ${conflict.start} y ${conflict.end}, que se solapa con esta (${hora}–${end}). ¿Crear la ruta de todas formas?`)) return;
  const salida = nextSalidaNumberForDriver(day, driver, null);
  items.forEach((s, i) => { s.day = day; s.driver = driver; s.vehicle = vehicle; s.salida = salida; s.horaSalida = hora; s.itemOrder = i; });
  saveData('alta', { summary: `Ruta creada desde el mapa (${items.length} pedidos, ${dr.name})`, day, affectedCount: items.length });
  closeOrdersMap();
  goToWeek(weekOffsetForDate(day));
  renderCalendar();
}
function omRefreshIfOpen(){
  const m = document.getElementById('orders-map-modal');
  if(m && m.classList.contains('open') && omMap) renderOrdersMap(false);
}

// ==================================================================================
// ---- MAPA DE RUTAS DEL DÍA: salidas (repartos, cisternas, arrastres) dibujadas ----
// ==================================================================================
const DM_COLORS = ['#0072B2', '#D55E00', '#009E73', '#CC79A7', '#E69F00', '#56B4E9', '#8E44AD', '#555555']; // aptos para daltonismo
let dmMap = null, dmLayer = null, dmBoundsList = [];
function dmColor(driverId){ return DM_COLORS[(((driverId || 1) - 1) % DM_COLORS.length + DM_COLORS.length) % DM_COLORS.length]; }

// ---- trazado por carretera (geometría ORS), en caché ----
const orsGeomQueue = new Map(), orsGeomFailed = new Set();
let orsGeomBusy = false, orsGeomTimer = null;
function queueOrsGeom(key, wp){
  if(!getOrsKey() || orsGeomFailed.has(key) || orsGeomQueue.has(key)) return;
  orsGeomQueue.set(key, wp);
  if(!orsGeomTimer) orsGeomTimer = setTimeout(processOrsGeomQueue, 800);
}
async function processOrsGeomQueue(){
  orsGeomTimer = null;
  if(orsGeomBusy) return;
  orsGeomBusy = true;
  let changed = false;
  try{
    while(orsGeomQueue.size){
      const [key, wp] = orsGeomQueue.entries().next().value;
      orsGeomQueue.delete(key);
      try{
        const res = await fetch(`${ORS_BASE}/v2/directions/${ORS_PROFILE}/geojson`, {
          method: 'POST',
          headers: { 'Authorization': getOrsKey(), 'Content-Type': 'application/json' },
          body: JSON.stringify({ coordinates: wp.map(c => [c[1], c[0]]), units: 'km', radiuses: wp.map(() => -1), geometry_simplify: true })
        });
        if(!res.ok) throw new Error('HTTP ' + res.status);
        const data = await res.json();
        const f = data.features[0];
        let pts = f.geometry.coordinates.map(c => [Math.round(c[1] * 1e5) / 1e5, Math.round(c[0] * 1e5) / 1e5]);
        if(pts.length > 300){ // aligera para no llenar el almacenamiento
          const step = Math.ceil(pts.length / 300);
          pts = pts.filter((_, i) => i % step === 0 || i === pts.length - 1);
        }
        orsCache.geom[key] = pts;
        const segs = f.properties.segments;
        if(segs && segs.length === wp.length - 1) orsCache.routes[key] = segs.map(sg => [Math.round(sg.distance * 10) / 10, Math.round(sg.duration / 60)]);
        changed = true;
      }catch(e){ orsGeomFailed.add(key); console.warn('ORS trazado:', e); }
      await orsSleep(1600);
    }
  }finally{ orsGeomBusy = false; }
  if(changed){
    const keys = Object.keys(orsCache.geom);
    if(keys.length > 60) keys.slice(0, 20).forEach(k => delete orsCache.geom[k]);
    saveOrsCache();
    renderCalendar();
    dmRefreshIfOpen();
  }
}
function dmRefreshIfOpen(){
  const m = document.getElementById('day-map-modal');
  if(m && m.classList.contains('open') && dmMap) renderDayMap(false);
}
function openDayMap(){
  if(typeof L === 'undefined'){ alert('No se ha cargado el mapa (Leaflet). Comprueba la conexión a internet.'); return; }
  document.getElementById('day-map-modal').classList.add('open');
  const sel = document.getElementById('dm-day');
  const opts = omDayOptions(), todayKey = localDateKey(new Date());
  sel.innerHTML = opts.map(d => { const dt = new Date(d + 'T00:00:00'); return `<option value="${d}">${DAYS_ES[dt.getDay()]} ${dt.getDate()} ${MONTHS_ES[dt.getMonth()]}</option>`; }).join('');
  sel.value = opts.includes(todayKey) ? todayKey : (opts.find(d => d >= todayKey) || opts[0]);
  setTimeout(() => {
    if(!dmMap){
      dmMap = L.map('day-map').setView([39.62, 2.98], 9);
      addBaseLayers(dmMap);
      registerRefMarker('DEPOT_COORDS', L.circleMarker(DEPOT_COORDS, { radius: 7, color: '#000', fillColor: '#fff', fillOpacity: 1, weight: 3 }).bindTooltip('Base').addTo(dmMap));
      registerRefMarker('PALMA_PORT_COORDS', L.circleMarker(PALMA_PORT_COORDS, { radius: 6, color: '#0C447C', fillColor: '#fff', fillOpacity: 1, weight: 3 }).bindTooltip('Puerto de Palma').addTo(dmMap));
      dmLayer = L.layerGroup().addTo(dmMap);
    }
    dmMap.invalidateSize();
    renderDayMap(true);
  }, 60);
}
function closeDayMap(){ document.getElementById('day-map-modal').classList.remove('open'); }
function dmFocus(i){ const b = dmBoundsList[i]; if(b && dmMap) dmMap.fitBounds(b.pad(0.2)); }
function renderDayMap(fit){
  if(!dmMap) return;
  const dayKey = document.getElementById('dm-day').value;
  const typeF = document.getElementById('dm-type').value;   // '' | 'reparto' | 'cisterna'
  const useRoad = document.getElementById('dm-road').checked;
  dmLayer.clearLayers(); dmBoundsList = [];
  const dayServices = servicesForDay(dayKey);
  const salidas = getSalidasForDay(dayKey, dayServices).filter(sal => !typeF || sal.type === typeF);
  const arrastres = typeF === 'reparto' ? [] : dayServices.filter(s => s.type === 'arrastre');
  const allPts = [], cards = [], seen = {};
  let nStops = 0, nUnlocated = 0, nRoadPending = 0;
  salidas.forEach(sal => {
    const color = dmColor(sal.driver);
    const dr = DRIVER_BY_ID.get(sal.driver), vh = VEHICLE_BY_ID.get(sal.vehicle);
    const startC = sal.origenTrayecto === 'puerto' ? PALMA_PORT_COORDS : DEPOT_COORDS;
    const endC = sal.destinoTrayecto === 'puerto' ? PALMA_PORT_COORDS : DEPOT_COORDS;
    const stops = sal.items.map((it, i) => ({ it, n: i + 1, coords: getStopCoords(it) }));
    const located = stops.filter(s => s.coords);
    const unlocated = stops.filter(s => !s.coords);
    nStops += stops.length; nUnlocated += unlocated.length;
    const wp = [startC].concat(located.map(s => s.coords), [endC]);
    let geom = null;
    if(useRoad && located.length){
      const key = orsRouteKey(wp);
      geom = orsCache.geom[key] || null;
      if(!geom){ queueOrsGeom(key, wp); nRoadPending++; }
    }
    const line = L.polyline(geom || wp, { color, weight: geom ? 4 : 3, opacity: 0.85, dashArray: geom ? null : '6 6' }).addTo(dmLayer);
    const bounds = L.latLngBounds(wp);
    dmBoundsList.push(bounds);
    wp.forEach(c => allPts.push(c));
    const hora = sal.items[0] && sal.items[0].horaSalida;
    const dur = calcSalidaDuration(sal);
    const salLabel = `${ORDINALS[sal.salida - 1] || sal.salida + 'ª'} sal.`;
    located.forEach(s => {
      const k = s.coords[0].toFixed(4) + ',' + s.coords[1].toFixed(4);
      const c = (seen[k] = (seen[k] || 0) + 1) - 1;
      let pos = s.coords;
      if(c > 0){ const ang = c * 2.4, r = 0.0018 * Math.sqrt(c); pos = [s.coords[0] + r * Math.sin(ang), s.coords[1] + r * Math.cos(ang) / 0.78]; }
      const it = s.it;
      const shape = sal.type === 'cisterna' ? 'border-radius:4px;' : 'border-radius:50%;';
      const arts = (it.articles || []).map(a => getArticleName(a.artCod) + (a.qty ? ' ×' + a.qty : '')).join('<br>');
      const m = L.marker(pos, { icon: L.divIcon({ className: '', iconSize: [22, 22], iconAnchor: [11, 11],
        html: `<div style="width:22px;height:22px;line-height:20px;text-align:center;font-size:11px;font-weight:700;color:#fff;background:${color};${shape}border:2px solid #fff;box-shadow:0 0 3px rgba(0,0,0,0.6);opacity:${it.servido ? 0.45 : 1};">${it.servido ? '✓' : s.n}</div>` }) });
      m.bindTooltip(`<strong>${escapeHtml(omOrderLabel(it))}</strong><br>${dr ? dr.name : '—'} · ${salLabel}${hora ? ' · ' + hora : ''}${vh ? ' · ' + vh.name : ''}<br>${escapeHtml(it.muni || '')}${it.direccion ? '<br>' + escapeHtml(it.direccion) : ''}${sal.type === 'cisterna' ? '<br>Cuba: ' + escapeHtml(it.cisternaCuba || 'sin asignar') : ''}${arts ? '<br>' + arts : ''}${it.servido ? '<br><em>servido</em>' : ''}`);
      m.on('click', () => { closeDayMap(); openEdit(it.id); });
      m.addTo(dmLayer);
    });
    cards.push({ idx: dmBoundsList.length - 1, color, dr, vh, sal, salLabel, hora, dur, stops, unlocated, typeLabel: sal.type === 'cisterna' ? 'Cisterna' : 'Reparto' });
  });
  arrastres.forEach(s => {
    const color = dmColor(s.driver);
    const tray = arrastreTrayectoField(s);
    const a = tray === 'puerto_base' ? PALMA_PORT_COORDS : DEPOT_COORDS;
    const b = tray === 'puerto_base' ? DEPOT_COORDS : PALMA_PORT_COORDS;
    L.polyline([a, b], { color, weight: 3, opacity: 0.9, dashArray: '2 8' }).addTo(dmLayer);
    const mid = [(a[0] + b[0]) / 2, (a[1] + b[1]) / 2];
    const dr = DRIVER_BY_ID.get(s.driver);
    L.marker(mid, { icon: L.divIcon({ className: '', iconSize: [22, 22], iconAnchor: [11, 11], html: `<div style="width:22px;height:22px;line-height:20px;text-align:center;font-size:12px;background:#fff;border:2px solid ${color};border-radius:4px;">⛴</div>` }) })
      .bindTooltip(`Arrastre ${arrastreTrayectoLabel(tray)}<br>${dr ? dr.name : '—'}${s.horaSalida ? ' · ' + s.horaSalida : ''}${s.cisternaCuba ? '<br>Cuba ' + escapeHtml(s.cisternaCuba) : ''}`)
      .on('click', () => { closeDayMap(); openEditArrastre(s.id); }).addTo(dmLayer);
    allPts.push(a, b);
  });
  if(fit && allPts.length) dmMap.fitBounds(L.latLngBounds(allPts).pad(0.12));
  // panel lateral
  const panel = document.getElementById('dm-panel');
  const head = `<div style="font-size:12px;margin-bottom:6px;"><strong>${salidas.length}</strong> salida${salidas.length === 1 ? '' : 's'} · <strong>${nStops}</strong> pedido${nStops === 1 ? '' : 's'}${arrastres.length ? ' · <strong>' + arrastres.length + '</strong> arrastre' + (arrastres.length === 1 ? '' : 's') : ''}${nUnlocated ? ` · <span style="color:#791F1F;">${nUnlocated} sin ubicar</span>` : ''}${nRoadPending ? ` · <span style="opacity:0.7;">trazando ${nRoadPending} ruta(s) por carretera…</span>` : ''}</div>`;
  const cardsHtml = cards.sort((a, b) => (a.hora || '99').localeCompare(b.hora || '99')).map(c => `
    <div style="border:0.5px solid var(--color-border-tertiary);border-left:5px solid ${c.color};border-radius:var(--border-radius-md);padding:5px 8px;margin-bottom:6px;cursor:pointer;" onclick="dmFocus(${c.idx})">
      <div style="font-size:12px;"><strong>${c.dr ? c.dr.name : '—'}</strong> · ${c.salLabel} · ${c.typeLabel}${c.vh ? ' · ' + c.vh.name : ''}</div>
      <div style="font-size:11px;opacity:0.75;">${c.hora ? c.hora + '→' + (addMinutes(c.hora, c.dur.totalMin) || '?') + ' · ' : ''}${formatDuration(c.dur.totalMin)} · ${c.dur.km} km${c.dur.missingCoords ? ' · ⚠' : ''}</div>
      ${c.stops.map(s => `<div style="font-size:11px;${s.it.servido ? 'text-decoration:line-through;opacity:0.55;' : ''}">${s.n}. ${escapeHtml(omOrderLabel(s.it))} <span style="opacity:0.6;">${escapeHtml(s.it.muni || '')}</span>${s.coords ? '' : ' <span style="color:#791F1F;">⚠ sin ubicar</span>'}${c.sal.type === 'cisterna' && !s.it.cisternaCuba ? ' <span style="color:#791F1F;">⚠ sin cuba</span>' : ''}</div>`).join('')}
    </div>`).join('');
  panel.innerHTML = head + (cardsHtml || '<div style="font-size:12px;color:var(--color-text-tertiary);">No hay salidas asignadas este día.</div>');
}

// ==================================================================================
// ---- ARRASTRAR Y SOLTAR PEDIDOS: reasigna conductor, día, vehículo, salida y hora ----
// ==================================================================================
let currentDrag = null; // {kind:'order'|'unassigned'|..., id}
function showUnassignZones(){ document.querySelectorAll('.unassign-drop').forEach(z => { z.style.display = 'block'; }); }
function hideUnassignZones(){ document.querySelectorAll('.unassign-drop').forEach(z => { z.style.display = 'none'; }); }
document.addEventListener('dragend', () => { currentDrag = null; hideUnassignZones(); });

function orderIsDraggable(kind){ return currentDrag && (currentDrag.kind === 'order' || currentDrag.kind === 'unassigned') && (kind == null || currentDrag.kind === kind); }
function highlightDrop(el, on){ el.style.boxShadow = on ? '0 0 0 2px #185FA5' : ''; }

// Mantiene alineada la ficha de la cuba (fecha de entrega y conductor) con el pedido movido
function syncCisternaForService(s){
  if(s.type !== 'cisterna' || !s.cisternaCuba) return false;
  const c = cisternaStock.find(x => cisternaKey(x.cuba) === cisternaKey(s.cisternaCuba));
  if(!c) return false;
  c.fechaEntregaAsignada = s.day;
  c.choferAsignado = DRIVER_BY_ID.get(s.driver)?.name || '';
  return true;
}
function orderLabelShort(s){ return `${s.pedidoNum ? '#' + s.pedidoNum + ' ' : ''}${s.client || 'Pedido'}`; }
function finishOrderMove(s, summary, day, cisternaTouched){
  saveData('reordenacion', { summary, day, pedidoNum: s.pedidoNum, type: s.type });
  if(cisternaTouched) writeCisternaStock();
  renderCalendar();
  if(cisternaTouched) renderCisternaFleetIfOpen();
}

// Suelta un pedido (asignado o sin asignar) dentro de una salida existente:
// hereda día, conductor, vehículo, número de salida, hora, 2ª persona y trayecto de esa salida.
function moveOrderToSalida(id, day, driver, salida, beforeId){
  currentDrag = null; hideUnassignZones();
  const s = services.find(x => x.id === id);
  if(!s || s.type === 'arrastre') return;
  if(s.day === day && s.driver === driver && s.salida === salida) return;
  const group = services.filter(x => x.day === day && x.driver === driver && x.salida === salida && x.id !== id && x.type !== 'arrastre')
    .sort((a, b) => (a.itemOrder || 0) - (b.itemOrder || 0));
  if(!group.length) return;
  const ref = group[0];
  if(s.type !== ref.type && (s.type === 'cisterna' || ref.type === 'cisterna')){
    alert('No se pueden mezclar cisternas y repartos en la misma salida.'); return;
  }
  const drName = DRIVER_BY_ID.get(driver)?.name || 'el conductor';
  if(s.type === 'cisterna'){
    const n = services.filter(x => x.day === day && x.type === 'cisterna' && x.driver === driver && x.id !== id).length;
    if(n >= 2 && !confirm(`${drName} ya tiene ${n} cisternas ese día (máx. 2). ¿Añadir esta de todas formas?`)) return;
  }
  if(ref.horaSalida && ref.twoPerson && ref.driver2){
    const dur = calcSalidaDuration({ items: group.concat([s]), origenTrayecto: ref.origenTrayecto, destinoTrayecto: ref.destinoTrayecto }).totalMin;
    const end = addMinutes(ref.horaSalida, dur);
    const c = getPersonBusyIntervals(day, ref.driver2, driver, salida).find(iv => intervalsOverlap(ref.horaSalida, end, iv.start, iv.end));
    if(c && !confirm(`${DRIVER_BY_ID.get(ref.driver2)?.name || 'El ayudante'} (ayudante) ya tiene otra salida entre ${c.start} y ${c.end}. ¿Continuar?`)) return;
  }
  const old = { day: s.day, driver: s.driver, salida: s.salida };
  const hadDriver = !!s.driver;
  Object.assign(s, {
    day, driver, salida, vehicle: ref.vehicle, horaSalida: ref.horaSalida || null,
    twoPerson: ref.twoPerson || false, driver2: ref.driver2 || null,
    origenTrayecto: ref.origenTrayecto || 'base', destinoTrayecto: ref.destinoTrayecto || 'base',
    assigned: true
  });
  if(!s.zones || !s.zones.length) s.zones = ref.zones || [];
  const list = group.slice();
  const idx = beforeId != null ? list.findIndex(x => x.id === beforeId) : -1;
  if(idx >= 0) list.splice(idx, 0, s); else list.push(s);
  list.forEach((x, i) => { x.itemOrder = i; });
  if(hadDriver) compactDriverSalidas(old.day, old.driver, old.salida);
  cascadeSalidaTimes(s.day, s.driver, s.salida); // desplaza las salidas siguientes si ahora se solapan
  const cis = syncCisternaForService(s);
  finishOrderMove(s, `${orderLabelShort(s)} movido a ${drName} (${ORDINALS[s.salida - 1] || s.salida + 'ª'} salida)`, day, cis);
}

// Suelta un pedido asignado en OTRO día: se crea una salida nueva para el mismo conductor,
// a continuación de la última que tenga ese día (o a primera hora si no tiene ninguna).
function moveOrderToDay(id, newDay){
  currentDrag = null; hideUnassignZones();
  const s = services.find(x => x.id === id);
  if(!s || s.type === 'arrastre' || s.day === newDay) return;
  if(!s.driver){ // sin asignar: solo cambia el día
    s.day = newDay;
    finishOrderMove(s, `${orderLabelShort(s)} movido de día`, newDay, false);
    return;
  }
  const drName = DRIVER_BY_ID.get(s.driver)?.name || 'el conductor';
  const others = servicesForDay(newDay).filter(x => x.id !== id);
  if(s.type === 'cisterna'){
    const n = others.filter(x => x.type === 'cisterna' && x.driver === s.driver).length;
    if(n >= 2 && !confirm(`${drName} ya tiene ${n} cisternas el ${newDay} (máx. 2). ¿Moverla de todas formas?`)) return;
  }
  const maxS = getMaxSalidaForDriver(newDay, s.driver, others);
  const prevEnd = maxS > 0 ? getSalidaEndTime(newDay, s.driver, maxS) : null;
  const old = { day: s.day, driver: s.driver, salida: s.salida };
  s.day = newDay;
  s.salida = nextSalidaNumberForDriver(newDay, s.driver, s.id);
  s.horaSalida = prevEnd || addMinutes(ROUTE_WINDOW_START, ROUTE_CARGA_MIN);
  s.itemOrder = 0; s.twoPerson = false; s.driver2 = null;
  compactDriverSalidas(old.day, old.driver, old.salida);
  const cis = syncCisternaForService(s);
  finishOrderMove(s, `${orderLabelShort(s)} movido al ${newDay} con ${drName} (${s.horaSalida})`, newDay, cis);
}

// Suelta un pedido asignado en la franja "sin asignar": vuelve a la bolsa de pendientes de ese día.
function unassignOrder(id, newDay){
  currentDrag = null; hideUnassignZones();
  const s = services.find(x => x.id === id);
  if(!s || s.type === 'arrastre' || !s.driver) return;
  const old = { day: s.day, driver: s.driver, salida: s.salida };
  Object.assign(s, { driver: null, vehicle: null, salida: 1, horaSalida: null, twoPerson: false, driver2: null, itemOrder: 0, day: newDay || s.day, imported: true, servido: false });
  compactDriverSalidas(old.day, old.driver, old.salida);
  const cis = syncCisternaForService(s);
  finishOrderMove(s, `${orderLabelShort(s)} desasignado`, s.day, cis);
}
function buildUnassignZone(dayKey){
  const z = document.createElement('div');
  z.className = 'unassign-drop';
  z.textContent = '⬇ Soltar aquí para dejar sin asignar';
  z.style.cssText = 'display:none;font-size:11px;text-align:center;padding:6px;border:1.5px dashed #E69F00;border-radius:5px;background:#FFF8ED;color:#633806;';
  z.addEventListener('dragover', e => { if(!orderIsDraggable('order')) return; e.preventDefault(); e.stopPropagation(); e.dataTransfer.dropEffect = 'move'; highlightDrop(z, true); });
  z.addEventListener('dragleave', () => highlightDrop(z, false));
  z.addEventListener('drop', e => {
    if(!orderIsDraggable('order')) return;
    e.preventDefault(); e.stopPropagation();
    highlightDrop(z, false);
    unassignOrder(currentDrag.id, dayKey);
  });
  return z;
}

// ---- PUNTOS DE REFERENCIA EDITABLES: Base y Puerto ----
const REF_POINTS_KEY = 'planificador_ref_points_v1';
let refMarkerList = [];
function registerRefMarker(coordsName, marker){ refMarkerList.push({ key: coordsName === 'DEPOT_COORDS' ? 'BASE' : 'PUERTO', marker }); }
function refCoordsArray(key){ return key === 'PUERTO' ? PALMA_PORT_COORDS : DEPOT_COORDS; }
async function loadRefPoints(){
  try{
    const r = await storageAdapter.get(REF_POINTS_KEY);
    if(r && r.value){
      const d = JSON.parse(r.value);
      ['BASE', 'PUERTO'].forEach(k => { if(d[k] && inMallorca(d[k])){ const a = refCoordsArray(k); a[0] = d[k][0]; a[1] = d[k][1]; } });
      orsPointIndex = null;
    }
  }catch(e){}
}
function saveRefPoints(){
  try{ storageAdapter.set(REF_POINTS_KEY, JSON.stringify({ BASE: DEPOT_COORDS.slice(), PUERTO: PALMA_PORT_COORDS.slice() })); }catch(e){}
}
function setRefPoint(key, coords){
  const a = refCoordsArray(key);
  a[0] = coords[0]; a[1] = coords[1];
  saveRefPoints();
  orsPointIndex = null;
  refMarkerList.filter(m => m.key === key).forEach(m => m.marker.setLatLng(coords));
  renderCalendar();
  ensureOrsData(true); // recalcula la matriz de distancias con el punto nuevo (si hay clave de ORS)
  dmRefreshIfOpen(); omRefreshIfOpen();
}
function openRefPointPicker(){
  const v = prompt('¿Qué punto quieres situar en el mapa?\n\n1) Puerto de Palma (muelle de carga/descarga de las cisternas)\n2) Base (nave de salida)', '1');
  const key = v === '1' ? 'PUERTO' : v === '2' ? 'BASE' : null;
  if(!key) return;
  const c = refCoordsArray(key);
  openPinPicker({ refKey: key, title: key === 'PUERTO' ? 'Situar el Puerto de Palma (punto de carga/descarga)' : 'Situar la Base', searchText: key === 'PUERTO' ? 'Puerto de Palma, Mallorca' : 'Polígono Son Castelló, Palma', current: c.slice(), onSave: () => {} });
}

// ---- GESTOR DE POBLACIONES LOCALIZADAS (corregir una localización equivocada) ----
async function geocodeCandidates(text){
  const apiKey = getOrsKey();
  if(!apiKey) throw new Error('Falta la clave de ORS');
  const url = `${ORS_BASE}/geocode/search?api_key=${encodeURIComponent(apiKey)}`
    + `&text=${encodeURIComponent(text)}&boundary.country=ES&focus.point.lon=2.95&focus.point.lat=39.6&size=6`;
  const res = await fetch(url);
  if(!res.ok) throw new Error('HTTP ' + res.status);
  const data = await res.json();
  return (data.features || []).map(f => ({ label: f.properties.label, accuracy: f.properties.accuracy, confidence: f.properties.confidence, coords: [f.geometry.coordinates[1], f.geometry.coordinates[0]] }));
}
function orsPlaceRows(){
  const names = new Map(); // key -> nombre a mostrar
  services.forEach(s => { const m = (s.muni || '').trim(); if(m && !names.has(normalizeStr(m))) names.set(normalizeStr(m), m); });
  Object.keys(orsCache.geo).forEach(k => { if(!names.has(k)) names.set(k, k); });
  return Array.from(names.entries()).map(([key, name]) => {
    const coords = getMuniCoords(name);
    const source = orsCache.geo[key] ? 'ORS / manual' : (coords ? 'tabla interna' : 'sin localizar');
    return { key, name, coords, source };
  }).sort((a, b) => a.name.localeCompare(b.name));
}
function openOrsPlaces(){ renderOrsPlaces(); document.getElementById('ors-places-modal').classList.add('open'); }
function closeOrsPlaces(){ document.getElementById('ors-places-modal').classList.remove('open'); }
function renderOrsPlaces(){
  const rows = orsPlaceRows();
  const q = (t) => String(t).replace(/\\/g, '\\\\').replace(/'/g, "\\'");
  document.getElementById('ors-places-tbody').innerHTML = rows.map(r => {
    const link = r.coords ? `<a href="https://www.openstreetmap.org/?mlat=${r.coords[0]}&mlon=${r.coords[1]}#map=14/${r.coords[0]}/${r.coords[1]}" target="_blank" rel="noopener">ver en mapa</a>` : '—';
    const isGeo = !!orsCache.geo[r.key];
    return `<tr>
      <td>${escapeHtml(r.name)}</td>
      <td>${r.coords ? r.coords[0].toFixed(5) + ', ' + r.coords[1].toFixed(5) : '—'}</td>
      <td>${escapeHtml(r.source)}</td>
      <td>${link}</td>
      <td style="white-space:nowrap;">
        <button class="btn-secondary" style="padding:2px 6px;font-size:11px;" onclick="orsPickPlace('${q(r.key)}','${q(r.name)}')">Buscar otra</button>
        <button class="btn-secondary" style="padding:2px 6px;font-size:11px;" onclick="orsEditCoords('${q(r.key)}')">Coordenadas</button>
        ${isGeo ? `<button class="btn-secondary" style="padding:2px 6px;font-size:11px;" onclick="orsResetPlace('${q(r.key)}')">Quitar</button>` : ''}
      </td>
    </tr>`;
  }).join('') || '<tr><td colspan="5">Aún no hay poblaciones.</td></tr>';
}
function applyOrsPlace(key, coords){
  orsCache.geo[key] = coords;
  orsPointIndex = null;
  saveOrsCache();
  renderOrsPlaces();
  renderCalendar();
  ensureOrsData(true); // recalcula la matriz con el punto corregido (si hay clave)
}
async function orsPickPlace(key, name){
  const query = prompt('Texto a buscar (sé más específico si hace falta, p. ej. "Sa Pobla, Mallorca"):', name);
  if(!query) return;
  try{
    const cands = await geocodeCandidates(query);
    if(!cands.length){ alert('Sin resultados para "' + query + '".'); return; }
    const list = cands.map((c, i) => `${i + 1}) ${c.label}`).join('\n');
    const pick = prompt(`Resultados para "${query}":\n\n${list}\n\nEscribe el número del correcto (o deja vacío para cancelar):`);
    const n = parseInt(pick, 10);
    if(!n || n < 1 || n > cands.length) return;
    applyOrsPlace(key, cands[n - 1].coords);
  }catch(e){ alert('No se pudo buscar: ' + e.message); }
}
function orsEditCoords(key){
  const cur = orsCache.geo[key] || null;
  const v = prompt('Coordenadas "latitud, longitud" (p. ej. 39.7669, 3.0214). En Google Maps: clic derecho sobre el punto y pulsa las coordenadas para copiarlas.', cur ? cur.join(', ') : '');
  if(!v) return;
  const m = v.replace(/,/g, ' ').trim().split(/\s+/).map(Number);
  if(m.length !== 2 || m.some(isNaN) || Math.abs(m[0]) > 90 || Math.abs(m[1]) > 180){ alert('Formato no válido.'); return; }
  applyOrsPlace(key, [m[0], m[1]]);
}
function orsResetPlace(key){
  if(!confirm('Se quita la corrección. Si la población no está en la tabla interna, se volverá a geocodificar automáticamente y puede salir el mismo resultado. ¿Continuar?')) return;
  delete orsCache.geo[key];
  orsPointIndex = null;
  saveOrsCache();
  renderOrsPlaces();
  renderCalendar();
  ensureOrsData(true);
}

// Calculate descent time in minutes for an item
function calcDescargaMin(item) {
  if(item.merchManual != null && item.merchManual !== '') return Number(item.merchManual);
  const qty = Number(item.merchQty) || 1;
  switch(item.merchType) {
    case 'cisterna':    return 180;
    case 'palet_grg':   return 10 * qty;
    case 'grg_trasvase':return 20 * qty;
    default:            return 0;
  }
}
 
// Calculate total route duration in minutes for a salida
// Returns { totalMin, driveMin, descargaMin, km, stops, breakdown, missingCoords }
function calcSalidaDuration(sal) {
  const stops = sal.items.map(i => ({
    coords: getStopCoords(i),
    pinned: !!getClientPin(i.client),
    descarga: calcDescargaMin(i),
    client: i.client,
    muni: i.muni
  }));
 
  const origenTrayecto = sal.origenTrayecto || sal.items[0]?.origenTrayecto || 'base';
  const destinoTrayecto = sal.destinoTrayecto || sal.items[0]?.destinoTrayecto || 'base';
  const startCoords = origenTrayecto === 'puerto' ? PALMA_PORT_COORDS : DEPOT_COORDS;
  const startId = origenTrayecto === 'puerto' ? 'PUERTO' : 'BASE';
  const endCoords = destinoTrayecto === 'puerto' ? PALMA_PORT_COORDS : DEPOT_COORDS;
 
  let km = 0;
  let driveMin = 0;
  let lastCoords = startCoords;
  let lastId = startId;
  // Si alguna parada tiene ubicación exacta, se usa la ruta real de toda la secuencia (1 petición, en caché)
  let routeLegs = null, legIdx = 0;
  if(stops.some(s => s.pinned && s.coords)){
    const wp = [startCoords].concat(stops.filter(s => s.coords).map(s => s.coords), [endCoords]);
    const rk = orsRouteKey(wp);
    routeLegs = orsCache.routes[rk] || null;
    if(!routeLegs && sal.driver) queueOrsRoute(rk, wp);
  }
  const legFromRoute = (fallback) => {
    const r = routeLegs && routeLegs[legIdx++];
    return r ? { km: r[0], min: r[1], real: true } : (legIdx++, fallback());
  };
  let lastMuni = null;
  let hasReturn = false;
  const breakdown = [];
 
  stops.forEach(s => {
    if(s.coords) {
      const sId = s.pinned ? 'P:' + normalizeStr(s.client) : orsIdFor(s.coords, s.muni);
      const leg = routeLegs ? legFromRoute(() => routeLegSync(lastId, lastCoords, sId, s.coords)) : routeLegSync(lastId, lastCoords, sId, s.coords);
      const legKm = leg.km;
      let legMin = leg.min;
      if(lastMuni && s.muni && normalizeStr(s.muni) === normalizeStr(lastMuni)) legMin = Math.max(legMin, SAME_MUNI_MIN);
      km += legKm;
      driveMin += legMin;
      breakdown.push({ client: s.client, muni: s.muni, legKm: Math.round(legKm*10)/10, legMin, descargaMin: s.descarga });
      lastCoords = s.coords;
      lastMuni = s.muni;
      lastId = sId;
      hasReturn = true;
    } else {
      breakdown.push({ client: s.client, muni: s.muni, legKm: null, legMin: null, descargaMin: s.descarga });
    }
  });
 
  // Trayecto de vuelta: a Base o al Puerto, según lo indicado en el pedido.
  if(hasReturn) {
    const endId = destinoTrayecto === 'puerto' ? 'PUERTO' : 'BASE';
    const retLeg = routeLegs ? legFromRoute(() => routeLegSync(lastId, lastCoords, endId, endCoords)) : routeLegSync(lastId, lastCoords, endId, endCoords);
    const retKm = retLeg.km;
    km += retKm;
    driveMin += retLeg.min;
  }
 
  const descMin = stops.reduce((t, s) => t + s.descarga, 0);
  return {
    totalMin: driveMin + descMin,
    driveMin,
    descargaMin: descMin,
    km: Math.round(km),
    stops: breakdown,
    missingCoords: stops.some(s => !s.coords)
  };
}
 
function formatDuration(min) {
  if(!min) return '—';
  const h = Math.floor(min/60), m = min%60;
  return h > 0 ? `${h}h ${m>0?m+'min':''}`.trim() : `${m}min`;
}
 
// Duración estimada de un arrastre independiente Base⇄Puerto (no lleva descarga, solo trayecto).
function calcArrastreDuration(){
  const leg=routeLegSync('BASE',DEPOT_COORDS,'PUERTO',PALMA_PORT_COORDS);
  const km=leg.km;
  return {km:Math.round(km),min:leg.min};
}
 
function addMinutes(timeStr, min) {
  if(!timeStr) return null;
  const [h, m] = timeStr.split(':').map(Number);
  const total = h*60 + m + min;
  return `${String(Math.floor(total/60)%24).padStart(2,'0')}:${String(total%60).padStart(2,'0')}`;
}
 function buildSalidaObj(dayKey,driverId,salidaNum){
  const items=services.filter(s=>s.day===dayKey&&s.driver===driverId&&s.salida===salidaNum)
    .sort((a,b)=>(a.itemOrder||0)-(b.itemOrder||0));
  if(!items.length)return null;
  return {driver:driverId,salida:salidaNum,type:items[0].type,vehicle:items[0].vehicle,items};
}
function getSalidaEndTime(dayKey,driverId,salidaNum){
  const sal=buildSalidaObj(dayKey,driverId,salidaNum);
  const hora=sal?.items[0]?.horaSalida;
  if(!sal||!hora)return null;
  return addMinutes(hora,calcSalidaDuration(sal).totalMin);
}
function cascadeSalidaTimes(dayKey,driverId,fromSalida){
  const max=getMaxSalidaForDriver(dayKey,driverId);
  for(let s=fromSalida+1;s<=max;s++){
    const prevEnd=getSalidaEndTime(dayKey,driverId,s-1);
    if(!prevEnd)break;
    const sal=buildSalidaObj(dayKey,driverId,s);
    if(!sal||!sal.items[0]?.horaSalida)break;
    if(sal.items[0].horaSalida<prevEnd) sal.items.forEach(it=>{it.horaSalida=prevEnd;});
  }
}
 
function getPersonSalidasForDay(dayKey, personId){
  const map={};
  // Los arrastres Base⇄Puerto ahora ocupan un número de salida real dentro de la secuencia
  // del conductor, así que cuentan aquí igual que cualquier otra salida (para detectar solapes).
  services.filter(s=>s.day===dayKey&&(s.driver===personId||(s.twoPerson&&s.driver2===personId)))
    .forEach(s=>{
      const k=`${s.driver}-${s.salida}`;
      if(!map[k])map[k]={driver:s.driver,salida:s.salida,type:s.type,vehicle:s.vehicle,items:[]};
      map[k].items.push(s);
    });
  return Object.values(map);
}
function getPersonBusyIntervals(dayKey, personId, excludeDriver, excludeSalida){
  return getPersonSalidasForDay(dayKey,personId)
    .filter(sal=>!(sal.driver===excludeDriver&&sal.salida===excludeSalida))
    .map(sal=>{
      const hora=sal.items[0]?.horaSalida;
      if(!hora)return null;
      return {start:hora,end:addMinutes(hora,calcSalidaDuration(sal).totalMin),driver:sal.driver,salida:sal.salida};
    }).filter(Boolean);
}
function intervalsOverlap(startA,endA,startB,endB){ return startA<endB && startB<endA; }
function buildProspectiveSalida(dayKey,driverId,salidaNum,excludeId,newItem){
  const items=services.filter(s=>s.day===dayKey&&s.driver===driverId&&s.salida===salidaNum&&s.id!==excludeId)
    .sort((a,b)=>(a.itemOrder||0)-(b.itemOrder||0));
  items.push(newItem);
  return {items};
}
 
// ---- STORAGE ----
function openDatabase(){
  if(!('indexedDB' in window))return Promise.resolve(null);
  if(dbPromise)return dbPromise;
  dbPromise=new Promise((resolve,reject)=>{
    const req=indexedDB.open(DB_NAME,DB_VERSION);
    req.onupgradeneeded=event=>{
      const db=event.target.result;
      if(!db.objectStoreNames.contains(CURRENT_STORE))db.createObjectStore(CURRENT_STORE,{keyPath:'key'});
      if(!db.objectStoreNames.contains(HISTORY_STORE)){
        const history=db.createObjectStore(HISTORY_STORE,{keyPath:'id',autoIncrement:true});
        history.createIndex('createdAt','createdAt',{unique:false});
        history.createIndex('day','day',{unique:false});
        history.createIndex('pedidoNum','pedidoNum',{unique:false});
        history.createIndex('action','action',{unique:false});
      }
    };
    req.onsuccess=()=>resolve(req.result);
    req.onerror=()=>reject(req.error);
  });
  return dbPromise;
}
 
function idbRequest(request){
  return new Promise((resolve,reject)=>{
    request.onsuccess=()=>resolve(request.result);
    request.onerror=()=>reject(request.error);
  });
}
 
function idbTx(db,storeName,mode,work){
  return new Promise((resolve,reject)=>{
    const tx=db.transaction(storeName,mode);
    const store=tx.objectStore(storeName);
    let result;
    tx.oncomplete=()=>resolve(result);
    tx.onerror=()=>reject(tx.error);
    tx.onabort=()=>reject(tx.error);
    result=work(store);
  });
}
 
function cloneServices(){
  return JSON.parse(JSON.stringify(services));
}
 
function getHistorySummary(action,details){
  const createdAt=new Date().toISOString();
  const base={action,serviceCount:services.length,createdAt,updatedAt:createdAt,snapshot:cloneServices()};
  if(details&&typeof details==='object')Object.assign(base,details);
  return base;
}
 
async function readCurrentServices(){
  if(window.planificadorServerStorage){
    const stored=await storageAdapter.get(STORAGE_KEY);
    if(stored&&stored.value)return JSON.parse(stored.value);
  }
  const db=await openDatabase();
  if(db){
    const current=await idbTx(db,CURRENT_STORE,'readonly',store=>idbRequest(store.get(STORAGE_KEY)));
    if(current&&Array.isArray(current.services)){
      if(window.planificadorServerStorage)try{await storageAdapter.set(STORAGE_KEY,JSON.stringify(current.services));}catch(error){window.planificadorStorageError=error?.message||'No se pudieron migrar los repartos al servidor.';}
      return current.services;
    }
  }
  const r=await storageAdapter.get(STORAGE_KEY);
  if(r&&r.value)return JSON.parse(r.value);
  return [];
}
 
async function writeCurrentServices(){
  const value=JSON.stringify(services);
  let remoteError=null;
  if(window.planificadorServerStorage){
    try{await storageAdapter.set(STORAGE_KEY,value);}catch(error){remoteError=error;}
  }
  const db=await openDatabase();
  if(db){
    await idbTx(db,CURRENT_STORE,'readwrite',store=>{
      store.put({key:STORAGE_KEY,services:JSON.parse(value),updatedAt:new Date().toISOString()});
    });
  }
  if(remoteError)throw remoteError;
  if(!window.planificadorServerStorage)await storageAdapter.set(STORAGE_KEY,value);
}
 
async function addHistoryEntry(action='cambio',details={}){
  const db=await openDatabase();
  const entry=getHistorySummary(action,details);
  if(db)await idbTx(db,HISTORY_STORE,'readwrite',store=>store.add(entry));
  if(window.planificadorServerStorage){
    const stored=await storageAdapter.get(HISTORY_STORAGE_KEY);
    const existing=stored&&stored.value?JSON.parse(stored.value):(await readLocalHistoryEntries(300)).map(summarizeHistoryEntry);
    if(!existing.some(item=>item.createdAt===entry.createdAt))existing.push(summarizeHistoryEntry(entry));
    const history=existing.slice(-300);
    await storageAdapter.set(HISTORY_STORAGE_KEY,JSON.stringify(history));
  }
}
 
async function replaceHistoryEntries(entries){
  const db=await openDatabase();
  if(db)await idbTx(db,HISTORY_STORE,'readwrite',store=>{
      store.clear();
      (entries||[]).forEach(entry=>store.add(entry));
    });
  if(window.planificadorServerStorage){
    await storageAdapter.set(HISTORY_STORAGE_KEY,JSON.stringify((entries||[]).map(summarizeHistoryEntry).slice(-300)));
  }
}
 
function summarizeHistoryEntry({snapshot,...entry}){return entry;}

async function readLocalHistoryEntries(limit=200){
  const db=await openDatabase();
  if(!db)return [];
  return new Promise((resolve,reject)=>{
    const tx=db.transaction(HISTORY_STORE,'readonly');
    const store=tx.objectStore(HISTORY_STORE);
    const entries=[];
    const req=store.openCursor();
    req.onsuccess=event=>{
      const cursor=event.target.result;
      if(cursor){
        const entry=cursor.value;
        if(!entry.createdAt)entry.createdAt=entry.updatedAt||new Date(0).toISOString();
        entries.push(entry);
        cursor.continue();
      }
    };
    tx.oncomplete=()=>resolve(entries.sort((a,b)=>String(b.createdAt||'').localeCompare(String(a.createdAt||''))).slice(0,limit));
    tx.onerror=()=>reject(tx.error);
  });
}

async function getHistoryEntries(limit=200){
  const local=await readLocalHistoryEntries(100000);
  if(!window.planificadorServerStorage)return local.slice(0,limit);
  const stored=await storageAdapter.get(HISTORY_STORAGE_KEY);
  let remote=stored&&stored.value?JSON.parse(stored.value):[];
  if(!remote.length&&local.length){
    remote=local.slice(-300).map(summarizeHistoryEntry);
    await storageAdapter.set(HISTORY_STORAGE_KEY,JSON.stringify(remote));
  }
  const merged=new Map(remote.map(entry=>[entry.createdAt||entry.updatedAt,entry]));
  local.forEach(entry=>merged.set(entry.createdAt||entry.updatedAt,{...merged.get(entry.createdAt||entry.updatedAt),...entry}));
  return [...merged.values()].sort((a,b)=>String(b.createdAt||'').localeCompare(String(a.createdAt||''))).slice(0,limit);
}
 
async function readBackupData(includeSnapshots){
  const history=await getHistoryEntries(100000);
  return {
    app:'planificador_reparto',
    version:1,
    exportedAt:new Date().toISOString(),
    services:cloneServices(),
    clientPins:{pins:clientPins,tried:pinsTried},
    cisternaStock:JSON.parse(JSON.stringify(cisternaStock)),
    productPort:productPort,
    registroMovimientos:JSON.parse(JSON.stringify(registroMovimientos)),
    // Las fotos completas son lo que hace crecer el archivo; se omiten por defecto
    history:includeSnapshots?history:history.map(({snapshot,...rest})=>rest)
  };
} 


// Migración: arrastres guardados antes de que contaran como salida numerada (salida:null) se
// numeran ahora, en orden cronológico por conductor/día, igual que si se hubieran creado nuevos.
function migrateArrastreSalidaNumbers(){
  const groups={};
  services.forEach(s=>{
    if(s.type==='arrastre'&&s.salida==null){
      const key=`${s.day}__${s.driver}`;
      if(!groups[key])groups[key]=[];
      groups[key].push(s);
    }
  });
  const keys=Object.keys(groups);
  if(!keys.length)return 0;
  let migrated=0;
  keys.forEach(key=>{
    const [day,driverStr]=key.split('__');
    const driver=parseInt(driverStr);
    const pending=groups[key].sort((a,b)=>(a.horaSalida||'99:99').localeCompare(b.horaSalida||'99:99')||(a.id-b.id));
    pending.forEach(s=>{
      s.salida=nextSalidaNumberForDriver(day,driver,s.id);
      migrated++;
    });
  });
  return migrated;
}
async function loadData(){
  try{
    services=await readCurrentServices();
    if(!Array.isArray(services))services=[];
    const migrated=migrateArrastreSalidaNumbers();
    await writeCurrentServices();
    const history=await getHistoryEntries(1);
    if(services.length&&!history.length){
      await addHistoryEntry('estado_inicial',{summary:`Estado inicial guardado (${services.length} pedidos)`,affectedCount:services.length});
    }
    if(migrated>0){
      await addHistoryEntry('migracion',{summary:`${migrated} arrastre(s) existente(s) numerado(s) como salida`,affectedCount:migrated});
    }
  }catch(e){
    services=[];
  }
  renderCalendar();
}
 
async function saveData(action='cambio',details={}){
  try{
    await writeCurrentServices();
    await addHistoryEntry(action,details);
    const ind=document.getElementById('save-indicator');
    ind.textContent=window.planificadorServerStorage?'✓ guardado en servidor':'✓ guardado';
    setTimeout(()=>{ind.textContent='';},2000);
  }catch(e){
    const ind=document.getElementById('save-indicator');
    ind.textContent='⚠ error al guardar';
    ind.title=e?.message||'No se pudo guardar el estado del planificador.';
  }
}
 
function servicesByDay(items=services){
  const map=new Map();
  items.forEach(s=>{
    if(!map.has(s.day))map.set(s.day,[]);
    map.get(s.day).push(s);
  });
  return map;
}
function servicesForDay(dayKey,index){return index?index.get(dayKey)||[]:services.filter(s=>s.day===dayKey);}
function countDriverLoad(dayServices,driverId){return dayServices.reduce((total,s)=>{
  if(s.type==='arrastre')return total; // no cuenta para la capacidad de cisternas/día
  // El ayudante (driver2/2º conductor) no cuenta como que él ha "hecho" esa salida — solo el
  // conductor titular. El tiempo del ayudante sigue contando aparte, vía getPersonBusyIntervals.
  return total+((s.driver===driverId)?1:0);
},0);}
 
function getMonday(offset){const today=new Date();const day=today.getDay();const diff=day===0?-6:1-day;const m=new Date(today);m.setDate(today.getDate()+diff+offset*7);m.setHours(0,0,0,0);return m;}
function getDaysForWeek(wo){const m=getMonday(wo);return Array.from({length:7},(_,i)=>{const d=new Date(m);d.setDate(m.getDate()+i);return d;});}
function dateKey(d){return localDateKey(d);}
function isWeekend(d){return d.getDay()===0;}
function isToday(d){return dateKey(d)===dateKey(new Date());}
 
 
// ---- CALENDAR RENDER ----
function getSalidasForDay(dayKey,dayServices){
  const map={};
  // Los bloques de "Arrastre Base⇄Puerto" (type:'arrastre') NO son una salida numerada — se
  // muestran aparte y no deben mezclarse aquí (si no, salen como "undefinedª sal.").
  (dayServices||servicesForDay(dayKey)).filter(s=>s.driver&&s.type!=='arrastre').forEach(s=>{
    const k=`${s.driver}-${s.salida}`;
    if(!map[k])map[k]={driver:s.driver,salida:s.salida,type:s.type,vehicle:s.vehicle,driver2:s.driver2||null,twoPerson:s.twoPerson||false,zones:s.zones||[],origenTrayecto:s.origenTrayecto||'base',destinoTrayecto:s.destinoTrayecto||'base',items:[]};
    map[k].items.push(s);
  });
  // Sort items within each salida by itemOrder
  Object.values(map).forEach(sal => {
    sal.items.sort((a,b) => (a.itemOrder||0) - (b.itemOrder||0));
  });
  return Object.values(map).sort((a,b)=>{
    const ha=a.items[0]?.horaSalida,hb=b.items[0]?.horaSalida;
    if(ha&&hb)return ha.localeCompare(hb);
    if(ha)return -1;
    if(hb)return 1;
    return a.driver!==b.driver?a.driver-b.driver:a.salida-b.salida;
  });
}
function getMaxSalidaForDriver(dayKey,driverId,dayServices){
  let max=0;
  (dayServices||servicesForDay(dayKey)).forEach(s=>{if(s.driver===driverId&&s.salida>max)max=s.salida;});
  return max;
}
function getVehicleForSalida(dayKey,driverId,salidaNum,dayServices){
  const s=(dayServices||servicesForDay(dayKey)).find(x=>x.driver===driverId&&x.salida===salidaNum);
  return s?s.vehicle:null;
}
function suggestDriver(dayKey,type){
  const eligible=type==='cisterna'?DRIVERS.filter(d=>d.canTractora):DRIVERS;
  const dayServices=servicesForDay(dayKey);
  const loads=eligible.map(d=>({driver:d,count:countDriverLoad(dayServices,d.id)}));
  loads.sort((a,b)=>a.count-b.count);return loads[0]?.driver||null;
}
 
 
// ---- MERCH / DESCARGA FORM ----
function onMerchTypeChange() {
  const type = document.getElementById('f-merch-type').value;
  const isManual = type === 'manual';
  const isCisterna = type === 'cisterna';
  document.getElementById('row-merch-qty').style.display = (!isManual && !isCisterna && type) ? '' : 'none';
  document.getElementById('row-merch-manual').style.display = isManual ? '' : 'none';
  if(!isManual&&!isCisterna&&type){
    const qtyEl=document.getElementById('f-merch-qty');
    if(!qtyEl.value)qtyEl.value=1;
  }
  updateMerchPreview();
}
 
function updateMerchPreview() {
  const type = document.getElementById('f-merch-type').value;
  const qty = Number(document.getElementById('f-merch-qty').value) || 1;
  const manual = Number(document.getElementById('f-merch-manual').value) || 0;
  const prev = document.getElementById('merch-preview');
  let min = 0;
  if(type === 'cisterna') min = 180;
  else if(type === 'palet_grg') min = 10 * qty;
  else if(type === 'grg_trasvase') min = 20 * qty;
  else if(type === 'manual') min = manual;
  prev.textContent = min > 0 ? `Tiempo descarga estimado: ${formatDuration(min)}` : '';
}
 
 
// ---- IMPORT ----
// Procesa un ArrayBuffer de Excel (pedidos + hojas de cubas/movimientos)
function processExcelBuffer(buf,opts){
  const silent=!!(opts&&opts.silent);
  const wb=XLSX.read(new Uint8Array(buf),{type:'array'});
  const flotaResult=handleImportFlota(wb);
  const registroResult=handleImportRegistroMovimientos(wb);
  let rows=null;
  try{rows=sheetToImportRows(wb.Sheets[wb.SheetNames[0]]);}catch(e){rows=null;}
  const hasPedidos=!!(rows&&rows.length);
  if(hasPedidos)parseImportRows(rows,silent);
  if(flotaResult||registroResult){
    renderCisternaFleetIfOpen();renderRegistroMovIfOpen();renderCalendar();
  }
  if(!silent){
    if(!hasPedidos&&(flotaResult||registroResult)){
      const parts=[];
      if(flotaResult)parts.push(`Flota de cisternas: ${flotaResult.nuevos} nueva(s), ${flotaResult.actualizados} actualizada(s).`);
      if(registroResult)parts.push(`Registro de arrastres: ${registroResult.nuevos} nuevo(s), ${registroResult.actualizados} actualizado(s).`);
      alert(parts.join('\n'));
    }else if(!hasPedidos){
      alert('No se ha encontrado ni una hoja de pedidos, ni "Situación actual cubas", ni "Registro movimientos" en este archivo.');
    }
  }
  return {pedidos:hasPedidos,flota:flotaResult,registro:registroResult};
}
 
function handleImport(event){
  const file=event.target.files[0];
  if(!file)return;
  if(typeof XLSX==='undefined'){
    alert('No se ha cargado el lector de Excel. Abre el archivo con conexión a internet o usa una versión con la librería Excel incluida.');
    event.target.value='';
    return;
  }
  event.target.value=''; // reset so same file can be re-imported
  const reader=new FileReader();
  reader.onload=function(e){
    try{processExcelBuffer(e.target.result);}
    catch(err){alert('Error al leer el archivo: '+err.message);}
  };
  reader.readAsArrayBuffer(file);
}
 
function parseImportRows(rows,silent){
  // New format: one row per article, same PncNro groups them
  // Key fields: PncNro, PncFecSrv (date to serve), PncFecSrvC (confirmed date),
  //             ArtCod, PncCan, CliNomFis, ObrNom, CliPob, PncObrPob, PncFlgCer
  const existingNums=new Set(services.filter(s=>s.pedidoNum).map(s=>String(s.pedidoNum)));
  importRows=[];
  let skippedClosed=0;
  let skippedBadArt=0;
 
  // Group rows by pedido key: PncNro (if >0) or "0_clientName_FecSrv"
  const pedidoMap={};
 
  rows.forEach(row=>{
    // Skip closed orders (S = servido/cerrado); keep N (abierto) and A
    const cerrado=getImportValue(row,[['pncflgcer'],['pedido','cerrado'],['cerrado']]);
    const cerradoVal=String(cerrado||'').trim().toUpperCase();
    if(cerradoVal==='S'){skippedClosed++;return;}
 
    // Get article code — skip only rows where ArtCod is null/empty or a datetime object
    const artRaw=getImportValue(row,[['artcod'],['articulo'],['art']]);
    if(artRaw===null||artRaw===undefined||artRaw===''){return;} // empty line, skip silently
    // Detect datetime: XLSX parses dates as JS Date objects (type check via string pattern)
    const artStr=String(artRaw);
    if(artStr.includes('T')&&artStr.includes('-')&&artStr.length>15){skippedBadArt++;return;}
    const artCod=artStr;
 
    const pedNum=getImportValue(row,[['pncnro'],['pncpcinro'],['pedido']]);
    const pedNumStr=String(pedNum||'0').trim();
 
    // Use PncFecSrvC (confirmed) if available, else PncFecSrv (requested)
    const fecSrvCRaw=getImportValue(row,[['pncfecsrvc'],['fecha','confirmada'],['confirmada']]);
    const fecSrvRaw=getImportValue(row,[['pncfecsrv'],['fecha','servir'],['fecsrv']]);
    const dateISO=valueDateToISO(fecSrvCRaw)||valueDateToISO(fecSrvRaw);
    if(!dateISO)return;
 
    const pobObra=getImportValue(row,[['pncobrpob'],['poblacion','obra']]);
    const pobCli=getImportValue(row,[['clipob'],['poblacion']]);
    const pobEntrega=pobObra||pobCli;
    const clientName=getImportValue(row,[['clinomfis'],['nombre','fiscal']]);
    const obraName=getImportValue(row,[['obrnom'],['nombre','obra']]);
    const reference=obraName?`${clientName} — ${obraName}`:clientName;
    const dirRaw=getImportValue(row,[['pncobrdir'],['obrdir'],['direccion','obra'],['direccion','entrega'],['dir','obra'],['clidir'],['direccion'],['domicilio']]);
    const cpRaw=String(getImportValue(row,[['pncobrcpo'],['obrcpo'],['codigo','postal']])||'').trim().replace(/\.0$/,'');
    const cpNorm=/^\d{4}$/.test(cpRaw)?'0'+cpRaw:cpRaw; // Excel pierde el 0 inicial (07300 -> 7300)
    const qty=Number(getImportValue(row,[['pnccan'],['cantidad']])||0);
 
    // Unique key per pedido: use PncNro if non-zero, else client+date
    const pedKey=pedNumStr!=='0'
      ?`NRO_${pedNumStr}`
      :`ZERO_${clientName}_${dateISO}`;
 
    if(!pedidoMap[pedKey]){
      pedidoMap[pedKey]={
        pedNum: pedNumStr!=='0'?pedNumStr:null,
        dateISO,
        client: reference,
        muni: cleanPob(pobEntrega),
        direccion: String(dirRaw||'').trim(),
        cp: cpNorm,
        pobRaw: pobEntrega,
        zones: detectZone(pobObra,pobCli),
        articles:[],
        cerrado: cerradoVal
      };
    }
    // Add article to this pedido
    pedidoMap[pedKey].articles.push({artCod, qty});
    if(!pedidoMap[pedKey].direccion&&dirRaw)pedidoMap[pedKey].direccion=String(dirRaw).trim();
    if(!pedidoMap[pedKey].cp&&cpNorm)pedidoMap[pedKey].cp=cpNorm;
    // Update dateISO to the earliest confirmed/requested date among articles
    if(dateISO<pedidoMap[pedKey].dateISO) pedidoMap[pedKey].dateISO=dateISO;
  });
 
  // Build importRows from grouped pedidos
  Object.values(pedidoMap).forEach(p=>{
    const isDup=p.pedNum&&existingNums.has(p.pedNum);
    const isOutside=OUTSIDE_KW.some(k=>normalizeStr(p.pobRaw||'').includes(k));
    importRows.push({
      pedNum: p.pedNum||'—',
      dateISO: p.dateISO,
      client: p.client,
      muni: p.muni,
      pobRaw: p.pobRaw,
      direccion: p.direccion||'',
      cp: p.cp||'',
      zones: p.zones,
      articles: p.articles,
      isDup: !!isDup,
      isOut: isOutside,
      sel: !isDup&&!isOutside
    });
  });
 
  // Pedidos ya importados antes de existir la dirección: se completan con la dirección/CP del Excel
  let enriched=0;
  importRows.forEach(r=>{
    if(!r.isDup||!(r.direccion||r.cp))return;
    services.filter(s=>String(s.pedidoNum)===String(r.pedNum)).forEach(s=>{
      if(r.direccion&&!s.direccion){s.direccion=r.direccion;enriched++;}
      if(r.cp&&!s.cp)s.cp=r.cp;
    });
  });
  if(enriched){
    saveData('importacion',{summary:`${enriched} pedido(s) existente(s) completados con la dirección de la obra`,affectedCount:enriched});
    ensureOrsData();
  }
 
  // Sort by date then client
  importRows.sort((a,b)=>a.dateISO.localeCompare(b.dateISO)||a.client.localeCompare(b.client));
 
  if(!importRows.length){
    if(!silent)alert(`No se encontraron pedidos importables.\n${skippedClosed} pedidos cerrados omitidos.\n${skippedBadArt} líneas con código de artículo inválido omitidas.`);
    return;
  }
  // En carga automática solo se muestra el modal si hay algo nuevo que importar
  if(silent&&!importRows.some(r=>!r.isDup&&!r.isOut)){importRows=[];return;}
  showImportPreview();
}
 
function showImportPreview(){
  const total=importRows.length;
  const dups=importRows.filter(r=>r.isDup).length;
  const noZone=importRows.filter(r=>!r.isDup&&!r.zones.length).length;
  const outside=importRows.filter(r=>r.isOut).length;
  document.getElementById('import-summary').innerHTML=
    `<strong>${total}</strong> pedidos abiertos · <strong>${total-dups-outside}</strong> nuevos`+
    (dups?` · <span style="color:#633806;">${dups} ya importados</span>`:'')+
    (outside?` · <span style="color:#4A1A8C;">${outside} dirección fiscal excluida</span>`:'')+
    (noZone?` · <span style="color:#791F1F;">${noZone} sin zona</span>`:'');
 
  const tbody=document.getElementById('import-tbody');
  tbody.innerHTML='';
  importRows.forEach((r,i)=>{
    const tr=document.createElement('tr');
    if(r.isDup||r.isOut)tr.className='import-row-skip';
    const d=new Date(r.dateISO+'T00:00:00');
    const dateLabel=DAYS_ES[d.getDay()]+' '+d.getDate()+'/'+String(d.getMonth()+1).padStart(2,'0');
    const zoneTag=r.zones.length
      ?r.zones.map(z=>`<span class="zone-tag">${z}</span>`).join(' ')
      :`<span class="no-zone">Sin zona</span>`;
    const dupTag=r.isDup?'<span class="dup-tag">Ya importado</span>':'';
    tr.innerHTML=`
      <td><input type="checkbox" class="row-check" data-idx="${i}" ${r.include?'checked':''} ${(r.isDup||r.isOut)?'disabled':''}></td>
      <td>${r.pedNum} ${dupTag}</td>
      <td>${dateLabel}</td>
      <td style="max-width:140px;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;" title="${r.client}">${r.client}</td>
      <td>${r.muni||r.pobRaw}</td>
      <td>${zoneTag}</td>
      <td style="font-size:10px;color:var(--color-text-tertiary);">${(r.articles||[]).map(a=>`${a.artCod}×${a.qty}`).join(', ')}</td>`;
    tr.querySelector('.row-check')?.addEventListener('change',function(){importRows[i].include=this.checked;});
    tbody.appendChild(tr);
  });
  document.getElementById('check-all').checked=true;
  document.getElementById('import-modal').classList.add('open');
}
 
function toggleAll(cb){importRows.forEach((r,i)=>{if(!r.isDup&&!r.isOut){r.include=cb.checked;const c=document.querySelector(`.row-check[data-idx="${i}"]`);if(c)c.checked=cb.checked;}});}
 
function confirmImport(){
  const toImport=importRows.filter(r=>r.include&&!r.isDup);
  if(!toImport.length){alert('No hay pedidos seleccionados para importar.');return;}
  toImport.forEach(r=>{
    services.push({
      articles: r.articles||[],
      id:Date.now()+Math.random(),
      pedidoNum:r.pedNum,
      type:'reparto',
      driver:null, // unassigned — will show as pending
      vehicle:null,
      day:r.dateISO,
      client:r.client,
      muni:r.muni,
      direccion:r.direccion||'',
      cp:r.cp||'',
      zones:r.zones,
      salida:1,
      twoPerson:false,
      driver2:null,
      imported:true,
      assigned:false
    });
  });
  saveData('importacion',{summary:`${toImport.length} pedidos importados`,affectedCount:toImport.length});
  ensureOrsData();
  closeImportModal();
  renderCalendar();
}
 
function closeImportModal(){document.getElementById('import-modal').classList.remove('open');importRows=[];}
document.getElementById('import-modal').addEventListener('click',function(e){if(e.target===this)closeImportModal();});
 
// ---- CALENDAR ----
function onMuniInput(){
  if(isEditing)return;
  const val=document.getElementById('f-muni').value.trim().toLowerCase();
  const list=document.getElementById('muni-list');
  if(val.length<2){list.style.display='none';return;}
  const matches=ALL_MUNIS.filter(m=>m.toLowerCase().includes(val)).slice(0,8);
  if(!matches.length){list.style.display='none';return;}
  list.innerHTML=matches.map(m=>`<div class="muni-opt" onclick="selectMuni('${m.replace(/'/g,"\\'")}')">${m}</div>`).join('');
  list.style.display='block';
}
function selectMuni(muni){
  document.getElementById('f-muni').value=muni;
  document.getElementById('muni-list').style.display='none';
  if(isEditing)return;
  buildZonesGrid(MUNI_ZONE[muni]||[]);
  updateSuggestion();
}
 
// ---- SUGGESTION ----
function updateSuggestion(){
  if(isEditing)return;
  const type=document.getElementById('f-type').value;
  const day=document.getElementById('f-day').value;
  const muni=document.getElementById('f-muni').value.trim();
  const box=document.getElementById('suggestion-box');
  if(type!=='reparto'||!day){box.style.display='none';return;}
  const suggested=suggestDriver(day,type);
  const zones=muni&&MUNI_ZONE[muni]?MUNI_ZONE[muni]:[];
  if(suggested){
    box.innerHTML=`<i class="ti ti-bulb" aria-hidden="true" style="font-size:13px;vertical-align:-1px;margin-right:6px;"></i><strong>Sugerencia:</strong> <strong>${suggested.name}</strong>${zones.length?' · zona <strong>'+zones.join(' + ')+'</strong>':''} (menor carga ese día)`;
    box.style.display='block';
    document.getElementById('f-driver').value=suggested.id;
    if(zones.length)buildZonesGrid(zones);
    onDriverOrDayChange();
  }else{box.style.display='none';}
}
function onDriverOrDayChange(){updateSalidaOptions();}
 
// ---- VEHICLE OPTIONS ----
function buildVehicleOptions(vEl,type,driverId){
  vEl.innerHTML='';
  const dr=DRIVER_BY_ID.get(parseInt(driverId));
  VEHICLES.forEach(v=>{
    if(type==='cisterna'){
      if(v.type!=='tractora')return;
      if(!dr?.canTractora)return;
    }else{
      if(v.type==='tractora'&&!dr?.canTractora)return; // tractora con plataforma también reparte
      if((v.type==='camion'||v.type==='furgon')&&!dr?.canCamion)return;
      if(v.type==='furgoneta'&&!dr?.canFurgon)return;
    }
    const o=document.createElement('option');o.value=v.id;o.textContent=v.name;vEl.appendChild(o);
  });
}
 
// ---- EDITOR DE PRODUCTOS ----
function addProductoRow(artCod='',qty=1,entregado=true){
  const list=document.getElementById('productos-list');
  const row=document.createElement('div');
  row.className='producto-row';
  row.style.cssText='display:flex;flex-direction:column;gap:2px;';
  row.innerHTML=`
    <div style="display:flex;gap:5px;align-items:center;">
      <input type="checkbox" class="prod-chk" ${entregado?'checked':''} title="Entregado">
      <input type="text" class="prod-cod" placeholder="Código artículo" value="${escapeHtml(artCod)}" style="flex:2;padding:4px 6px;border:0.5px solid var(--color-border-tertiary);border-radius:4px;" oninput="updateProdName(this)">
      <input type="number" class="prod-qty" min="1" value="${qty||1}" style="width:60px;padding:4px 6px;border:0.5px solid var(--color-border-tertiary);border-radius:4px;">
      <button type="button" onclick="this.closest('.producto-row').remove()" style="font-size:11px;padding:2px 6px;cursor:pointer;border-radius:3px;background:rgba(0,0,0,0.08);border:none;">✕</button>
    </div>
    <div class="prod-name" style="font-size:10px;color:var(--color-text-tertiary);padding-left:22px;"></div>`;
  list.appendChild(row);
  updateProdName(row.querySelector('.prod-cod'));
}
// Si algún producto introducido tiene formato Cisterna, autoseleccionamos Tipo=Cisterna y
// Descarga=Cisterna (solo si no lo están ya, para no pisar una selección manual repetidamente).
function autoSelectCisternaIfNeeded(){
  const codes=Array.from(document.querySelectorAll('#productos-list .prod-cod')).map(el=>el.value.trim());
  if(!codes.some(isCisternaFormatCode))return;
  const typeEl=document.getElementById('f-type');
  if(typeEl&&typeEl.value!=='cisterna'){
    typeEl.value='cisterna';
    onTypeChange();
  }
  const merchEl=document.getElementById('f-merch-type');
  if(merchEl&&merchEl.value!=='cisterna'){
    merchEl.value='cisterna';
    onMerchTypeChange();
  }
}
function updateProdName(input){
  const row=input.closest('.producto-row');
  const cod=input.value.trim();
  const nameEl=row.querySelector('.prod-name');
  nameEl.textContent=cod?getArticleName(cod):'';
  autoSelectCisternaIfNeeded();
  refreshCisternaSelect();
}
function renderProductos(articles){
  document.getElementById('productos-list').innerHTML='';
  (articles||[]).forEach(a=>addProductoRow(a.artCod,a.qty,true));
}
function readProductos(){
  return Array.from(document.querySelectorAll('#productos-list .producto-row')).map(row=>({
    artCod: row.querySelector('.prod-cod').value.trim(),
    qty: Number(row.querySelector('.prod-qty').value)||1,
    entregado: row.querySelector('.prod-chk').checked
  })).filter(a=>a.artCod);
}
 
function updateSalidaOptions(){
  const day=document.getElementById('f-day').value;
  const driver=parseInt(document.getElementById('f-driver').value);
  const sel=document.getElementById('f-salida');
  const savedVal=isEditing?parseInt(sel.value)||null:null;
  sel.innerHTML='';if(!day||!driver)return;
  const max=getMaxSalidaForDriver(day,driver);
  const dayServices=servicesForDay(day);
  for(let i=1;i<=max+1;i++){
    const isArrastreSlot=dayServices.some(s=>s.driver===driver&&s.salida===i&&s.type==='arrastre');
    const o=document.createElement('option');
    o.value=i;
    if(isArrastreSlot){
      o.textContent=`${ORDINALS[i-1]||i+'ª'} — ocupado por arrastre ⛴`;
      o.disabled=true;
    }else{
      o.textContent=i<=max?`${ORDINALS[i-1]||i+'ª'} salida (existente)`:`${ORDINALS[i-1]||i+'ª'} salida (nueva)`;
    }
    sel.appendChild(o);
  }
  if(isEditing&&savedVal)sel.value=savedVal;else sel.value=max+1;
  onSalidaChange();
}
function onSalidaChange(){
  const day=document.getElementById('f-day').value;
  const driver=parseInt(document.getElementById('f-driver').value);
  const salida=parseInt(document.getElementById('f-salida').value);
  const max=getMaxSalidaForDriver(day,driver);
  const existingVehicle=getVehicleForSalida(day,driver,salida);
  const type=document.getElementById('f-type').value;
  const vEl=document.getElementById('f-vehicle');
  buildVehicleOptions(vEl,type,driver);
  if(existingVehicle&&salida<=max){
    const vh=VEHICLE_BY_ID.get(existingVehicle);
    document.getElementById('row-vehicle-select').style.display='none';
    document.getElementById('row-vehicle-locked').style.display='';
    document.getElementById('f-vehicle-locked').textContent=vh?vh.name:existingVehicle;
    vEl.value=existingVehicle;
    const existing=(servicesForDay(day)||[]).find(s=>s.driver===driver&&s.salida===salida&&s.horaSalida);
    if(existing)document.getElementById('f-hora-salida').value=existing.horaSalida;
  }else{
    document.getElementById('row-vehicle-select').style.display='';
    document.getElementById('row-vehicle-locked').style.display='none';
  }
}
 
// ---- ZONES ----
function getSelectedZones(){return Array.from(document.querySelectorAll('.zone-chip.selected')).map(el=>el.dataset.zone);}
function buildZonesGrid(sel){
  const grid=document.getElementById('zones-grid');grid.innerHTML='';
  ZONES.forEach(z=>{const chip=document.createElement('div');chip.className='zone-chip'+(sel&&sel.includes(z)?' selected':'');chip.dataset.zone=z;chip.innerHTML=`<i class="ti ti-map-pin" aria-hidden="true" style="font-size:11px;"></i>${z}`;chip.onclick=()=>chip.classList.toggle('selected');grid.appendChild(chip);});
}
function moveSalida(dayKey,driverId,salidaNum,dir){
  const target=salidaNum+dir;if(target<1)return;
  const maxS=getMaxSalidaForDriver(dayKey,driverId);if(target>maxS)return;
  services.forEach(s=>{if(s.day===dayKey&&s.driver===driverId){if(s.salida===salidaNum)s.salida=target;else if(s.salida===target)s.salida=salidaNum;}});
  saveData('reordenacion',{summary:'Salida reordenada',day:dayKey});
  renderCalendar();
}

// ---- DRAG & DROP DE BLOQUES ENTRE DIAS ----
// Si el pedido movido es de cisterna y tiene cuba asignada, mantiene alineada la fecha de
// entrega registrada en la flota (cisternaStock) con el nuevo dia del servicio.
function syncCisternaDateForService(s){
  if(s.type!=='cisterna'||!s.cisternaCuba)return false;
  const c=cisternaStock.find(x=>cisternaKey(x.cuba)===cisternaKey(s.cisternaCuba));
  if(!c)return false;
  c.fechaEntregaAsignada=s.day;
  return true;
}
// Mueve un bloque completo (salida, arrastre o pedido sin asignar) al dia soltado en el
// calendario. `payload` viene del dataTransfer del drag (ver renderCalendar). Al mover una
// salida entera, se le asigna el siguiente numero de salida libre de ese conductor en el dia
// destino (no conserva el numero de origen, que puede estar ocupado alli).
function moveBlockToDay(payload,newDayKey){
  if(!payload||!payload.kind)return;
  let cisternaTouched=false;
  if(payload.kind==='salida'){
    if(payload.day===newDayKey)return;
    const items=services.filter(s=>s.day===payload.day&&s.driver===payload.driver&&s.salida===payload.salida);
    if(!items.length)return;
    const horaSalida=items[0]?.horaSalida||null;
    if(horaSalida){
      const dur=calcSalidaDuration({items,origenTrayecto:items[0].origenTrayecto,destinoTrayecto:items[0].destinoTrayecto}).totalMin;
      const endTime=addMinutes(horaSalida,dur);
      const conflictMsgs=[];
      const driverConflict=getPersonBusyIntervals(newDayKey,payload.driver,null,null)
        .find(iv=>intervalsOverlap(horaSalida,endTime,iv.start,iv.end));
      if(driverConflict)conflictMsgs.push(`${DRIVER_BY_ID.get(payload.driver)?.name||'El conductor'} ya tiene otra salida ese día entre ${driverConflict.start} y ${driverConflict.end}.`);
      const driver2=items[0].twoPerson?items[0].driver2:null;
      if(driver2){
        const helperConflict=getPersonBusyIntervals(newDayKey,driver2,null,null)
          .find(iv=>intervalsOverlap(horaSalida,endTime,iv.start,iv.end));
        if(helperConflict)conflictMsgs.push(`${DRIVER_BY_ID.get(driver2)?.name||'El ayudante'} ya tiene otra salida ese día entre ${helperConflict.start} y ${helperConflict.end}.`);
      }
      if(conflictMsgs.length&&!confirm(`Al mover esta salida al ${newDayKey} se solapa con lo ya programado:\n\n${conflictMsgs.join('\n')}\n\n¿Moverla de todas formas? Tendrás que ajustar la hora después.`))return;
    }
    const newSalida=nextSalidaNumberForDriver(newDayKey,payload.driver,null);
    items.forEach(s=>{
      s.day=newDayKey;s.salida=newSalida;
      if(syncCisternaDateForService(s))cisternaTouched=true;
    });
    compactDriverSalidas(payload.day,payload.driver,payload.salida);
    saveData('reordenacion',{summary:`Salida movida al ${newDayKey}`,day:newDayKey});
  }else if(payload.kind==='arrastre'){
    const s=services.find(x=>x.id===payload.id);
    if(!s||s.day===newDayKey)return;
    if(s.horaSalida){
      const dur=calcArrastreDuration().min;
      const endTime=addMinutes(s.horaSalida,dur);
      const conflict=getPersonBusyIntervals(newDayKey,s.driver,null,null)
        .find(iv=>intervalsOverlap(s.horaSalida,endTime,iv.start,iv.end));
      if(conflict&&!confirm(`Al mover este arrastre al ${newDayKey} se solapa con otra salida de ${DRIVER_BY_ID.get(s.driver)?.name||'el conductor'} entre ${conflict.start} y ${conflict.end}.\n\n¿Moverlo de todas formas?`))return;
    }
    const oldDay=s.day,driver=s.driver,oldSalida=s.salida;
    s.day=newDayKey;
    s.salida=nextSalidaNumberForDriver(newDayKey,driver,s.id);
    compactDriverSalidas(oldDay,driver,oldSalida);
    saveData('reordenacion',{summary:'Arrastre movido de dia',day:newDayKey,type:'arrastre'});
  }else if(payload.kind==='unassigned'){
    const s=services.find(x=>x.id===payload.id);
    if(!s||s.day===newDayKey)return;
    s.day=newDayKey;
    saveData('reordenacion',{summary:`${s.pedidoNum?'#'+s.pedidoNum+' ':''}${s.client||'Pedido'} movido de dia`,day:newDayKey,pedidoNum:s.pedidoNum,type:s.type});
  }else if(payload.kind==='order'){
    moveOrderToDay(payload.id,newDayKey);
    return;
  }else return;
  if(cisternaTouched)writeCisternaStock();
  renderCalendar();
  if(cisternaTouched)renderCisternaFleetIfOpen();
}
 
 
// ---- SERVIDO STATE ----
function toggleServido(id, e) {
  if (e) e.stopPropagation();
  const idx = services.findIndex(s => s.id === id);
  if (idx > -1) {
    services[idx].servido = !services[idx].servido;
    saveData('servido',{summary:`${services[idx].servido?'Servido':'Pendiente'} ${services[idx].pedidoNum?'#'+services[idx].pedidoNum:''}`.trim(),day:services[idx].day,pedidoNum:services[idx].pedidoNum,type:services[idx].type});
    renderCalendar();
  }
}
 
function toggleSalidaServida(dayKey, driverId, salidaNum, e) {
  if (e) e.stopPropagation();
  // Find all items for this salida
  const items = services.filter(s => s.day === dayKey && s.driver === driverId && s.salida === salidaNum);
  if (!items.length) return;
  // If all are servido → unmark all; otherwise mark all
  const allServido = items.every(s => s.servido);
  items.forEach(s => { s.servido = !allServido; });
  saveData('servido',{summary:`Salida ${!allServido?'servida':'pendiente'} (${items.length} pedidos)`,day:dayKey,affectedCount:items.length});
  renderCalendar();
}
 
function duplicateForReprogramming(idStr){
  const id=parseFloat(idStr);
  const orig=services.find(s=>s.id===id);
  if(!orig){console.log('No encontrado:',id);return;}
  services.push({
    ...orig,
    id:Date.now(),
    noEntregado:false,
    observaciones:'',
    servido:false,
    day:orig.day, // dia siguiente. fecha para reprogramar
    driver:null,
    salida:1,
    vehicle:null,
    cisternaCuba:null,
    cisternaPlataforma:null
  });
  saveData('duplicacion',{summary:`${orig.pedidoNum?'#'+orig.pedidoNum+' ':''}${orig.client} duplicado para reprogramar`,day:orig.day});
  renderCalendar();
}
 
function moveItem(itemId, dir) {
  const item = services.find(s => s.id === itemId);
  if(!item) return;
  // Get all items in same salida, sorted by current order
  const siblings = services
    .filter(s => s.day === item.day && s.driver === item.driver && s.salida === item.salida)
    .sort((a,b) => (a.itemOrder||0) - (b.itemOrder||0));
  // Ensure all have itemOrder
  siblings.forEach((s, i) => { s.itemOrder = i; });
  const idx = siblings.findIndex(s => s.id === itemId);
  const targetIdx = idx + dir;
  if(targetIdx < 0 || targetIdx >= siblings.length) return;
  // Swap orders
  const tmp = siblings[idx].itemOrder;
  siblings[idx].itemOrder = siblings[targetIdx].itemOrder;
  siblings[targetIdx].itemOrder = tmp;
  saveData('reordenacion',{summary:'Pedido reordenado',day:item.day,pedidoNum:item.pedidoNum,type:item.type});
  renderCalendar();
}
 
function renderCalendar(){
  const days=getDaysForWeek(currentWeek);
  const dayIndex=servicesByDay();
  const cal=document.getElementById('calendar');
  cal.innerHTML='';
  const frag=document.createDocumentFragment();
  days.forEach(day=>{
    const key=dateKey(day);
    const dayServices=servicesForDay(key,dayIndex);
    const cisternas=dayServices.filter(s=>s.type==='cisterna');
    // El ayudante (driver2) no cuenta como que ha "hecho" esa salida — solo el conductor titular
    // computa para el tope de 2 cisternas/día. El tiempo del ayudante sí sigue contando (ver getPersonBusyIntervals).
    const dc={};cisternas.forEach(s=>{dc[s.driver]=(dc[s.driver]||0)+1;});
    const overloaded=Object.values(dc).some(v=>v>2);
    const warning=!overloaded&&Object.values(dc).some(v=>v===2);
    const unassigned=dayServices.filter(s=>s.imported&&!s.driver);
 
    const col=document.createElement('div');
    col.className='day-col'+(isWeekend(day)?' day-weekend':'')+(isToday(day)?' today':'')+(overloaded?' overloaded':warning?' warning':'');
    col.innerHTML=`<div class="day-header"><span class="day-name">${DAYS_ES[day.getDay()]} ${day.getDate()} ${MONTHS_ES[day.getMonth()]}</span><div style="display:flex;align-items:center;gap:5px;">${unassigned.length?`<span style="font-size:10px;background:#FAEEDA;color:#633806;border-radius:3px;padding:1px 5px;">${unassigned.length} sin asignar</span>`:''}<span class="capacity-dot ${overloaded?'over':warning?'warn':''}"></span></div></div>`;
 
    const svcDiv=document.createElement('div');svcDiv.className='day-services';
    if(!isWeekend(day)){
      const svcFrag=document.createDocumentFragment();
      unassigned.forEach(s=>{
        const chip=document.createElement('div');
        chip.style.cssText='font-size:11px;padding:3px 6px;background:#FFF8ED;border:0.5px solid #FAC775;border-radius:4px;cursor:pointer;color:#633806;';
        const zl=s.zones&&s.zones.length?s.zones.join(' · '):'sin zona';
        const pedidoLabel=s.pedidoNum?`<strong style="font-size:10px;color:#3F2506;">#${s.pedidoNum}</strong> · `:'';
        chip.innerHTML=`<span style="opacity:0.6;font-size:10px;">⚠ Sin asignar</span><br><span style="font-weight:500;">${pedidoLabel}${s.client||'—'}</span><br><span style="font-size:10px;opacity:0.7;">${s.muni||''} · ${zl}</span>`;
        chip.onclick=()=>openEdit(s.id);
        chip.draggable=true;
        chip.addEventListener('dragstart',e=>{
          e.dataTransfer.effectAllowed='move';
          e.dataTransfer.setData('blockMove',JSON.stringify({kind:'unassigned',id:s.id}));
          currentDrag={kind:'unassigned',id:s.id};
          chip.style.opacity='0.4';
        });
        chip.addEventListener('dragend',()=>{chip.style.opacity='';});
        svcFrag.appendChild(chip);
      });
 
      const dayBlocks=[]; // {hora, driver, kind, salida, el} — para intercalar salidas y arrastres por hora
      const salidas=getSalidasForDay(key,dayServices);
      const maxByDriver={};salidas.forEach(s=>{if(maxByDriver[s.driver]==null)maxByDriver[s.driver]=getMaxSalidaForDriver(key,s.driver,dayServices);});
      salidas.forEach(sal=>{
        const dr=DRIVER_BY_ID.get(sal.driver);const vh=VEHICLE_BY_ID.get(sal.vehicle);
        const dr2=sal.twoPerson&&sal.driver2?DRIVER_BY_ID.get(sal.driver2):null;
        const maxS=maxByDriver[sal.driver]||1;
        const allServido=sal.items.length>0&&sal.items.every(i=>i.servido);
        const block=document.createElement('div');block.className='salida-block'+(allServido?' servido':'');
        block.addEventListener('dragover',e=>{ if(!orderIsDraggable())return; e.preventDefault(); e.stopPropagation(); e.dataTransfer.dropEffect='move'; highlightDrop(block,true); });
        block.addEventListener('dragleave',e=>{ if(!block.contains(e.relatedTarget))highlightDrop(block,false); });
        block.addEventListener('drop',e=>{ if(!orderIsDraggable())return; e.preventDefault(); e.stopPropagation(); highlightDrop(block,false); moveOrderToSalida(currentDrag.id,key,sal.driver,sal.salida,null); });
        const hdr=document.createElement('div');hdr.className='salida-header '+(sal.type==='cisterna'?'cisterna':'reparto');
        const upBtn=sal.salida>1?`<button onclick="event.stopPropagation();moveSalida('${key}',${sal.driver},${sal.salida},-1)">↑</button>`:'';
        const dnBtn=sal.salida<maxS?`<button onclick="event.stopPropagation();moveSalida('${key}',${sal.driver},${sal.salida},1)">↓</button>`:'';
        const chkSalidaId=`chk-sal-${key}-${sal.driver}-${sal.salida}`;
        const dur=calcSalidaDuration(sal);
        const horaSalida=sal.items[0]?.horaSalida||null;
        const horaRegreso=horaSalida?addMinutes(horaSalida,dur.totalMin):null;
        const warnLabel=dur.missingCoords?`<span title="Alguna población no tiene coordenadas — distancia parcial" style="font-size:10px;color:#BA7517;margin-left:3px;">⚠</span>`:'';
        const durLabel=dur.totalMin>0?`<span style="font-size:10px;opacity:0.75;margin-left:4px;">${formatDuration(dur.totalMin)}${horaSalida?' · '+horaSalida+'→'+(horaRegreso||'?'):''}</span>${warnLabel}`:'';
        const kmLabel=dur.km>0?`<span style="font-size:9px;opacity:0.6;margin-left:3px;">${dur.km}km</span>`:'';
        const trayectoBadges=(sal.origenTrayecto==='puerto'?`<span class="salida-badge" title="Sale desde el Puerto de Palma">⛴ desde puerto</span>`:'')+
          (sal.destinoTrayecto==='puerto'?`<span class="salida-badge" title="Vuelve al Puerto de Palma">⛴ hacia puerto</span>`:'');
        hdr.innerHTML=`<span class="salida-label" style="display:flex;align-items:center;gap:5px;"><input type="checkbox" id="${chkSalidaId}" class="servido-chk" ${allServido?'checked':''} onclick="toggleSalidaServida('${key}',${sal.driver},${sal.salida},event)" title="Marcar salida completa como servida"><label for="${chkSalidaId}" class="salida-label-inner"><span class="salida-badge">${ORDINALS[sal.salida-1]||sal.salida+'ª'} sal.</span>${trayectoBadges}<strong>${dr?dr.name:'—'}</strong>${dr2?' <span style="opacity:0.7;font-size:10px;">+'+dr2.name+'</span>':''}</label>${durLabel}${kmLabel}</span><div style="display:flex;align-items:center;gap:2px;"><div class="order-btns">${upBtn}${dnBtn}</div><i class="ti ti-pencil" aria-hidden="true" style="font-size:11px;cursor:pointer;margin-left:4px;" onclick="openEditSalida('${key}',${sal.driver},${sal.salida})"></i></div>`;
        hdr.draggable=true;
        hdr.style.cursor='grab';
        hdr.title='Arrastra para mover la salida a otro día';
        hdr.addEventListener('dragstart',e=>{
          e.dataTransfer.effectAllowed='move';
          e.dataTransfer.setData('blockMove',JSON.stringify({kind:'salida',day:key,driver:sal.driver,salida:sal.salida}));
          block.classList.add('dragging');
        });
        hdr.addEventListener('dragend',()=>block.classList.remove('dragging'));
        const body=document.createElement('div');body.className='salida-body '+(sal.type==='cisterna'?'cisterna-body':'reparto-body');
        // Ensure all items have order
        sal.items.forEach((item, i) => { if(item.itemOrder == null) item.itemOrder = i; });
        const totalItems = sal.items.length;
        sal.items.forEach((item, itemIdx)=>{
          const row=document.createElement('div');
          row.className='client-row'+(item.servido?' servido':'')+(item.noEntregado?' no-entregado':'');
          row.draggable = true;
          row.dataset.itemId = item.id;
          row.dataset.day = key;
          row.dataset.driver = sal.driver;
          row.dataset.salida = sal.salida;
 
          const pedidoLabel=item.pedidoNum?`<strong style="color:var(--color-text-primary);font-size:10px;">#${item.pedidoNum}</strong> `:'';
          const chkId=`chk-item-${item.id}`;
          const descMin=calcDescargaMin(item);
          const descLabel=descMin>0?`<span style="font-size:9px;opacity:0.6;margin-left:2px;">⏱${formatDuration(descMin)}</span>`:'';
          const upBtn = itemIdx > 0
            ? `<button class="item-order-btn" onclick="event.stopPropagation();moveItem(${JSON.stringify(item.id)},-1)" title="Subir">↑</button>` : '';
          const dnBtn = itemIdx < totalItems-1
            ? `<button class="item-order-btn" onclick="event.stopPropagation();moveItem(${JSON.stringify(item.id)},1)" title="Bajar">↓</button>` : '';
 
          
          const artLabel=(item.articles&&item.articles.length)
          ?`<span class="art-tag" title="${item.articles.map(a=>a.artCod).join(', ')}">${item.articles.map(a=>getArticleName(a.artCod)+(a.qty?` ×${a.qty}`:'')).join(' · ')}</span>`:'';
          const dupBtn=item.noEntregado?`<button class="item-dup-btn" data-item-id="${item.id}" onclick="duplicateForReprogramming(this.getAttribute('data-item-id'))" title="Duplicar para reprogramar">↻</button>`:'';          row.innerHTML=`<span class="drag-handle" title="Arrastrar para reordenar">⠿</span><input type="checkbox" id="${chkId}" class="servido-chk item-chk" ${item.servido?'checked':''}        onclick="toggleServido(${JSON.stringify(item.id)},event)" title="Marcar como servido"><label for="${chkId}" class="client-label"><span class="client-text">· ${pedidoLabel}${item.client||'(sin nombre)'}</span>${item.muni?`<span class="muni-tag">${item.muni}</span>`:''}</label>${pinTag(item)}${descLabel}${artLabel}<span class="item-order-btns">${upBtn}${dnBtn}${dupBtn}</span>`;
          row.querySelector('.client-label').addEventListener('click', e => { if(e.target!==row.querySelector('input')) { e.preventDefault(); openEdit(item.id); }});
 
          // Drag & drop events
          row.addEventListener('dragstart', e => {
            e.dataTransfer.effectAllowed = 'move';
            e.dataTransfer.setData('itemId', String(item.id));
            e.dataTransfer.setData('blockMove', JSON.stringify({kind:'order', id:item.id}));
            currentDrag = {kind:'order', id:item.id};
            showUnassignZones();
            row.classList.add('dragging');
          });
          row.addEventListener('dragend', () => { row.classList.remove('dragging'); currentDrag = null; hideUnassignZones(); });
          row.addEventListener('dragover', e => { e.preventDefault(); e.dataTransfer.dropEffect='move'; row.classList.add('drag-over'); });
          row.addEventListener('dragleave', () => row.classList.remove('drag-over'));
          row.addEventListener('drop', e => {
            e.preventDefault(); e.stopPropagation();
            row.classList.remove('drag-over');
            const draggedId = orderIsDraggable() ? String(currentDrag.id) : e.dataTransfer.getData('itemId');
            if(!draggedId || draggedId === String(item.id)) return;
            const dragged = services.find(s => String(s.id) === draggedId);
            const target = item;
            if(!dragged) return;
            if(dragged.day !== target.day || dragged.driver !== target.driver || dragged.salida !== target.salida){
              // Otra salida (de otro conductor/día, o un pedido sin asignar): se mueve a esa salida, antes de este pedido
              moveOrderToSalida(dragged.id, target.day, target.driver, target.salida, target.id);
              return;
            }
            // Misma salida: reordenar, poniendo el arrastrado antes de este
            const siblings = services
              .filter(s => s.day===dragged.day && s.driver===dragged.driver && s.salida===dragged.salida)
              .sort((a,b) => (a.itemOrder||0)-(b.itemOrder||0));
            siblings.forEach((s,i) => { s.itemOrder = i*2; }); // space out
            dragged.itemOrder = target.itemOrder - 1; // insert before target
            // Renormalize
            const reordered = siblings.slice().sort((a,b)=>a.itemOrder-b.itemOrder);
            reordered.forEach((s,i) => { s.itemOrder = i; });
            saveData('reordenacion',{summary:'Pedido reordenado por arrastre',day:dragged.day,pedidoNum:dragged.pedidoNum,type:dragged.type});
            renderCalendar();
          });
 
          body.appendChild(row);
        });
        const metaParts=[];if(vh)metaParts.push(vh.name);if(sal.zones&&sal.zones.length)metaParts.push(sal.zones.join(' · '));if(sal.twoPerson)metaParts.push('👥 2 personas');
        const meta=document.createElement('div');meta.className='salida-meta';meta.textContent=metaParts.join(' · ');body.appendChild(meta);
        block.appendChild(hdr);block.appendChild(body);
        dayBlocks.push({hora:horaSalida||null,driver:sal.driver,kind:'salida',salida:sal.salida,el:block});
      });
      const arrastresDelDia=dayServices.filter(s=>s.type==='arrastre');
      const arDur=calcArrastreDuration();
      arrastresDelDia.forEach(s=>{
        const dr=DRIVER_BY_ID.get(s.driver);const vh=VEHICLE_BY_ID.get(s.vehicle);
        const trayecto=arrastreTrayectoField(s);
        const cubaInfo=s.cisternaCuba?cisternaStock.find(c=>cisternaKey(c.cuba)===cisternaKey(s.cisternaCuba)):null;
        const cubaLabel=s.cisternaCuba
          ?`<span style="opacity:0.8;">Cuba <strong>${escapeHtml(s.cisternaCuba)}</strong>${cubaInfo&&cubaInfo.plataforma?' ('+escapeHtml(cubaInfo.plataforma)+')':''}</span>`
          :`<span style="color:#A32D2D;">⚠ sin cuba asociada</span>`;
        const block=document.createElement('div');block.className='salida-block';
        block.innerHTML=`<div class="salida-header cisterna" style="cursor:pointer;">
          <span class="salida-label"><span class="salida-badge">⛴ arrastre</span><strong>${dr?dr.name:'—'}</strong></span>
          <span style="font-size:10px;opacity:0.75;">${s.horaSalida?s.horaSalida+' · ':''}${formatDuration(arDur.min)}</span>
        </div>
        <div class="salida-body cisterna-body">
          <div style="font-size:11px;">${arrastreTrayectoLabel(trayecto)}${vh?' · '+vh.name:''}<span style="font-size:9px;opacity:0.6;margin-left:3px;">${arDur.km}km</span></div>
          <div class="salida-meta">${cubaLabel}</div>
          ${s.observaciones?`<div class="salida-meta">${escapeHtml(s.observaciones)}</div>`:''}
        </div>`;
        const arrHdr=block.querySelector('.salida-header');
        arrHdr.onclick=()=>openEditArrastre(s.id);
        arrHdr.draggable=true;
        arrHdr.title='Arrastra para mover el arrastre a otro día';
        arrHdr.addEventListener('dragstart',e=>{
          e.dataTransfer.effectAllowed='move';
          e.dataTransfer.setData('blockMove',JSON.stringify({kind:'arrastre',id:s.id}));
          block.classList.add('dragging');
        });
        arrHdr.addEventListener('dragend',()=>block.classList.remove('dragging'));
        dayBlocks.push({hora:s.horaSalida||null,driver:s.driver,kind:'arrastre',salida:null,el:block});
      });
      // Orden cronológico real: intercala salidas y arrastres por hora (los sin hora van al final,
      // igual que antes hacían las salidas sin hora).
      dayBlocks.sort((a,b)=>{
        const ha=a.hora,hb=b.hora;
        if(ha&&hb&&ha!==hb)return ha.localeCompare(hb);
        if(ha&&!hb)return -1;
        if(!ha&&hb)return 1;
        if(a.driver!==b.driver)return (a.driver||0)-(b.driver||0);
        if(a.kind!==b.kind)return a.kind==='salida'?-1:1;
        return (a.salida||0)-(b.salida||0);
      });
      dayBlocks.forEach(b=>svcFrag.appendChild(b.el));
      const addBtn=document.createElement('button');addBtn.className='add-btn';addBtn.textContent='+ añadir pedido';addBtn.onclick=()=>openAdd(key,null);svcFrag.appendChild(addBtn);
      const addArrastreBtn=document.createElement('button');addArrastreBtn.className='add-btn';addArrastreBtn.textContent='⛴ + Arrastre puerto';addArrastreBtn.onclick=()=>openAddArrastre(key);svcFrag.appendChild(addArrastreBtn);
      svcDiv.appendChild(buildUnassignZone(key));
      svcDiv.appendChild(svcFrag);
    }
    if(!isWeekend(day)){
      col.addEventListener('dragover',e=>{
        if(!e.dataTransfer.types.includes('blockmove'))return; // ignora el reordenado de filas
        e.preventDefault();
        e.dataTransfer.dropEffect='move';
        col.classList.add('day-drop-target');
      });
      col.addEventListener('dragleave',e=>{
        if(!col.contains(e.relatedTarget))col.classList.remove('day-drop-target');
      });
      col.addEventListener('drop',e=>{
        const raw=e.dataTransfer.getData('blockMove');
        if(!raw)return;
        e.preventDefault();
        col.classList.remove('day-drop-target');
        let payload;try{payload=JSON.parse(raw);}catch(err){return;}
        moveBlockToDay(payload,key);
      });
    }
    col.appendChild(svcDiv);frag.appendChild(col);
  });
  cal.appendChild(frag);
  updateStats(dayIndex);updateAlerts(dayIndex);
 
}
function updateStats(dayIndex){
  const keys=getDaysForWeek(currentWeek).map(dateKey);
  const ws=keys.flatMap(key=>servicesForDay(key,dayIndex));
  document.getElementById('stat-cisternas').textContent=ws.filter(s=>s.type==='cisterna').length;
  document.getElementById('stat-repartos').textContent=ws.filter(s=>s.type==='reparto'&&s.driver).length;
  const todayKey=dateKey(new Date());
  const usedT=new Set(servicesForDay(todayKey,dayIndex).filter(s=>s.type==='cisterna').map(s=>s.vehicle));
  document.getElementById('stat-tractoras').textContent=4-usedT.size;
}
function updateAlerts(dayIndex){
  const todayKey=localDateKey(new Date());
  const lookback=new Date();lookback.setDate(lookback.getDate()-6);
  const lookbackKey=localDateKey(lookback);

  // "Pendientes de asignar": se recorren TODOS los servicios en memoria directamente por fecha
  // (no la lista de días del calendario), para no depender de qué semanas se generan ni de si el
  // día cae en el filtro de fin de semana. Ventana: desde 5 días antes de hoy en adelante.
  const pendingByDay={};
  services.forEach(s=>{
    if(!s.imported||s.driver)return;
    if(!s.day||s.day<lookbackKey)return; // sin fecha, o demasiado antiguo (fuera de la ventana)
    (pendingByDay[s.day]=pendingByDay[s.day]||[]).push(s);
  });

  const alerts=[];
  const alertDaySet=new Set();
  Object.keys(pendingByDay).sort().forEach(dayStr=>{
    const list=pendingByDay[dayStr];
    const d=new Date(dayStr+'T00:00:00');
    const isOverdue=dayStr<todayKey;
    alerts.push(`${DAYS_ES[d.getDay()]} ${d.getDate()}/${d.getMonth()+1}: ${list.length} pedido(s) importado(s) sin asignar${isOverdue?' ⚠ atrasado':''}`);
    alertDaySet.add(dayStr);
  });

  // Resto de alertas (exceso de cisternas/día, cisterna sin cuba) siguen mirando la ventana de
  // 5 semanas alrededor de hoy, como antes.
  const allDays=[-2,-1,0,1,2].flatMap(getDaysForWeek);
  allDays.forEach(day=>{
    if(isWeekend(day))return;
    const key=dateKey(day);
    const isFutureOrToday=key>=todayKey;
    const dayServices=servicesForDay(key,dayIndex);
    const dayC=dayServices.filter(s=>s.type==='cisterna');
    // El ayudante (driver2) no cuenta como que ha "hecho" esa salida — solo el conductor titular.
    const dc={};dayC.forEach(s=>{dc[s.driver]=(dc[s.driver]||0)+1;});
    Object.entries(dc).forEach(([did,cnt])=>{
      if(cnt>2){
        const d=DRIVER_BY_ID.get(parseInt(did));
        alerts.push(`${DAYS_ES[day.getDay()]} ${day.getDate()}/${day.getMonth()+1}: ${d?d.name:'Conductor'} tiene ${cnt} cisternas (máx. 2)`);
        alertDaySet.add(key);
      }
    });
    if(isFutureOrToday){
      const sinCuba=dayC.filter(s=>!s.cisternaCuba);
      if(sinCuba.length){
        alerts.push(`${DAYS_ES[day.getDay()]} ${day.getDate()}/${day.getMonth()+1}: ${sinCuba.length} cisterna(s) sin cuba física asignada`);
        alertDaySet.add(key);
      }
    }
  });

  cisternaStock.forEach(c=>{
    const expired=['itvCaduca','adrCaduca','imdgCaduca'].filter(f=>c[f]&&c[f]<todayKey);
    if(expired.length){alerts.push(`Cuba ${c.cuba}: documentación caducada (${expired.map(f=>({itvCaduca:'ITV',adrCaduca:'ADR',imdgCaduca:'IMDG'}[f])).join(', ')})`);}
  });

  document.getElementById('stat-alerts').textContent=alertDaySet.size;
  document.getElementById('alerts').innerHTML=alerts.map(a=>`<div class="alert-item"><i class="ti ti-alert-triangle" aria-hidden="true" style="font-size:13px;vertical-align:-1px;margin-right:6px;"></i>${a}</div>`).join('');
}

function setWeek(n,btn){currentWeek=n;selectedTimelineDay=null;document.querySelectorAll('.week-tab').forEach(t=>t.classList.remove('active'));btn?.classList.add('active');renderCalendar();}
 
function openAdd(dayKey,type){
  isEditing=false;editingId=null;
  document.getElementById('modal-title').textContent='Añadir pedido';
  document.getElementById('f-pedido').value='';
  document.getElementById('f-merch-type').value=type==='cisterna'?'cisterna':'';
  document.getElementById('f-merch-qty').value='';
  document.getElementById('f-merch-manual').value='';
  document.getElementById('row-merch-qty').style.display='none';
  document.getElementById('row-merch-manual').style.display='none';
  document.getElementById('f-hora-salida').value='';
  document.getElementById('btn-delete').style.display='none';
  document.getElementById('f-client').value='';document.getElementById('f-muni').value='';document.getElementById('f-direccion').value='';updatePinStatus();
  renderProductos([]);
  document.getElementById('f-no-entregado').checked=false;
  document.getElementById('f-observaciones').value='';
  document.getElementById('f-twoperson').checked=false;document.getElementById('suggestion-box').style.display='none';document.getElementById('muni-list').style.display='none';
  document.getElementById('f-origen-trayecto').value='base';document.getElementById('f-destino-trayecto').value='base';
  populateFormNew(type||'reparto',dayKey);buildZonesGrid([]);updateMerchPreview();refreshCisternaSelect();document.getElementById('modal').classList.add('open');
}
 
function openEdit(id){
  isEditing=true;editingId=id;
  const s=services.find(x=>x.id===id);if(!s)return;
  document.getElementById('modal-title').textContent='Editar pedido';document.getElementById('btn-delete').style.display='';
  document.getElementById('f-pedido').value=s.pedidoNum||'';
  renderProductos(s.articles);
  document.getElementById('f-merch-type').value=s.merchType||'';
  document.getElementById('f-merch-qty').value=s.merchQty||'';
  document.getElementById('f-merch-manual').value=s.merchManual||'';
  document.getElementById('f-hora-salida').value=s.horaSalida||'';
  onMerchTypeChange();
  updateMerchPreview();
  document.getElementById('suggestion-box').style.display='none';document.getElementById('muni-list').style.display='none';
  populateFormSilent(s.type,s.day);
  document.getElementById('f-type').value=s.type;document.getElementById('f-muni').value=s.muni||'';
  document.getElementById('f-driver').value=s.driver||'';document.getElementById('f-day').value=s.day;
  document.getElementById('f-client').value=s.client||'';document.getElementById('f-direccion').value=s.direccion||'';updatePinStatus();
  document.getElementById('f-origen-trayecto').value=s.origenTrayecto||'base';
  document.getElementById('f-destino-trayecto').value=s.destinoTrayecto||'base';
  document.getElementById('f-twoperson').checked=!!s.twoPerson;
  buildZonesGrid(s.zones||[]);
  updateSalidaOptions();document.getElementById('f-salida').value=s.salida||1;
  onSalidaChange();onTypeChangeSilent(s.type);
  if(s.twoPerson&&s.driver2){document.getElementById('f-driver2').value=s.driver2;document.getElementById('row-second').style.display=((s.type==='cisterna'||s.type==='reparto')?'':'none');}
  document.getElementById('modal').classList.add('open');
  document.getElementById('f-no-entregado').checked=!!s.noEntregado;
  document.getElementById('f-observaciones').value=s.observaciones||'';
  refreshCisternaSelect();
}
 
 
function openEditSalida(dayKey,driverId,salidaNum){const first=services.find(s=>s.day===dayKey&&s.driver===driverId&&s.salida===salidaNum);if(first)openEdit(first.id);}
function populateFormSilent(type,dayKey){
  document.getElementById('f-type').value=type||'reparto';
  const dEl=document.getElementById('f-driver');dEl.innerHTML='';
  DRIVERS.forEach(d=>{if(type==='cisterna'&&!d.canTractora)return;const o=document.createElement('option');o.value=d.id;o.textContent=d.name;dEl.appendChild(o);});
  const dyEl=document.getElementById('f-day');dyEl.innerHTML='';
  [-2,-1,0,1,2].flatMap(getDaysForWeek).filter(d=>!isWeekend(d)).forEach(d=>{const o=document.createElement('option');o.value=dateKey(d);o.textContent=DAYS_ES[d.getDay()]+' '+d.getDate()+' '+MONTHS_ES[d.getMonth()];if(dayKey&&dateKey(d)===dayKey)o.selected=true;dyEl.appendChild(o);});
  const d2El=document.getElementById('f-driver2');d2El.innerHTML='';DRIVERS.forEach(d=>{const o=document.createElement('option');o.value=d.id;o.textContent=d.name;d2El.appendChild(o);});
}
function populateFormNew(type,dayKey){
  populateFormSilent(type,dayKey);
  buildVehicleOptions(document.getElementById('f-vehicle'),type,document.getElementById('f-driver').value);
  document.getElementById('row-vehicle-select').style.display='';document.getElementById('row-vehicle-locked').style.display='none';
  updateSalidaOptions();onTypeChangeSilent(type);
}
function onTypeChangeSilent(type){
  const two=document.getElementById('f-twoperson').checked;
  document.getElementById('row-twoperson').style.display=(type==='cisterna'||type==='reparto')?'':'none';
  document.getElementById('row-second').style.display=((type==='cisterna'||type==='reparto')&&two)?'':'none';
  document.getElementById('row-zones').style.display=type==='reparto'?'':'none';
  document.getElementById('row-productos').style.display=(type==='reparto'||type==='cisterna')?'':'none';
  document.getElementById('row-muni').style.display=(type==='reparto'||type==='cisterna')?'':'none';
  document.getElementById('row-no-entregado').style.display=(type==='cisterna'||type==='reparto')?'':'none';
  document.getElementById('row-observaciones').style.display=(type==='cisterna'||type==='reparto')?'':'none';
  document.getElementById('row-trayecto-cisterna').style.display=type==='cisterna'?'':'none';
}
function onTypeChange(){
  const type=document.getElementById('f-type').value;onTypeChangeSilent(type);
  if(type==='cisterna'){
    document.getElementById('f-merch-type').value='cisterna';
    onMerchTypeChange();
  }
  if(!isEditing){
    if(type==='reparto')updateSuggestion();else document.getElementById('suggestion-box').style.display='none';
    const dEl=document.getElementById('f-driver');
    const cur=dEl.value;dEl.innerHTML='';
    DRIVERS.forEach(d=>{if(type==='cisterna'&&!d.canTractora)return;const o=document.createElement('option');o.value=d.id;o.textContent=d.name;if(d.id==cur)o.selected=true;dEl.appendChild(o);});
    buildVehicleOptions(document.getElementById('f-vehicle'),type,dEl.value);
    document.getElementById('row-vehicle-select').style.display='';document.getElementById('row-vehicle-locked').style.display='none';
    updateSalidaOptions();
  }
  refreshCisternaSelect();
}
document.getElementById('f-twoperson').addEventListener('change',()=>{const type=document.getElementById('f-type').value;
const two=document.getElementById('f-twoperson').checked;document.getElementById('row-second').style.display=((type==='cisterna'||type==='reparto')&&two)?'':'none';});
document.getElementById('f-day').addEventListener('change',()=>{if(!isEditing)updateSuggestion();onDriverOrDayChange();refreshCisternaSelect();});
document.getElementById('f-driver').addEventListener('change',()=>{onDriverOrDayChange();});
document.getElementById('f-salida').addEventListener('change',()=>{onSalidaChange();});
document.addEventListener('click',function(e){if(!e.target.closest('.muni-wrap'))document.getElementById('muni-list').style.display='none';});
 
 
// ---- SAVE / DELETE ----
function saveService(){
  const type=document.getElementById('f-type').value;
  const driver=parseInt(document.getElementById('f-driver').value);
  const day=document.getElementById('f-day').value;const client=document.getElementById('f-client').value.trim();
  const muni=document.getElementById('f-muni').value.trim();
  const direccion=document.getElementById('f-direccion').value.trim();
  const pedidoNum=document.getElementById('f-pedido').value.trim()||null;
  const merchType=document.getElementById('f-merch-type').value||null;
  const merchQty=document.getElementById('f-merch-qty').value?Number(document.getElementById('f-merch-qty').value):null;
  const merchManual=document.getElementById('f-merch-manual').value?Number(document.getElementById('f-merch-manual').value):null;
  const horaSalida=document.getElementById('f-hora-salida').value||null;
  const twoPerson=document.getElementById('f-twoperson').checked;
  const driver2=twoPerson?parseInt(document.getElementById('f-driver2').value):null;
  const zones=type==='reparto'?getSelectedZones():[];
  const salida=parseInt(document.getElementById('f-salida').value)||1;
  const noEntregado=document.getElementById('f-no-entregado').checked;
  const observaciones=document.getElementById('f-observaciones').value.trim();
  const vehicle=document.getElementById('f-vehicle').value;
  const origenTrayecto=type==='cisterna'?(document.getElementById('f-origen-trayecto').value||'base'):'base';
  const destinoTrayecto=type==='cisterna'?(document.getElementById('f-destino-trayecto').value||'base'):'base';
  const productos=readProductos();
  const entregadosArticles=productos.filter(p=>p.entregado).map(({artCod,qty})=>({artCod,qty}));
  const pendientesArticles=productos.filter(p=>!p.entregado).map(({artCod,qty})=>({artCod,qty}));
  if(horaSalida){
         const prospectiveItem={muni,merchType,merchQty,merchManual,client,origenTrayecto,destinoTrayecto};
         const dur=calcSalidaDuration(buildProspectiveSalida(day,driver,salida,editingId,prospectiveItem)).totalMin;
         const endTime=addMinutes(horaSalida,dur);
 
         if(salida>1){
             const prevEnd=getSalidaEndTime(day,driver,salida-1);
             if(prevEnd&&horaSalida<prevEnd){
                  alert(`Se solapa con la salida anterior de ${DRIVER_BY_ID.get(driver)?.name} (finaliza a las ${prevEnd}).`);
                  return;
             }
         }
 
         const driverConflict=getPersonBusyIntervals(day,driver,driver,salida)
            .find(iv=>intervalsOverlap(horaSalida,endTime,iv.start,iv.end));
         if(driverConflict){
              alert(`${DRIVER_BY_ID.get(driver)?.name} ya tiene otra salida entre ${driverConflict.start} y ${driverConflict.end}.`);
              return;
         }
 
         if(twoPerson&&driver2){
              const helperConflict=getPersonBusyIntervals(day,driver2,driver,salida)
                 .find(iv=>intervalsOverlap(horaSalida,endTime,iv.start,iv.end));
              if(helperConflict){
                  alert(`${DRIVER_BY_ID.get(driver2)?.name} (ayudante) ya tiene otra salida entre ${helperConflict.start} y ${helperConflict.end}.`);
                  return;
               }
          }
      }
  
 
  // Validar: PACO (id=4) no puede usar tractoras
  const vh=VEHICLE_BY_ID.get(vehicle);
  if(driver===4&&vh&&vh.type==='tractora'){
    alert('PACO no tiene licencia de tractor. No puede usar tractoras.');
    return;
  }
  const newId=editingId||Date.now();
  const action=editingId?'edicion':'alta';
  if(editingId){
    const idx=services.findIndex(s=>s.id===editingId);if(idx>-1){
      const finalArticles=entregadosArticles;
      services[idx]={
        ...services[idx],pedidoNum,type,driver,vehicle,day,client,muni,twoPerson,driver2,zones,salida,assigned:true,merchType,merchQty,merchManual,horaSalida,noEntregado,observaciones,articles:finalArticles,origenTrayecto,destinoTrayecto,direccion};
    }
  } else services.push({id:newId,pedidoNum,type,driver,vehicle,day,client,muni,twoPerson,driver2,zones,salida,merchType,merchQty,merchManual,horaSalida,articles:entregadosArticles, noEntregado, observaciones,origenTrayecto,destinoTrayecto,direccion,cisternaCuba:null,cisternaPlataforma:null});
  if(type==='cisterna'){
    const selectedCuba=document.getElementById('f-cisterna-cuba')?.value||'';
    const current=services.find(s=>s.id===newId);
    if(current){
      if(selectedCuba&&current.cisternaCuba!==selectedCuba){
        assignCisternaToService(newId,selectedCuba);
      }else if(!selectedCuba&&current.cisternaCuba){
        releaseCisternaFromService(newId,false);
      }else if(selectedCuba&&current.cisternaCuba===selectedCuba){
        const c=cisternaStock.find(x=>cisternaKey(x.cuba)===cisternaKey(selectedCuba));
        if(c){c.fechaEntregaAsignada=day;c.clienteAsignado=client;c.choferAsignado=DRIVER_BY_ID.get(driver)?.name||'';}
      }
    }
    writeCisternaStock();
  }
  if(editingId&&noEntregado&&pendientesArticles.length){
    const orig=services.find(s=>s.id===editingId);
    services.push({
      ...orig,
      id:Date.now(),
      articles:pendientesArticles,
      noEntregado:false,
      observaciones:'',
      servido:false,
      day:orig.day,
      driver:null,
      salida:1,
      vehicle:null,
      cisternaCuba:null,
      cisternaPlataforma:null
    });
  }
if(horaSalida){
  services.filter(s=>s.day===day&&s.driver===driver&&s.salida===salida).forEach(s=>{s.horaSalida=horaSalida;});
  cascadeSalidaTimes(day,driver,salida);
}
ensureOrsData();
saveData(action,{summary:`${pedidoNum?'#'+pedidoNum+' ':''}${client||'Pedido sin cliente'}`,day,pedidoNum,type});
  closeModal();renderCalendar();
}
function deleteService(){const deleted=services.find(s=>s.id===editingId);if(deleted&&deleted.cisternaCuba)releaseCisternaFromService(deleted.id,false);services=services.filter(s=>s.id!==editingId);if(deleted)compactDriverSalidas(deleted.day,deleted.driver,deleted.salida);writeCisternaStock();saveData('borrado',{summary:deleted?`${deleted.pedidoNum?'#'+deleted.pedidoNum+' ':''}${deleted.client||'Pedido eliminado'}`:'Pedido eliminado',day:deleted?.day,pedidoNum:deleted?.pedidoNum,type:deleted?.type});closeModal();renderCalendar();renderCisternaFleetIfOpen();}
function closeModal(){document.getElementById('modal').classList.remove('open');editingId=null;isEditing=false;}
 
// ---- BLOQUE INDEPENDIENTE: ARRASTRE BASE ⇄ PUERTO ----
// No es un pedido de cliente, pero SÍ cuenta como una salida más dentro de la secuencia numerada
// del conductor para ese día (ver nextSalidaNumberForDriver): así el Timeline y el desplegable
// "Salida" del formulario de pedido reflejan el hueco real que ocupa. Se sigue guardando como
// service{type:'arrastre'} y queda excluido de getSalidasForDay() (no se agrupa como bloque de
// salida normal, tiene su propio bloque "⛴ arrastre") y de countDriverLoad()/el límite de 2
// cisternas al día (eso es un límite de entregas, no de movimientos de tractor).
let editingArrastreId=null;
function arrastreTrayectoField(s){
  // Tolerante con distintos nombres de campo que pudieran existir en datos ya guardados.
  return s.trayectoArrastre||s.trayecto||'base_puerto';
}
function arrastreTrayectoLabel(trayecto){
  return trayecto==='puerto_base'?'⛴ Puerto → Base':'⛴ Base → Puerto';
}
// Siguiente número de salida libre para un conductor en un día, contando TODO lo que ya tenga
// (salidas normales + arrastres) — es lo que hace que un arrastre "cuente como salida".
function nextSalidaNumberForDriver(day,driverId,excludeId){
  const dayServices=services.filter(s=>s.day===day&&s.id!==excludeId);
  return getMaxSalidaForDriver(day,driverId,dayServices)+1;
}
// Al borrar una salida (pedido o arrastre) puede quedar un hueco en la numeración de ese
// conductor/día (p.ej. si queda 1, 3 tras borrar la 2). Esto renumera automáticamente hacia
// abajo todo lo que hubiera por encima del hueco (salidas normales Y arrastres), para que la
// secuencia vuelva a ser contigua y los arrastres posteriores "se muevan" de salida solos.
function compactDriverSalidas(day,driverId,removedSalida){
  if(driverId==null||removedSalida==null)return;
  const stillOccupied=services.some(s=>s.day===day&&s.driver===driverId&&s.salida===removedSalida);
  if(stillOccupied)return; // el hueco no está realmente vacío (aún queda algo con ese número)
  services.forEach(s=>{
    if(s.day===day&&s.driver===driverId&&s.salida>removedSalida)s.salida-=1;
  });
}
// Recalcula las opciones del selector manual de salida del modal de arrastre: cada arrastre
// ocupa su propio hueco en exclusiva (no comparte salida con otros pedidos), así que aquí solo
// se listan los números libres (más el siguiente nuevo), deshabilitando los ya ocupados.
function updateArrastreSalidaOptions(){
  const sel=document.getElementById('fa-salida');
  if(!sel)return;
  const day=document.getElementById('fa-day').value;
  const driver=parseInt(document.getElementById('fa-driver').value);
  const orig=editingArrastreId?services.find(s=>s.id===editingArrastreId):null;
  sel.innerHTML='';
  if(!day||!driver)return;
  const dayServices=servicesForDay(day).filter(s=>s.id!==editingArrastreId);
  const max=getMaxSalidaForDriver(day,driver,dayServices);
  for(let i=1;i<=max+1;i++){
    const occupied=dayServices.some(s=>s.driver===driver&&s.salida===i);
    const o=document.createElement('option');
    o.value=i;
    o.textContent=occupied?`${ORDINALS[i-1]||i+'ª'} — ocupado`:`${ORDINALS[i-1]||i+'ª'}${i<=max?'':' (nueva)'}`;
    if(occupied)o.disabled=true;
    sel.appendChild(o);
  }
  const savedVal=orig&&orig.driver===driver&&orig.day===day?orig.salida:null;
  const savedOk=savedVal&&[...sel.options].some(o=>Number(o.value)===savedVal&&!o.disabled);
  sel.value=savedOk?savedVal:(max+1);
}
// Cubas candidatas para el selector del modal de arrastre: no se excluyen (se puede forzar a
// mano, como el resto de selectores de cuba del planificador), pero se ordenan primero las que
// están en la ubicación esperada según el trayecto elegido (Base→Puerto necesita una cuba en
// base; Puerto→Base necesita una cuba en puerto) y se marca con ⚠ la que no encaja.
function updateArrastreCubaOptions(){
  const trayectoEl=document.getElementById('fa-trayecto');
  const cubaEl=document.getElementById('fa-cuba');
  if(!trayectoEl||!cubaEl)return;
  const expected=trayectoEl.value==='puerto_base'?'puerto':'base';
  const current=cubaEl.value;
  const list=cisternaStock.slice().map(c=>({c,loc:cisternaMallorcaLocation(c)}))
    .sort((a,b)=>{
      const matchA=a.loc===expected,matchB=b.loc===expected;
      if(matchA!==matchB)return matchA?-1:1;
      return a.c.cuba.localeCompare(b.c.cuba);
    });
  cubaEl.innerHTML='<option value="">— sin especificar —</option>';
  list.forEach(({c,loc})=>{
    const o=document.createElement('option');
    o.value=c.cuba;
    const locLabel=loc==='puerto'?'Puerto':loc==='base'?'Base':'sin ubicar';
    o.textContent=`${c.cuba}${c.plataforma?' ('+c.plataforma+')':''} — ${locLabel}${loc&&loc!==expected?' ⚠':''}`;
    cubaEl.appendChild(o);
  });
  if(current&&[...cubaEl.options].some(o=>o.value===current))cubaEl.value=current;
}
function openAddArrastre(dayKey){
  editingArrastreId=null;
  document.getElementById('arrastre-modal-title').textContent='Arrastre Base ⇄ Puerto';
  document.getElementById('fa-btn-delete').style.display='none';
  const dyEl=document.getElementById('fa-day');dyEl.innerHTML='';
  [-2,-1,0,1,2].flatMap(getDaysForWeek).filter(d=>!isWeekend(d)).forEach(d=>{
    const o=document.createElement('option');o.value=dateKey(d);o.textContent=DAYS_ES[d.getDay()]+' '+d.getDate()+' '+MONTHS_ES[d.getMonth()];
    if(dayKey&&dateKey(d)===dayKey)o.selected=true;
    dyEl.appendChild(o);
  });
  const drEl=document.getElementById('fa-driver');drEl.innerHTML='';
  DRIVERS.filter(d=>d.canTractora).forEach(d=>{const o=document.createElement('option');o.value=d.id;o.textContent=d.name;drEl.appendChild(o);});
  const vEl=document.getElementById('fa-vehicle');vEl.innerHTML='<option value="">— sin especificar —</option>';
  VEHICLES.filter(v=>v.type==='tractora').forEach(v=>{const o=document.createElement('option');o.value=v.id;o.textContent=v.name;vEl.appendChild(o);});
  document.getElementById('fa-trayecto').value='base_puerto';
  updateArrastreCubaOptions();
  document.getElementById('fa-hora').value='';
  document.getElementById('fa-observaciones').value='';
  updateArrastreSalidaOptions();
  document.getElementById('arrastre-modal').classList.add('open');
}
function openEditArrastre(id){
  const s=services.find(x=>x.id===id);
  if(!s)return;
  openAddArrastre(s.day);
  editingArrastreId=id;
  document.getElementById('arrastre-modal-title').textContent='Editar arrastre';
  document.getElementById('fa-btn-delete').style.display='';
  document.getElementById('fa-day').value=s.day;
  document.getElementById('fa-driver').value=s.driver||'';
  document.getElementById('fa-vehicle').value=s.vehicle||'';
  document.getElementById('fa-trayecto').value=arrastreTrayectoField(s);
  updateArrastreCubaOptions();
  const cubaEl=document.getElementById('fa-cuba');
  if(s.cisternaCuba&&![...cubaEl.options].some(o=>o.value===s.cisternaCuba)){
    const o=document.createElement('option');o.value=s.cisternaCuba;o.textContent=`${s.cisternaCuba} (no está en la flota registrada)`;cubaEl.appendChild(o);
  }
  cubaEl.value=s.cisternaCuba||'';
  document.getElementById('fa-hora').value=s.horaSalida||'';
  document.getElementById('fa-observaciones').value=s.observaciones||'';
  updateArrastreSalidaOptions();
}
function closeArrastreModal(){document.getElementById('arrastre-modal').classList.remove('open');editingArrastreId=null;}
function saveArrastre(){
  const day=document.getElementById('fa-day').value;
  const driver=parseInt(document.getElementById('fa-driver').value);
  const vehicle=document.getElementById('fa-vehicle').value||null;
  const trayectoArrastre=document.getElementById('fa-trayecto').value;
  const cisternaCuba=document.getElementById('fa-cuba').value||null;
  const horaSalida=document.getElementById('fa-hora').value||null;
  const observaciones=document.getElementById('fa-observaciones').value.trim();
  if(!day||!driver){alert('Selecciona día y conductor.');return;}
  const salidaSel=document.getElementById('fa-salida');
  let salida=salidaSel&&salidaSel.value?parseInt(salidaSel.value):null;
  if(!salida)salida=nextSalidaNumberForDriver(day,driver,editingArrastreId);
  const conflict=services.find(s=>s.id!==editingArrastreId&&s.day===day&&s.driver===driver&&s.salida===salida);
  if(conflict){alert('Esa salida ya está ocupada. Elige otra en el desplegable "Salida".');return;}
  if(editingArrastreId){
    const idx=services.findIndex(s=>s.id===editingArrastreId);
    if(idx>-1){
      const oldDay=services[idx].day,oldDriver=services[idx].driver,oldSalida=services[idx].salida;
      services[idx]={...services[idx],day,driver,vehicle,trayectoArrastre,cisternaCuba,horaSalida,observaciones,salida};
      if(oldDay!==day||oldDriver!==driver||oldSalida!==salida)compactDriverSalidas(oldDay,oldDriver,oldSalida);
    }
  }else{
    services.push({
      id:Date.now()+Math.random(),type:'arrastre',day,driver,driver2:null,twoPerson:false,
      vehicle,trayectoArrastre,cisternaCuba,horaSalida,observaciones,
      salida,client:null,muni:null,zones:[],articles:[]
    });
  }
  saveData(editingArrastreId?'edicion':'alta',{summary:`Arrastre Base⇄Puerto (${DRIVER_BY_ID.get(driver)?.name||''})`,day,type:'arrastre'});
  closeArrastreModal();
  renderCalendar();
}
function deleteArrastre(){
  if(!editingArrastreId)return;
  const deleted=services.find(s=>s.id===editingArrastreId);
  services=services.filter(s=>s.id!==editingArrastreId);
  if(deleted)compactDriverSalidas(deleted.day,deleted.driver,deleted.salida);
  saveData('borrado',{summary:'Arrastre Base⇄Puerto eliminado',day:deleted?.day,type:'arrastre'});
  closeArrastreModal();
  renderCalendar();
}
document.getElementById('modal').addEventListener('click',function(e){if(e.target===this)closeModal();});
 
function escapeHtml(value){
  return String(value??'').replace(/[&<>"']/g,ch=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[ch]));
}
function downloadFile(name,content,type){
  const blob=new Blob([content],{type});
  const url=URL.createObjectURL(blob);
  const a=document.createElement('a');
  a.href=url;
  a.download=name;
  document.body.appendChild(a);
  a.click();
  a.remove();
  setTimeout(()=>URL.revokeObjectURL(url),1000);
}
 
async function exportBackup(){
  const stamp=new Date().toISOString().slice(0,10);
  try{
    const backup=await readBackupData(false);
    downloadFile(`planificador_backup_${stamp}.json`,JSON.stringify(backup),'application/json;charset=utf-8');
  }catch(e){
    console.error('Copia de seguridad:',e);
    // Último recurso: copia sin histórico, que siempre es pequeña
    try{
      const backup=await readBackupData(false);
      backup.history=[];
      downloadFile(`planificador_backup_${stamp}_sin_historico.json`,JSON.stringify(backup),'application/json;charset=utf-8');
      alert('El histórico es demasiado grande y se ha guardado la copia SIN histórico.\nMotivo: '+e.message);
    }catch(e2){
      alert('No se pudo crear la copia de seguridad: '+e2.message);
    }
  }
} 
function handleBackupImport(event){
  const file=event.target.files[0];
  event.target.value='';
  if(!file)return;
  const reader=new FileReader();
  reader.onload=async e=>{
    try{
      const backup=JSON.parse(e.target.result);
      if(!backup||!Array.isArray(backup.services))throw new Error('Formato no valido');
      if(!confirm('Esto sustituira los repartos actuales por los de la copia seleccionada.'))return;
      services=backup.services;
      if(backup.clientPins&&backup.clientPins.pins){clientPins=backup.clientPins.pins;pinsTried=backup.clientPins.tried||{};saveClientPins();}
      if(Array.isArray(backup.cisternaStock)){cisternaStock=backup.cisternaStock;await writeCisternaStock();}
      if(backup.productPort&&typeof backup.productPort==='object'){productPort=backup.productPort;await saveProductPort();}
      if(Array.isArray(backup.registroMovimientos)){registroMovimientos=backup.registroMovimientos;await writeRegistroMovimientos();}
      await writeCurrentServices();
      await replaceHistoryEntries(Array.isArray(backup.history)?backup.history:[]);
      await addHistoryEntry('restauracion',{summary:`Copia restaurada (${services.length} pedidos)`,affectedCount:services.length});
      renderCalendar();
      alert('Copia restaurada correctamente.');
    }catch(err){
      alert('No se pudo restaurar la copia. Revisa que sea un archivo de copia del planificador.');
    }
  };
  reader.readAsText(file);
}
 
function actionLabel(action){
  return {
    alta:'Alta',
    edicion:'Edicion',
    borrado:'Borrado',
    importacion:'Importacion',
    servido:'Servido',
    reordenacion:'Reordenacion',
    estado_inicial:'Estado inicial',
    restauracion:'Restauracion',
    migracion:'Migracion',
    cambio:'Cambio'
  }[action]||'Cambio';
}
 
function formatDateTime(iso){
  if(!iso)return '';
  const d=new Date(iso);
  if(isNaN(d))return iso;
  return d.toLocaleString('es-ES',{day:'2-digit',month:'2-digit',year:'numeric',hour:'2-digit',minute:'2-digit'});
}
 
async function openHistory(){
  document.getElementById('history-modal').classList.add('open');
  await renderHistory();
}
 
function closeHistory(){
  document.getElementById('history-modal').classList.remove('open');
}
 
async function renderHistory(){
  const tbody=document.getElementById('history-tbody');
  const summary=document.getElementById('history-summary');
  tbody.innerHTML='<tr><td colspan="5">Cargando historico...</td></tr>';
  try{
    const entries=await getHistoryEntries(300);
    summary.textContent=entries.length?`${entries.length} cambios guardados en base de datos`:'Todavia no hay historico guardado.';
    if(!entries.length){tbody.innerHTML='<tr><td colspan="5">Sin movimientos registrados.</td></tr>';return;}
    tbody.innerHTML='';
    entries.forEach(entry=>{
      const tr=document.createElement('tr');
      tr.innerHTML=`
        <td>${formatDateTime(entry.createdAt||entry.updatedAt)}</td>
        <td><span class="history-action">${actionLabel(entry.action)}</span></td>
        <td>${escapeHtml(entry.summary||'')}</td>
        <td>${escapeHtml(entry.day||'')}</td>
        <td>${entry.serviceCount||0}</td>`;
      tbody.appendChild(tr);
    });
  }catch(e){
    summary.textContent='No se pudo leer el historico.';
    tbody.innerHTML='<tr><td colspan="5">Error leyendo la base de datos.</td></tr>';
  }
}
 
async function exportHistoryExcel(){
  const entries=await getHistoryEntries(10000);
  if(!entries.length){alert('No hay historico para exportar.');return;}
  const rows=[['Fecha guardado','Accion','Resumen','Fecha reparto','Nº Pedido','Tipo','Pedidos en planificador']];
  entries.forEach(entry=>{
    rows.push([
      formatDateTime(entry.createdAt||entry.updatedAt),
      actionLabel(entry.action),
      entry.summary||'',
      entry.day||'',
      entry.pedidoNum||'',
      entry.type||'',
      entry.serviceCount||0
    ]);
  });
  if(typeof XLSX!=='undefined'){
    const ws=XLSX.utils.aoa_to_sheet(rows);
    ws['!cols']=[{wch:18},{wch:14},{wch:36},{wch:12},{wch:14},{wch:14},{wch:18}];
    const wb=XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb,ws,'Historico');
    const wbout=XLSX.write(wb,{bookType:'xlsx',type:'array'});
    const blob=new Blob([wbout],{type:'application/octet-stream'});
    const url=URL.createObjectURL(blob);
    const a=document.createElement('a');
    a.href=url;a.download='historico_repartos.xlsx';
    document.body.appendChild(a);a.click();a.remove();
    setTimeout(()=>URL.revokeObjectURL(url),1000);
    return;
  }
  const html=`<!doctype html><html><head><meta charset="utf-8"></head><body><table border="1">${rows.map((row,i)=>`<tr>${row.map(cell=>i===0?`<th>${escapeHtml(String(cell))}</th>`:`<td>${escapeHtml(String(cell))}</td>`).join('')}</tr>`).join('')}</table></body></html>`;
  downloadFile('historico_repartos.xls',html,'application/vnd.ms-excel;charset=utf-8');
}
 
 
// ---- EXPORT ----
function exportExcel(){
  if(!services.length){alert('No hay pedidos para exportar.');return;}
  const TYPE_LABELS={cisterna:'Cisterna Mallorca',islas:'Cisterna Islas',tasmar:'Agencia Tasmar (ADR)',luchana:'Agencia Luchana',reparto:'Reparto propio'};
  const rows=[['Nº Pedido','Fecha','Día','Tipo','Conductor','Nº Salida','Vehículo','Cliente','Municipio','Zonas','Destino','2 personas','2º Conductor','Estado','Servido','Cuba asignada','Artículos']];
  const sorted=[...services].sort((a,b)=>{
    const da=(a.day||''), db=(b.day||'');
    if(da!==db) return da.localeCompare(db);
    const dra=(a.driver||0), drb=(b.driver||0);
    if(dra!==drb) return dra-drb;
    return (a.salida||0)-(b.salida||0);
  });
  sorted.forEach(s=>{
    const dr=DRIVER_BY_ID.get(s.driver);
    const vh=VEHICLE_BY_ID.get(s.vehicle);
    const dr2=s.driver2?DRIVER_BY_ID.get(s.driver2):null;
    const d=new Date((s.day||'')+'T00:00:00');
    const dayLabel=s.day&&!isNaN(d)?DAYS_ES[d.getDay()]+' '+d.getDate()+' '+MONTHS_ES[d.getMonth()]:'';
    const tipoLabel=TYPE_LABELS[s.type]||'Sin clasificar';
    const salidaLabel=s.salida?(ORDINALS[s.salida-1]||s.salida+'ª'):'';
    const estado=!s.type?'Sin clasificar':s.driver?'Asignado':'Pendiente asignación';
    rows.push([
      s.pedidoNum||'',
      s.day||'',
      dayLabel,
      tipoLabel,
      dr?dr.name:'Sin asignar',
      salidaLabel,
      vh?vh.name:'',
      s.client||'',
      s.muni||'',
      (s.zones||[]).filter(function(z){return z&&typeof z==='string';}).join(', '),
      s.destino||'',
      s.twoPerson?'Sí':'No',
      dr2?dr2.name:'',
      estado,
      s.servido?'Sí':'No',
      s.cisternaCuba||'',
      (s.articles||[]).map(a=>a.artCod+(a.qty?' ×'+a.qty:'')).join(' | ')
    ]);
  });
  console.log("hola");
  if(typeof XLSX!=='undefined'){
    // Ensure every cell is a safe type (string or number) — never null/undefined/object
    const safeRows = rows.map(row =>
        row.map(cell => {
            if(cell === null || cell === undefined) return '';
 
            if(Array.isArray(cell))
                return cell.join(', ');
 
            if(typeof cell === 'object')
                return JSON.stringify(cell);
 
            if(typeof cell === 'boolean')
                return cell ? 'Sí' : 'No';
 
            return String(cell);
      })
    );
    safeRows.forEach((row,i)=>{
       row.forEach((cell,j)=>{
           if(typeof cell !== 'string' && typeof cell !== 'number'){
              console.log('Fila',i,'Columna',j,'Valor:',cell);
           }
       });
    });
    const ws=XLSX.utils.aoa_to_sheet(safeRows);
    safeRows.forEach((row,i)=>{
        console.log("FILA",i,row);
    });
    ws['!cols']=[{wch:10},{wch:12},{wch:14},{wch:22},{wch:12},{wch:8},{wch:10},{wch:26},{wch:20},{wch:28},{wch:10},{wch:10},{wch:12},{wch:18}];
    const wb=XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws,'Histórico');
    let wbout;
 
    try {
        wbout = XLSX.write(wb,{
             bookType:'xlsx',
             type:'array'
        });
    } catch(err) {
        console.error(err);
        alert('Error exportando Excel: ' + err.message);
        return;
    }
    const blob=new Blob([wbout],{type:'application/octet-stream'});
    const url=URL.createObjectURL(blob);
    const a=document.createElement('a');
    a.href=url;a.download='reparto_historico.xlsx';
    document.body.appendChild(a);
 
    const event = new MouseEvent('click', {
        view: window,
        bubbles: true,
        cancelable: true
    });
 
    a.dispatchEvent(event);
    setTimeout(function(){URL.revokeObjectURL(url);a.remove();},1000);
    return;
  }
  const html=`<!doctype html><html><head><meta charset="utf-8"></head><body><table border="1">${rows.map((row,i)=>`<tr>${row.map(cell=>i===0?`<th>${escapeHtml(String(cell))}</th>`:`<td>${escapeHtml(String(cell))}</td>`).join('')}</tr>`).join('')}</table></body></html>`;
  downloadFile('reparto_historico.xls',html,'application/vnd.ms-excel;charset=utf-8');
}
 
async function loadFleetData(){
  try{
    cisternaStock=await readCisternaStock();
    if(!Array.isArray(cisternaStock))cisternaStock=[];
  }catch(e){cisternaStock=[];}
  const removed=dedupeCisternaStock();
  const staleReleased=releaseStaleCisternaAssignments();
  if(removed>0||staleReleased>0)await writeCisternaStock();
  await loadProductPort();
  try{
    registroMovimientos=await readRegistroMovimientos();
    if(!Array.isArray(registroMovimientos))registroMovimientos=[];
  }catch(e){registroMovimientos=[];}
  const purged=purgeRegistroMovBeforeCutoff();
  if(purged>0)await writeRegistroMovimientos();
}
// ---- CARGA AUTOMÁTICA DE EXCEL AL ABRIR ----
const AUTO_EXCEL_FILES=['Pedidos.xlsx','Movimiento_Cubas_2026_dashboard.xlsx'];
const EXCEL_DIR_KEY='excel_dir_handle';

async function saveExcelDirHandle(handle){
  const db=await openDatabase();
  if(db)await idbTx(db,CURRENT_STORE,'readwrite',s=>{s.put({key:EXCEL_DIR_KEY,handle});});
}
async function loadExcelDirHandle(){
  const db=await openDatabase();
  if(!db)return null;
  const r=await idbTx(db,CURRENT_STORE,'readonly',s=>idbRequest(s.get(EXCEL_DIR_KEY)));
  return r?r.handle:null;
}
function setAutoLoadStatus(msg,showBtn){
  const ind=document.getElementById('save-indicator');
  if(ind)ind.textContent=msg;
  const btn=document.getElementById('btn-reload-excel');
  if(btn)btn.style.display=showBtn?'':'none';
}
// Vía 1: fetch relativo (funciona si el planificador se sirve por http/https)
async function autoLoadViaFetch(){
  let loaded=0;
  for(const name of AUTO_EXCEL_FILES){
    try{
      const res=await fetch(encodeURI(name),{cache:'no-store'});
      if(!res.ok)continue;
      processExcelBuffer(await res.arrayBuffer(),{silent:true});
      loaded++;
    }catch(e){/* file:// o archivo inexistente */}
  }
  return loaded;
}
// Vía 2: carpeta elegida una vez (File System Access API)
async function autoLoadViaFolder(userGesture){
  if(!window.showDirectoryPicker)return 0;
  let dir=null;
  try{dir=await loadExcelDirHandle();}catch(e){return 0;}
  if(!dir)return 0;
  let perm=await dir.queryPermission({mode:'read'});
  if(perm!=='granted'){
    if(!userGesture){setAutoLoadStatus('Excel: pulsa "Recargar Excel" para dar permiso',true);return 0;}
    perm=await dir.requestPermission({mode:'read'});
    if(perm!=='granted')return 0;
  }
  let loaded=0;
  for(const name of AUTO_EXCEL_FILES){
    try{
      const fh=await dir.getFileHandle(name);
      const file=await fh.getFile();
      processExcelBuffer(await file.arrayBuffer(),{silent:true});
      loaded++;
    }catch(e){console.warn('Excel no encontrado en la carpeta:',name);}
  }
  return loaded;
}
async function pickExcelFolder(){
  if(!window.showDirectoryPicker){alert('Tu navegador no permite elegir carpeta (usa Chrome o Edge).');return;}
  try{
    const dir=await window.showDirectoryPicker({mode:'read'});
    await saveExcelDirHandle(dir);
    await runAutoLoadExcels(true);
  }catch(e){/* cancelado */}
}
async function runAutoLoadExcels(userGesture){
  if(typeof XLSX==='undefined')return;
  let n=await autoLoadViaFetch();
  if(!n)n=await autoLoadViaFolder(!!userGesture);
  if(n){
    setAutoLoadStatus(`✓ ${n} Excel cargado(s) automáticamente`,false);
    setTimeout(()=>setAutoLoadStatus('',false),4000);
    if(typeof ensureOrsData==='function')ensureOrsData();
  }
}

loadData();
loadFleetData().then(()=>loadOrsCache()).then(()=>loadClientPins()).then(()=>loadRefPoints()).then(()=>{renderCalendar();renderCisternaFleetIfOpen();renderRegistroMovIfOpen();ensureOrsData();}).then(()=>runAutoLoadExcels(false));
 
// ---- TIMELINE ----
function openTimeline() {
  const days = getDaysForWeek(currentWeek).filter(d => !isWeekend(d));
  // Default to today if in current week, else first day
  const todayKey = dateKey(new Date());
  const defaultDay = days.find(d => dateKey(d) === todayKey) || days[0];
  renderTimeline(defaultDay ? dateKey(defaultDay) : dateKey(days[0]));
  document.getElementById('timeline-modal').classList.add('open');
}
 
function closeTimeline() {
  document.getElementById('timeline-modal').classList.remove('open');
}
 
function renderTimeline(dayKey) {
  currentTimelineDayKey = dayKey;
     const days = getDaysForWeek(currentWeek).filter(d => !isWeekend(d));
  const dayServices = servicesForDay(dayKey);
  const salidas = getSalidasForDay(dayKey, dayServices);
  const arrastresDelDia = dayServices.filter(s => s.type === 'arrastre');
  const arDur = calcArrastreDuration();
  const d = new Date(dayKey + 'T00:00:00');
 
  document.getElementById('timeline-title').textContent =
    `Timeline — ${DAYS_ES[d.getDay()]} ${d.getDate()} ${MONTHS_ES[d.getMonth()]}`;
 
  // Day selector tabs
  const tabsHtml = days.map(day => {
    const k = dateKey(day);
    const active = k === dayKey;
    return `<button onclick="renderTimeline('${k}')" style="padding:4px 10px;font-size:12px;cursor:pointer;border-radius:var(--border-radius-md);border:0.5px solid ${active?'#185FA5':'var(--color-border-tertiary)'};background:${active?'#185FA5':'var(--color-background-secondary)'};color:${active?'#fff':'var(--color-text-secondary)'};">${DAYS_ES[day.getDay()]} ${day.getDate()}</button>`;
  }).join('');
 
  // Build timeline per driver
  const START_H = 7, END_H = 20; // timeline window 7:00–20:00
  const TOTAL_MIN = (END_H - START_H) * 60;
 
  function pct(min) { return Math.min(100, Math.max(0, (min / TOTAL_MIN) * 100)); }
  function minFromTime(t) { if(!t) return null; const [h,m]=t.split(':').map(Number); return (h-START_H)*60+m; }
 
  // Group salidas by driver
  const byDriver = {};
  salidas.forEach(sal => {
    if(!byDriver[sal.driver]) byDriver[sal.driver] = [];
    byDriver[sal.driver].push(sal);
    if(sal.twoPerson && sal.driver2) {
      if(!byDriver[sal.driver2]) byDriver[sal.driver2] = [];
      byDriver[sal.driver2].push({...sal, _ayudante:true});
    }
  });
  // Los arrastres Base⇄Puerto se intercalan en el timeline igual que las salidas normales,
  // como bloques propios (icono ⛴, esquema azul), ya que ocupan hueco real en la agenda del conductor.
  arrastresDelDia.forEach(s => {
    if(!byDriver[s.driver]) byDriver[s.driver] = [];
    byDriver[s.driver].push({...s, _arrastre:true, salida:s.salida});
  });
 
  // Hour markers
  const markers = [];
  for(let h = START_H; h <= END_H; h++) {
    markers.push(`<div style="position:absolute;left:${pct((h-START_H)*60)}%;top:0;bottom:0;border-left:0.5px solid var(--color-border-tertiary);pointer-events:none;"></div><div style="position:absolute;left:${pct((h-START_H)*60)}%;top:-16px;font-size:10px;color:var(--color-text-tertiary);transform:translateX(-50%);">${String(h).padStart(2,'0')}:00</div>`);
  }
 
  const driverRows = DRIVERS.map(driver => {
    const dSalidas = (byDriver[driver.id] || []).slice().sort((a,b)=>{
      const ha = a._arrastre ? (a.horaSalida||'') : (a.items[0]?.horaSalida||'');
      const hb = b._arrastre ? (b.horaSalida||'') : (b.items[0]?.horaSalida||'');
      if(ha && hb) return ha.localeCompare(hb);
      if(ha) return -1;
      if(hb) return 1;
      return (a.salida||0)-(b.salida||0);
    });
    if(!dSalidas.length) {
      return `<div style="display:flex;align-items:center;gap:8px;margin-bottom:8px;">
        <div style="width:80px;font-size:12px;font-weight:500;color:var(--color-text-secondary);flex-shrink:0;">${driver.name}</div>
        <div style="flex:1;height:32px;background:var(--color-background-secondary);border-radius:4px;display:flex;align-items:center;padding:0 8px;">
          <span style="font-size:11px;color:var(--color-text-tertiary);">Sin salidas</span>
        </div>
      </div>`;
    }
 
    const bars = dSalidas.map(sal => {
      if(sal._arrastre) {
        const horaSalida = sal.horaSalida || null;
        const startMin = horaSalida ? minFromTime(horaSalida) : null;
        const totalMin = arDur.min;
        const color = '#0C447C', bgColor = '#dceefb';
        const trayecto = arrastreTrayectoField(sal);
        const vh = VEHICLE_BY_ID.get(sal.vehicle);
        const cubaInfo = sal.cisternaCuba ? cisternaStock.find(c=>cisternaKey(c.cuba)===cisternaKey(sal.cisternaCuba)) : null;
        const label = `⛴ Arrastre ${horaSalida||'sin hora'} (${formatDuration(totalMin)})`;
        const fullTitle = `${label}\n${arrastreTrayectoLabel(trayecto)}${vh?' · '+vh.name:''}\n${sal.cisternaCuba?'Cuba '+sal.cisternaCuba+(cubaInfo&&cubaInfo.plataforma?' ('+cubaInfo.plataforma+')':''):'sin cuba asociada'}\nTotal: ${arDur.km}km, ${formatDuration(totalMin)}`;
        if(startMin == null) {
          return `<div style="display:inline-flex;align-items:center;gap:4px;background:${bgColor};color:${color};border:0.5px solid ${color}40;border-radius:4px;padding:2px 7px;font-size:11px;margin-right:4px;" title="${fullTitle}">
            ⛴ arrastre · ${formatDuration(totalMin)} · ${arDur.km}km
          </div>`;
        }
        const left = pct(startMin);
        const width = Math.max(2, pct(totalMin));
        return `<div style="position:absolute;left:${left}%;width:${width}%;top:4px;bottom:4px;background:${bgColor};border:1px dashed ${color};border-radius:4px;overflow:hidden;cursor:default;" title="${fullTitle}">
          <div style="padding:2px 5px;font-size:10px;color:${color};white-space:nowrap;overflow:hidden;text-overflow:ellipsis;">⛴ ${horaSalida}→${addMinutes(horaSalida,totalMin)||'?'}</div>
        </div>`;
      }
      const dur = calcSalidaDuration(sal);
      const horaSalida = sal.items[0]?.horaSalida || null;
      const startMin = horaSalida ? minFromTime(horaSalida) : null;
      const endMin = startMin != null ? startMin + dur.totalMin : null;
      const allServido = sal.items.every(i => i.servido);
      const color = allServido ? '#639922' : (sal.type === 'cisterna' ? '#185FA5' : '#27500A');
      const bgColor = allServido ? '#EAF3DE' : (sal.type === 'cisterna' ? '#E6F1FB' : '#EAF3DE');
      const ayteTag = sal._ayudante ? ' 👤ayte' : '';
      const label = `${ORDINALS[sal.salida-1]||sal.salida+'ª'} ${horaSalida||'sin hora'} ${dur.totalMin>0?'('+formatDuration(dur.totalMin)+')':''}${ayteTag}`;
      const breakdownTooltip = dur.stops.map(s =>
        `${s.muni||s.client}: ${s.legKm!=null?s.legKm+'km/'+s.legMin+'min trayecto':'sin coords'} + ${formatDuration(s.descargaMin)} descarga`
      ).join(' → ');
      const fullTitle = `${label}\nDepósito→${dur.stops.map(s=>s.muni||s.client).join('→')}→Depósito\n${breakdownTooltip}\nTotal: ${dur.km}km, conducción: ${formatDuration(dur.driveMin)}, descarga: ${formatDuration(dur.descargaMin)}`;
 
      if(startMin == null) {
        // No start time — show as floating pill
        return `<div style="display:inline-flex;align-items:center;gap:4px;background:${bgColor};color:${color};border:0.5px solid ${color}40;border-radius:4px;padding:2px 7px;font-size:11px;margin-right:4px;" title="${fullTitle}">
          ${ORDINALS[sal.salida-1]||sal.salida+'ª'} sal.${ayteTag} · ${dur.totalMin>0?formatDuration(dur.totalMin):'sin datos'}${dur.km>0?' · '+dur.km+'km':''}
        </div>`;
      }
 
      const left = pct(startMin);
      const width = Math.max(2, pct(dur.totalMin));
      return `<div style="position:absolute;left:${left}%;width:${width}%;top:4px;bottom:4px;background:${bgColor};border:1px ${sal._ayudante?'dashed':'solid'} ${color};border-radius:4px;overflow:hidden;cursor:default;" title="${fullTitle}">
        <div style="padding:2px 5px;font-size:10px;color:${color};white-space:nowrap;overflow:hidden;text-overflow:ellipsis;">${ORDINALS[sal.salida-1]||sal.salida+'ª'} ${horaSalida}→${addMinutes(horaSalida,dur.totalMin)||'?'}${ayteTag}</div>
      </div>`;
    });
 
    const hasTime = dSalidas.some(sal => sal._arrastre ? sal.horaSalida : sal.items[0]?.horaSalida);
 
    return `<div style="display:flex;align-items:center;gap:8px;margin-bottom:10px;">
      <div style="width:80px;font-size:12px;font-weight:500;color:var(--color-text-primary);flex-shrink:0;">${driver.name}</div>
      ${hasTime
        ? `<div style="flex:1;height:36px;position:relative;background:var(--color-background-secondary);border-radius:4px;">${bars.join('')}</div>`
        : `<div style="flex:1;display:flex;flex-wrap:wrap;gap:4px;padding:2px 0;">${bars.join('')}</div>`
      }
    </div>`;
  });
 
  const anyTime = salidas.some(sal => sal.items[0]?.horaSalida) || arrastresDelDia.some(s => s.horaSalida);
 
  document.getElementById('timeline-content').innerHTML = `
    <div style="margin-bottom:8px;display:flex;gap:6px;flex-wrap:wrap;">${tabsHtml}</div>
    <div style="margin-bottom:16px;font-size:12px;color:var(--color-text-tertiary);">
      ${anyTime ? 'Las barras muestran duración estimada según km y tiempo de descarga.' : '⚠ Añade hora de salida a las salidas para ver el timeline completo.'}
    </div>
    ${anyTime ? `<div style="position:relative;margin-bottom:20px;margin-left:88px;height:16px;">${markers.join('')}</div>` : ''}
    <div>${driverRows.join('')}</div>
    <div style="margin-top:12px;padding-top:10px;border-top:0.5px solid var(--color-border-tertiary);">
      <div style="font-size:11px;color:var(--color-text-tertiary);margin-bottom:8px;">
        Velocidad media: ${SPEED_KMH} km/h · Factor carretera: ×${ROAD_FACTOR} · Cisterna: 3h · Palet/GRG: 10 min/ud · GRG trasvasado: 20 min/ud
        · Pasa el ratón sobre las barras para ver el desglose por parada.
      </div>
      ${(salidas.length>0 || arrastresDelDia.length>0) ? `<table style="width:100%;border-collapse:collapse;font-size:11px;">
        <thead><tr style="background:var(--color-background-secondary);">
          <th style="padding:4px 8px;text-align:left;border-bottom:0.5px solid var(--color-border-tertiary);">Conductor</th>
          <th style="padding:4px 8px;text-align:left;border-bottom:0.5px solid var(--color-border-tertiary);">Salida</th>
          <th style="padding:4px 8px;text-align:left;border-bottom:0.5px solid var(--color-border-tertiary);">Clientes</th>
          <th style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">Km</th>
          <th style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">Conducción</th>
          <th style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">Descarga</th>
          <th style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">Total</th>
        </tr></thead>
        <tbody>${salidas.map(sal => {
          const dr = DRIVERS.find(x=>x.id===sal.driver);
          const dur2 = calcSalidaDuration(sal);
          const hora = sal.items[0]?.horaSalida||'—';
          const rowColor = sal.items.every(i=>i.servido) ? 'opacity:0.5;' : '';
          return `<tr style="${rowColor}">
            <td style="padding:4px 8px;border-bottom:0.5px solid var(--color-border-tertiary);">${dr?dr.name:'—'}</td>
            <td style="padding:4px 8px;border-bottom:0.5px solid var(--color-border-tertiary);">${ORDINALS[sal.salida-1]||sal.salida+'ª'} · ${hora}</td>
            <td style="padding:4px 8px;border-bottom:0.5px solid var(--color-border-tertiary);">${sal.items.map(i=>i.muni||i.client||'—').join(', ')}</td>
            <td style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">${dur2.km>0?dur2.km+'km':'—'}</td>
            <td style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">${dur2.driveMin>0?formatDuration(dur2.driveMin):'—'}</td>
            <td style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">${dur2.descargaMin>0?formatDuration(dur2.descargaMin):'—'}</td>
            <td style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);font-weight:500;">${dur2.totalMin>0?formatDuration(dur2.totalMin):'—'}</td>
          </tr>`;
        }).join('')}${arrastresDelDia.map(s => {
          const dr = DRIVERS.find(x=>x.id===s.driver);
          const trayecto = arrastreTrayectoField(s);
          const hora = s.horaSalida||'—';
          return `<tr>
            <td style="padding:4px 8px;border-bottom:0.5px solid var(--color-border-tertiary);">${dr?dr.name:'—'}</td>
            <td style="padding:4px 8px;border-bottom:0.5px solid var(--color-border-tertiary);">⛴ arrastre · ${hora}</td>
            <td style="padding:4px 8px;border-bottom:0.5px solid var(--color-border-tertiary);">${arrastreTrayectoLabel(trayecto)}${s.cisternaCuba?' · Cuba '+escapeHtml(s.cisternaCuba):''}</td>
            <td style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">${arDur.km}km</td>
            <td style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">${formatDuration(arDur.min)}</td>
            <td style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);">—</td>
            <td style="padding:4px 8px;text-align:right;border-bottom:0.5px solid var(--color-border-tertiary);font-weight:500;">${formatDuration(arDur.min)}</td>
          </tr>`;
        }).join('')}</tbody>
      </table>` : ''}
    </div>
  `;
}
 
// ---- TABLA CISTERNAS SEMANA ----
let cisternasWeek=0;
function openCisternasTable(){
  cisternasWeek=currentWeek;
  renderCisternasTable();
  document.getElementById('cisternas-modal').classList.add('open');
}
function closeCisternasTable(){document.getElementById('cisternas-modal').classList.remove('open');}
function setCisternasWeek(n){cisternasWeek=n;renderCisternasTable();}
function renderCisternasTable(){
  const labels=['2 sem. antes','Sem. anterior','Esta semana','Sem. siguiente','2 sem. después'];
  document.getElementById('cisternas-week-tabs').innerHTML=[-2,-1,0,1,2].map(n=>
    `<button class="week-tab${n===cisternasWeek?' active':''}" onclick="setCisternasWeek(${n})">${labels[n+2]}</button>`
  ).join('');
 
  const days=getDaysForWeek(cisternasWeek).filter(d=>!isWeekend(d));
  const dayIndex=servicesByDay();
  const rows=days.map(day=>{
    const key=dateKey(day);
    const cisternas=servicesForDay(key,dayIndex).filter(s=>s.type==='cisterna');
    const label=`${DAYS_ES[day.getDay()]} ${day.getDate()} ${MONTHS_ES[day.getMonth()]}`;
    if(!cisternas.length){
      return `<div style="padding:6px 0;border-bottom:0.5px solid var(--color-border-tertiary);"><strong style="font-size:12px;">${label}</strong><div style="font-size:12px;color:var(--color-text-tertiary);">Sin cisternas</div></div>`;
    }
    const items=cisternas.map(s=>{
      const productos=(s.articles&&s.articles.length)?s.articles.map(a=>getArticleName(a.artCod)).join(', '):(s.merchType==='cisterna'?'Cisterna':'—');
      const cubaTag=s.cisternaCuba?` <span style="opacity:0.65;">[${s.cisternaCuba}]</span>`:' <span style="color:#A32D2D;">[sin cuba]</span>';
      return `<div style="font-size:12px;padding:2px 0;">· ${s.client||'(sin cliente)'}${cubaTag} <span style="opacity:0.65;">— ${productos}</span></div>`;
    }).join('');
    return `<div style="padding:6px 0;border-bottom:0.5px solid var(--color-border-tertiary);"><strong style="font-size:12px;">${label}</strong>${items}</div>`;
  }).join('');
  document.getElementById('cisternas-content').innerHTML=rows;
}
document.getElementById('cisternas-modal').addEventListener('click',function(e){if(e.target===this)closeCisternasTable();});
function printCisternasTable(){
  const content=document.getElementById('cisternas-content').innerHTML;
  const w=window.open('','_blank');
  w.document.write(`<html><head><title>Cisternas semana</title><style>body{font-family:sans-serif;padding:20px;font-size:13px;}strong{display:block;margin-top:10px;}</style></head><body>${content}</body></html>`);
  w.document.close();w.focus();w.print();w.close();
}
 
document.getElementById('timeline-modal').addEventListener('click', function(e) {
  if(e.target === this) closeTimeline();
});
 
// ---- BUSQUEDA DE PEDIDO ----
function weekOffsetForDate(dateStr){
  const target=new Date(dateStr+'T00:00:00');
  const day=target.getDay();
  const diff=day===0?-6:1-day;
  const monday=new Date(target);monday.setDate(target.getDate()+diff);monday.setHours(0,0,0,0);
  const thisMonday=getMonday(0);
  return Math.round((monday-thisMonday)/(7*86400000));
}
function goToWeek(offset){
  currentWeek=offset;selectedTimelineDay=null;
  const tabs=document.querySelectorAll('.week-tab');
  tabs.forEach(t=>t.classList.remove('active'));
  const idx=offset+2;
  if(idx>=0&&idx<tabs.length)tabs[idx].classList.add('active');
  renderCalendar();
}
function searchPedido(){
  const input=document.getElementById('search-pedido');
  const num=input.value.trim();
  if(!num)return;
  const s=services.find(x=>String(x.pedidoNum||'').trim()===num);
  if(!s){alert(`No se encuentra ningún pedido con número "${num}".`);return;}
  goToWeek(weekOffsetForDate(s.day));
  openEdit(s.id);
  input.value='';
}
 
// ---- IMPRESION LISTADO DEL DIA (timeline) ----
 
function printTimeline(){
  const dayKey = currentTimelineDayKey;
  const d = new Date(dayKey + 'T00:00:00');
  const title = `Timeline — ${DAYS_ES[d.getDay()]} ${d.getDate()} ${MONTHS_ES[d.getMonth()]}`;
  const dayServices = servicesForDay(dayKey);
  const salidas = getSalidasForDay(dayKey, dayServices);
  const arrastresDelDia = dayServices.filter(s => s.type === 'arrastre');
 
  const rows = salidas.flatMap(sal => {
    const dr = DRIVER_BY_ID.get(sal.driver);
    const vh = VEHICLE_BY_ID.get(sal.vehicle);
    const salidaLabel = ORDINALS[sal.salida-1] || sal.salida+'ª';
    const hora = sal.items[0]?.horaSalida || '—';
    return sal.items.map(item => {
      const productos = (item.articles||[]).map(a=>getArticleName(a.artCod)+(a.qty?` ×${a.qty}`:'')).join(', ');
      return `<tr>
        <td>${dr?dr.name:'—'}</td>
        <td>${vh?vh.name:'—'}</td>
        <td>${salidaLabel}</td>
        <td>${hora}</td>
        <td>${item.client||'(sin nombre)'}</td>
        <td>${productos||'—'}</td>
      </tr>`;
    });
  }).concat(arrastresDelDia.map(s => {
    const dr = DRIVER_BY_ID.get(s.driver);
    const vh = VEHICLE_BY_ID.get(s.vehicle);
    const trayecto = arrastreTrayectoField(s);
    return `<tr>
      <td>${dr?dr.name:'—'}</td>
      <td>${vh?vh.name:'—'}</td>
      <td>⛴ arrastre</td>
      <td>${s.horaSalida||'—'}</td>
      <td>${arrastreTrayectoLabel(trayecto)}</td>
      <td>${s.cisternaCuba?'Cuba '+s.cisternaCuba:'—'}</td>
    </tr>`;
  })).join('');
 
  const html = `<!doctype html><html><head><meta charset="utf-8"><title>${title}</title>
    <style>
      body{font-family:sans-serif;padding:20px;font-size:12px;color:#000;}
      h2{margin-bottom:10px;}
      table{width:100%;border-collapse:collapse;}
      th,td{padding:5px 8px;border-bottom:1px solid #ccc;text-align:left;}
      th{background:#f0f0f0;}
    </style></head><body>
    <h2>${title}</h2>
    <table><thead><tr><th>Conductor</th><th>Vehículo</th><th>Salida</th><th>Hora salida</th><th>Cliente</th><th>Productos</th></tr></thead>
    <tbody>${rows || '<tr><td colspan="6">Sin servicios</td></tr>'}</tbody></table>
    </body></html>`;
 
  const w = window.open('','_blank');
  w.document.write(html);
  w.document.close();w.focus();w.print();w.close();
}

// ==================================================================================
// ---- PROPUESTA AUTOMATICA DE RUTAS (agrupación por zona + huecos horarios) ----
// ==================================================================================
// Ventana de trabajo 7:00–14:00 (420 min), menos 30 min de merienda = 390 min netos por ruta.
// Se reservan 20 min fijos de carga en base antes de la primera parada (cifra provisional,
// a mejorar más adelante con un cálculo más preciso por tipo/cantidad de mercancía).
const ROUTE_WORK_WINDOW_MIN=7*60;
const ROUTE_MERIENDA_MIN=30;
const ROUTE_BUDGET_MIN=ROUTE_WORK_WINDOW_MIN-ROUTE_MERIENDA_MIN; // 390
const ROUTE_CARGA_MIN=20;
const ROUTE_WINDOW_START='07:00';

// Chófer→vehículo habitual, editable puntualmente desde la propuesta.
const DEFAULT_DRIVER_VEHICLE=[
  {driverId:8,vehicleId:'T4'}, // TUGORES - 5759KRT
  {driverId:1,vehicleId:'T2'}, // SEBASTIÀ - 9606KJW
  {driverId:2,vehicleId:'T3'}, // PEDRO - 5920MJK
  {driverId:3,vehicleId:'T1'}, // CARLOS - 7015HMR
  {driverId:4,vehicleId:'C2'}, // PACO - 2904MJS
  {driverId:5,vehicleId:'C1'}, // PAQUITO - 5818GHD
];
const FURGON_KVZ_ID='F1'; // 1821KVZ — cabe un GRG suelto
const FURGON_FVC_ID='FV'; // 0094FVC — solo jerricanes/sacos, no cabe un GRG
// ROBERTO (7) y PEP (6) quedan excluidos de la propuesta automática de rutas: habitualmente
// están en almacén, no salen a repartir. DEFAULT_DRIVER_VEHICLE ya no los incluye, así que basta
// con no añadirlos aparte (ver findDriverForRoute).
let routeProposalState=null;

function getArticleFormatDigit(cod){
  const code=String(cod||'').trim();
  return /^\d{6,}$/.test(code)?code.slice(-1):null;
}
function isGrgFormatCode(cod){return getArticleFormatDigit(cod)==='2';}
function isJerricanFormatCode(cod){return getArticleFormatDigit(cod)==='3';}
// Estimación de nº de GRG a partir de la cantidad del artículo: si viene en tramos de millar
// (p.ej. 1000, 2000 litros) se cuenta 1 GRG por millar; si viene en unidades pequeñas cada
// unidad ya corresponde a un GRG. Heurística a perfeccionar más adelante.
function estimateGrgUnits(qty){
  qty=Number(qty)||0;
  if(qty<=0)return 0;
  if(qty>=1000)return Math.max(1,Math.round(qty/1000));
  return Math.max(1,Math.round(qty));
}
function routeArticleProfile(routeServices){
  let grgCount=0,hasOther=false,hasAny=false;
  routeServices.forEach(s=>{
    (s.articles||[]).forEach(a=>{
      hasAny=true;
      if(isGrgFormatCode(a.artCod))grgCount+=estimateGrgUnits(a.qty);
      else if(isJerricanFormatCode(a.artCod)){/* jerricane: cabe en FVC */}
      else hasOther=true; // sacos u otro formato no identificado: por precaución no se asume que cabe en furgón
    });
  });
  return {grgCount,hasOther,hasAny};
}
// Vehículo requerido por la carga de una ruta de reparto.
function decideRouteVehicle(routeServices){
  const p=routeArticleProfile(routeServices);
  if(p.grgCount===1&&!p.hasOther)return {vehicleId:FURGON_KVZ_ID,mode:'furgon'};
  if(p.grgCount===0&&p.hasAny&&!p.hasOther)return {vehicleId:FURGON_FVC_ID,mode:'furgon'};
  return {vehicleId:null,mode:'tractora-camion'};
}

function getItemZones(item){
  if(item.zones&&item.zones.length)return item.zones;
  if(item.muni&&MUNI_ZONE[item.muni])return MUNI_ZONE[item.muni];
  if(item.muni){
    const key=normalizeStr(item.muni);
    if(POB_ZONE[key])return POB_ZONE[key];
    const base=key.split('-')[0].trim();
    const found=POB_ZONE_KEYS.find(k=>key.startsWith(k)||k.startsWith(base));
    if(found)return POB_ZONE[found];
  }
  return [];
}
function itemDistanceKm(a,b){
  const ca=getStopCoords(a),cb=getStopCoords(b);
  if(!ca||!cb)return 9999;
  return haversineKm(ca,cb);
}
function buildRouteSalida(routeServices,origenTrayecto,destinoTrayecto){
  return {items:routeServices.map(s=>({muni:s.muni,merchType:s.merchType,merchQty:s.merchQty,merchManual:s.merchManual,client:s.client})),origenTrayecto:origenTrayecto||'base',destinoTrayecto:destinoTrayecto||'base'};
}
function routeTotalMin(routeServices,origenTrayecto,destinoTrayecto){
  return ROUTE_CARGA_MIN+calcSalidaDuration(buildRouteSalida(routeServices,origenTrayecto,destinoTrayecto)).totalMin;
}

// Tiempo ya comprometido ese día por cada conductor (carga+ruta / arrastre), para no solaparle
// una ruta nueva propuesta con lo que ya tiene asignado.
function initDriverUsageMap(dayKey){
  const map={};
  const dayServices=servicesForDay(dayKey);
  getSalidasForDay(dayKey,dayServices).forEach(sal=>{
    const dur=ROUTE_CARGA_MIN+calcSalidaDuration(sal).totalMin;
    map[sal.driver]=(map[sal.driver]||0)+dur;
    if(sal.twoPerson&&sal.driver2)map[sal.driver2]=(map[sal.driver2]||0)+dur;
  });
  const arDur=calcArrastreDuration().min;
  dayServices.filter(s=>s.type==='arrastre').forEach(s=>{map[s.driver]=(map[s.driver]||0)+arDur;});
  return map;
}
// Busca chófer disponible para una ruta ya dimensionada (routeMin, incluida la carga), entre los
// conductores de reparto por defecto (DEFAULT_DRIVER_VEHICLE) — Roberto y Pep quedan fuera de la
// propuesta porque habitualmente están en almacén. Empieza por el que menos hueco lleve ocupado
// ese día; el vehículo de furgón (si aplica) ya viene fijado en vehicleDecision.vehicleId.
function findDriverForRoute(routeMin,vehicleDecision,usageMap,requireTractoraCapable){
  const candidateIds=DEFAULT_DRIVER_VEHICLE.map(d=>d.driverId).slice().sort((a,b)=>(usageMap[a]||0)-(usageMap[b]||0));
  for(const dId of candidateIds){
    const dr=DRIVER_BY_ID.get(dId);
    if(requireTractoraCapable&&!dr?.canTractora)continue;
    const used=usageMap[dId]||0;
    if(used+routeMin<=ROUTE_BUDGET_MIN){
      const vehicleId=vehicleDecision.vehicleId||(DEFAULT_DRIVER_VEHICLE.find(d=>d.driverId===dId)?.vehicleId)||null;
      const horaSalida=addMinutes(ROUTE_WINDOW_START,used+ROUTE_CARGA_MIN);
      usageMap[dId]=used+routeMin;
      return {driverId:dId,vehicleId,horaSalida};
    }
  }
  return null;
}

// ---- VALIDACION DE SOLAPES EN LA PROPUESTA (antes de confirmar) ----
// Intervalos ocupados por OTRAS entradas de la propia propuesta para un chófer, para detectar
// que dos rutas propuestas al mismo conductor no se solapen entre sí (además de con lo ya confirmado).
function routeProposalIntervalsForDriver(st,driverId,excludeKind,excludeIdx){
  const list=[];
  st.cisternaProposals.forEach((p,idx)=>{
    if(excludeKind==='cisterna'&&excludeIdx===idx)return;
    if(p.driverId!==driverId||!p.horaSalida)return;
    list.push({start:p.horaSalida,end:addMinutes(p.horaSalida,180)});
  });
  st.newRoutes.forEach((r,idx)=>{
    if(excludeKind==='new'&&excludeIdx===idx)return;
    if(r.driverId!==driverId||!r.horaSalida)return;
    const routeServices=r.serviceIds.map(id=>services.find(x=>x.id===id)).filter(Boolean);
    const min=ROUTE_CARGA_MIN+calcSalidaDuration(buildRouteSalida(routeServices)).totalMin;
    list.push({start:r.horaSalida,end:addMinutes(r.horaSalida,min)});
  });
  return list;
}
// Devuelve una descripción del solape si lo hay, o null si el hueco está libre. Comprueba contra
// lo ya confirmado ese día (getPersonBusyIntervals) y contra el resto de la propia propuesta.
function checkRouteProposalConflict(st,driverId,horaSalida,durationMin,excludeKind,excludeIdx){
  if(!driverId||!horaSalida||!durationMin)return null;
  const end=addMinutes(horaSalida,durationMin);
  const existing=getPersonBusyIntervals(st.dayKey,driverId,null,null);
  const confExisting=existing.find(iv=>intervalsOverlap(horaSalida,end,iv.start,iv.end));
  if(confExisting)return `solapa con salida ya confirmada (${confExisting.start}–${confExisting.end})`;
  const confProposal=routeProposalIntervalsForDriver(st,driverId,excludeKind,excludeIdx)
    .find(iv=>intervalsOverlap(horaSalida,end,iv.start,iv.end));
  if(confProposal)return `solapa con otra ruta de esta propuesta (${confProposal.start}–${confProposal.end})`;
  return null;
}
// Recorre toda la propuesta y devuelve la lista de conflictos detectados, para el aviso al confirmar.
function collectRouteProposalConflicts(st){
  const out=[];
  st.cisternaProposals.forEach((p,idx)=>{
    if(!p.driverId)return;
    const c=checkRouteProposalConflict(st,p.driverId,p.horaSalida,180,'cisterna',idx);
    if(c){const s=services.find(x=>x.id===p.serviceId);out.push(`Cisterna ${s?.pedidoNum?'#'+s.pedidoNum:(s?.client||'')} (${DRIVER_BY_ID.get(p.driverId)?.name}): ${c}`);}
  });
  st.newRoutes.forEach((r,idx)=>{
    if(!r.driverId)return;
    const routeServices=r.serviceIds.map(id=>services.find(x=>x.id===id)).filter(Boolean);
    const min=ROUTE_CARGA_MIN+calcSalidaDuration(buildRouteSalida(routeServices)).totalMin;
    const c=checkRouteProposalConflict(st,r.driverId,r.horaSalida,min,'new',idx);
    if(c)out.push(`Ruta zona ${r.zone} (${DRIVER_BY_ID.get(r.driverId)?.name}): ${c}`);
  });
  return out;
}

// Agrupa pedidos de una zona en rutas nuevas respetando el presupuesto horario, con un
// vecino-más-cercano voraz para minimizar el trayecto dentro de cada ruta.
function clusterZoneItems(items){
  const remaining=items.slice();
  const routes=[];
  while(remaining.length){
    const route=[remaining.shift()];
    let progress=true;
    while(progress&&remaining.length){
      progress=false;
      let bestIdx=-1,bestDist=Infinity;
      remaining.forEach((cand,idx)=>{
        const testRoute=route.concat([cand]);
        if(routeTotalMin(testRoute)<=ROUTE_BUDGET_MIN){
          const dist=itemDistanceKm(route[route.length-1],cand);
          if(dist<bestDist){bestDist=dist;bestIdx=idx;}
        }
      });
      if(bestIdx>=0){route.push(remaining.splice(bestIdx,1)[0]);progress=true;}
    }
    routes.push(route);
  }
  return routes;
}

// Pedidos importados y sin conductor de días ANTERIORES a hoy (hasta 5 días atrás), que siguen
// pendientes. Se incorporan a la propuesta del día que se esté viendo —siempre que ese día sea
// hoy o posterior, nunca tiene sentido "proponer rutas" para un día ya pasado— porque lo lógico
// es sacarlos con las rutas de hoy en cuanto se detectan, no esperar a que alguien navegue
// manualmente hasta el viernes pasado.
function getOverdueUnassigned(proposalDayKey){
  const todayKey=localDateKey(new Date());
  if(proposalDayKey<todayKey)return []; // no se proponen rutas para un día ya pasado
  const lookback=new Date();lookback.setDate(lookback.getDate()-5);
  const lookbackKey=localDateKey(lookback);
  return services.filter(s=>s.imported&&!s.driver&&s.day&&s.day>=lookbackKey&&s.day<todayKey);
}

// ---- CALCULO PRINCIPAL DE LA PROPUESTA PARA UN DIA ----
function computeRouteProposals(dayKey){
  const usageMap=initDriverUsageMap(dayKey);
  const overdue=getOverdueUnassigned(dayKey);
  const overdueDaySet=new Set(overdue.map(s=>s.day));

  // 1) Cisternas sin conductor: las de hoy + las atrasadas de días anteriores sin asignar.
  const cisternaServices=services.filter(s=>s.day===dayKey&&s.type==='cisterna'&&!s.driver)
    .concat(overdue.filter(s=>s.type==='cisterna'));
  const cisternaProposals=cisternaServices.map(s=>{
    const assign=findDriverForRoute(ROUTE_CARGA_MIN+180,{mode:'tractora-camion'},usageMap,true);
    return {serviceId:s.id,driverId:assign?assign.driverId:null,vehicleId:assign?assign.vehicleId:null,horaSalida:assign?assign.horaSalida:null,isOverdue:s.day!==dayKey,originalDay:s.day};
  });

  // 2) Repartos sin conductor: los de hoy + los atrasados, unidos en la misma bolsa por zona.
  const existingSalidas=getSalidasForDay(dayKey,servicesForDay(dayKey)).filter(sal=>sal.type==='reparto');
  const unassigned=services.filter(s=>s.day===dayKey&&s.type==='reparto'&&!s.driver)
    .concat(overdue.filter(s=>s.type==='reparto'));
  const noZoneItems=[];
  const byZonePool={};
  unassigned.forEach(s=>{
    const zones=getItemZones(s);
    if(!zones.length){noZoneItems.push(s);return;}
    (byZonePool[zones[0]]=byZonePool[zones[0]]||[]).push(s);
  });

  const existingRouteAdditions=[];
  existingSalidas.forEach(sal=>{
    const zone=(sal.zones&&sal.zones[0])||getItemZones(sal.items[0])[0];
    if(!zone)return;
    const pool=byZonePool[zone];
    if(!pool||!pool.length)return;
    let currentItems=sal.items.slice();
    const added=[];
    let progress=true;
    while(progress&&pool.length){
      progress=false;
      let bestIdx=-1,bestTotal=Infinity;
      pool.forEach((cand,idx)=>{
        const testItems=currentItems.concat([cand]);
        const total=ROUTE_CARGA_MIN+calcSalidaDuration(buildRouteSalida(testItems,sal.origenTrayecto,sal.destinoTrayecto)).totalMin;
        if(total<=ROUTE_BUDGET_MIN&&total<bestTotal){bestTotal=total;bestIdx=idx;}
      });
      if(bestIdx>=0){
        const item=pool.splice(bestIdx,1)[0];
        currentItems.push(item);added.push(item);progress=true;
      }
    }
    if(added.length){
      const extraMin=(ROUTE_CARGA_MIN+calcSalidaDuration(buildRouteSalida(currentItems,sal.origenTrayecto,sal.destinoTrayecto)).totalMin)
        -(ROUTE_CARGA_MIN+calcSalidaDuration(sal).totalMin);
      usageMap[sal.driver]=(usageMap[sal.driver]||0)+extraMin;
      if(sal.twoPerson&&sal.driver2)usageMap[sal.driver2]=(usageMap[sal.driver2]||0)+extraMin;
      existingRouteAdditions.push({driver:sal.driver,salida:sal.salida,zone,vehicle:sal.vehicle,horaSalida:sal.items[0]?.horaSalida||null,addedServiceIds:added.map(a=>a.id)});
    }
  });

  const newRoutes=[];
  const overflowItems=[];
  Object.keys(byZonePool).forEach(zone=>{
    const items=byZonePool[zone];
    if(!items.length)return;
    clusterZoneItems(items).forEach(cluster=>{
      const vehicleDecision=decideRouteVehicle(cluster);
      const routeMin=routeTotalMin(cluster);
      const assign=findDriverForRoute(routeMin,vehicleDecision,usageMap,false);
      if(assign){
        newRoutes.push({zone,serviceIds:cluster.map(c=>c.id),driverId:assign.driverId,vehicleId:assign.vehicleId,horaSalida:assign.horaSalida});
      }else{
        overflowItems.push(...cluster);
      }
    });
  });

  return {dayKey,cisternaProposals,existingRouteAdditions,newRoutes,noZoneItems,overflowItems,overdueDaySet,hasOverdue:overdue.length>0};
}

function applyRouteProposals(state){
  if(!state)return 0;
  let count=0;
  state.cisternaProposals.forEach(p=>{
    if(!p.driverId)return;
    const s=services.find(x=>x.id===p.serviceId);
    if(!s)return;
    if(s.day!==state.dayKey)s.day=state.dayKey; // atrasado: se reprograma al día de la propuesta
    s.driver=p.driverId;s.vehicle=p.vehicleId;
    s.salida=nextSalidaNumberForDriver(state.dayKey,p.driverId,null);
    if(p.horaSalida)s.horaSalida=p.horaSalida;
    count++;
  });
  state.existingRouteAdditions.forEach(r=>{
    let order=1000;
    r.addedServiceIds.forEach(id=>{
      const s=services.find(x=>x.id===id);
      if(!s)return;
      if(s.day!==state.dayKey)s.day=state.dayKey;
      s.driver=r.driver;s.vehicle=r.vehicle;s.salida=r.salida;s.itemOrder=order++;
      if(r.horaSalida)s.horaSalida=r.horaSalida;
      count++;
    });
  });
  state.newRoutes.forEach(r=>{
    if(!r.driverId||!r.serviceIds.length)return;
    const salida=nextSalidaNumberForDriver(state.dayKey,r.driverId,null);
    let order=0;
    r.serviceIds.forEach(id=>{
      const s=services.find(x=>x.id===id);
      if(!s)return;
      if(s.day!==state.dayKey)s.day=state.dayKey;
      s.driver=r.driverId;s.vehicle=r.vehicleId;s.salida=salida;s.horaSalida=r.horaSalida;s.itemOrder=order++;
      count++;
    });
  });
  return count;
}
// ---- UI ----
function buildDriverOptionsHtml(selectedId,requireTractora){
  return DRIVERS.filter(d=>!requireTractora||d.canTractora)
    .map(d=>`<option value="${d.id}" ${Number(selectedId)===d.id?'selected':''}>${d.name}</option>`).join('');
}
function buildVehicleOptionsHtml(selectedId,driverId){
  const dr=DRIVER_BY_ID.get(parseInt(driverId));
  return VEHICLES.filter(v=>{
    if(v.type==='tractora')return dr?.canTractora;
    if(v.type==='camion'||v.type==='furgon')return dr?.canCamion||dr?.canFurgon;
    if(v.type==='furgoneta')return dr?.canFurgon;
    return true;
  }).map(v=>`<option value="${v.id}" ${selectedId===v.id?'selected':''}>${v.name}</option>`).join('');
}
function openProposeRoutes(){
  const days=getDaysForWeek(currentWeek).filter(d=>!isWeekend(d));
  const todayKey=dateKey(new Date());
  const defaultDay=days.find(d=>dateKey(d)===todayKey)||days[0];
  renderProposeRoutes(defaultDay?dateKey(defaultDay):dateKey(days[0]));
  document.getElementById('propose-routes-modal').classList.add('open');
}
function closeProposeRoutes(){
  document.getElementById('propose-routes-modal').classList.remove('open');
  routeProposalState=null;
}
function renderProposeRoutes(dayKey){
  routeProposalState=computeRouteProposals(dayKey);
  renderProposeRoutesContent();
}
function renderProposeRoutesContent(){
  const st=routeProposalState;
  if(!st)return;
  const days=getDaysForWeek(currentWeek).filter(d=>!isWeekend(d));
  const tabsHtml=days.map(d=>{
    const k=dateKey(d);
    const active=k===st.dayKey;
    return `<button onclick="renderProposeRoutes('${k}')" style="padding:4px 10px;font-size:12px;cursor:pointer;border-radius:var(--border-radius-md);border:0.5px solid ${active?'#185FA5':'var(--color-border-tertiary)'};background:${active?'#185FA5':'var(--color-background-secondary)'};color:${active?'#fff':'var(--color-text-secondary)'};">${DAYS_ES[d.getDay()]} ${d.getDate()}</button>`;
  }).join('');
  const d=new Date(st.dayKey+'T00:00:00');
  const dayLabel=`${DAYS_ES[d.getDay()]} ${d.getDate()} ${MONTHS_ES[d.getMonth()]}`;
  const sections=[`<div style="margin-bottom:8px;display:flex;gap:6px;flex-wrap:wrap;">${tabsHtml}</div>`,
    `<h3 style="font-size:14px;margin-bottom:0.5rem;">Propuesta de rutas — ${dayLabel}</h3>`];
  if(st.hasOverdue){
         sections.push(`<div style="font-size:11px;background:#FAEEDA;color:#633806;border:0.5px solid #E5B96F;border-radius:var(--border-radius-md);padding:6px 9px;margin-bottom:0.5rem;">⚠ Esta propuesta incluye pedidos atrasados de días anteriores (hasta 5 días atrás). Al confirmar, se reprogramarán a la fecha de hoy/${st.dayKey}.</div>`);
   }

  if(st.cisternaProposals.length){
    sections.push(`<h4 style="font-size:12px;margin:0.75rem 0 0.4rem;color:var(--color-text-secondary);">Cisternas sin conductor (${st.cisternaProposals.length})</h4>`);
    sections.push(st.cisternaProposals.map((p,idx)=>{
      const s=services.find(x=>x.id===p.serviceId);
      if(!s)return '';
      const cConflict=p.driverId?checkRouteProposalConflict(st,p.driverId,p.horaSalida,180,'cisterna',idx):null;
      return `<div style="display:flex;gap:6px;align-items:center;padding:5px 8px;border:0.5px solid var(--color-border-tertiary);border-radius:var(--border-radius-md);margin-bottom:4px;font-size:12px;">
        <span style="flex:1;">${s.pedidoNum?'#'+escapeHtml(s.pedidoNum)+' — ':''}${escapeHtml(s.client||'(sin cliente)')} <span style="opacity:0.6;">${escapeHtml(s.muni||'')}</span></span>
        <select onchange="updateCisternaProposalDriver(${idx},this.value)" style="font-size:11px;">
          <option value="">— sin proponer —</option>${buildDriverOptionsHtml(p.driverId,true)}
        </select>
        <span style="font-size:10px;${cConflict?'color:#A32D2D;':'opacity:0.7;'}">${cConflict?'⚠ '+cConflict:(p.horaSalida||(p.driverId?'':'⚠ sin hueco disponible'))}</span>
      </div>`;
    }).join(''));
  }
  if(st.existingRouteAdditions.length){
    sections.push(`<h4 style="font-size:12px;margin:0.75rem 0 0.4rem;color:var(--color-text-secondary);">Rutas existentes ampliadas (${st.existingRouteAdditions.length})</h4>`);
    sections.push(st.existingRouteAdditions.map((r,idx)=>{
      const dr=DRIVER_BY_ID.get(r.driver);
      const stopsHtml=r.addedServiceIds.map(id=>{
        const s=services.find(x=>x.id===id);
        if(!s)return '';
        const overdueTag=st.overdueDaySet.has(s.day)?`<span style="font-size:9px;background:#FAEEDA;color:#633806;border-radius:3px;padding:0 3px;margin-right:3px;">⚠ ${s.day}</span>`:'';
        return `<div style="font-size:11px;padding:2px 0;display:flex;justify-content:space-between;">
          <span>+ ${overdueTag}${s.pedidoNum?'#'+escapeHtml(s.pedidoNum)+' — ':''}${escapeHtml(s.client||'—')} <span style="opacity:0.6;">${escapeHtml(s.muni||'')}</span></span>
          <button onclick="removeStopFromExistingRoute(${idx},${JSON.stringify(id)})" style="font-size:10px;padding:0 4px;cursor:pointer;border:none;background:rgba(0,0,0,0.08);border-radius:3px;">✕</button>
        </div>`;
      }).join('');
      return `<div style="padding:6px 8px;border:0.5px solid #A9E0CB;background:#F1FAF6;border-radius:var(--border-radius-md);margin-bottom:6px;">
        <div style="display:flex;gap:6px;align-items:center;flex-wrap:wrap;margin-bottom:4px;">
          <strong style="font-size:12px;">Zona ${escapeHtml(r.zone)} — ${dr?dr.name:'—'} (${ORDINALS[r.salida-1]||r.salida+'ª'} sal.${r.horaSalida?' · '+r.horaSalida:''})</strong>
        </div>
        ${stopsHtml}
      </div>`;
    }).join(''));
  }
  if(st.newRoutes.length){
    sections.push(`<h4 style="font-size:12px;margin:0.75rem 0 0.4rem;color:var(--color-text-secondary);">Rutas nuevas propuestas (${st.newRoutes.length})</h4>`);
        sections.push(st.newRoutes.map((r,idx)=>{
      const routeServices=r.serviceIds.map(id=>services.find(x=>x.id===id)).filter(Boolean);
      const durObj=calcSalidaDuration(buildRouteSalida(routeServices));
      const totalMin=ROUTE_CARGA_MIN+durObj.totalMin;
      const stopsHtml=routeServices.map(s=>{
        const overdueTag=st.overdueDaySet.has(s.day)?`<span style="font-size:9px;background:#FAEEDA;color:#633806;border-radius:3px;padding:0 3px;margin-right:3px;">⚠ ${s.day}</span>`:'';
        return `<div style="font-size:11px;padding:2px 0;display:flex;justify-content:space-between;">
          <span>· ${overdueTag}${s.pedidoNum?'#'+escapeHtml(s.pedidoNum)+' — ':''}${escapeHtml(s.client||'—')} <span style="opacity:0.6;">${escapeHtml(s.muni||'')}</span></span>
          <button onclick="removeStopFromNewRoute(${idx},${JSON.stringify(s.id)})" style="font-size:10px;padding:0 4px;cursor:pointer;border:none;background:rgba(0,0,0,0.08);border-radius:3px;">✕</button>
        </div>`;
      }).join('');
        const rConflict=r.driverId?checkRouteProposalConflict(st,r.driverId,r.horaSalida,totalMin,'new',idx):null;
      return `<div style="padding:6px 8px;border:0.5px solid #B5D4F4;background:#EEF6FD;border-radius:var(--border-radius-md);margin-bottom:6px;">
        <div style="display:flex;gap:6px;align-items:center;flex-wrap:wrap;margin-bottom:4px;">
          <strong style="font-size:12px;">Zona ${escapeHtml(r.zone)}</strong>
          <select onchange="updateRouteProposalDriver(${idx},this.value)" style="font-size:11px;">${buildDriverOptionsHtml(r.driverId,false)}</select>
          <select onchange="updateRouteProposalVehicle(${idx},this.value)" style="font-size:11px;">${buildVehicleOptionsHtml(r.vehicleId,r.driverId)}</select>
          <input type="time" value="${r.horaSalida||''}" onchange="updateRouteProposalHora(${idx},this.value)" style="font-size:11px;width:80px;">
          <span style="font-size:10px;${rConflict?'color:#A32D2D;':'opacity:0.65;'}">${rConflict?'⚠ '+rConflict+' · ':''}${formatDuration(totalMin)} total${durObj.missingCoords?' · ⚠ falta ubicar alguna población':''}</span>
        </div>
        ${stopsHtml}
      </div>`;
    }).join(''));
  }

  if(st.noZoneItems.length){
    sections.push(`<h4 style="font-size:12px;margin:0.75rem 0 0.4rem;color:#791F1F;">Sin zona detectada — asignación manual (${st.noZoneItems.length})</h4>`);
    sections.push(st.noZoneItems.map(s=>`<div style="font-size:11px;padding:3px 6px;background:#FBE4D5;border-radius:4px;margin-bottom:3px;cursor:pointer;" onclick="closeProposeRoutes();openEdit(${JSON.stringify(s.id)});">${s.pedidoNum?'#'+escapeHtml(s.pedidoNum)+' — ':''}${escapeHtml(s.client||'—')} <span style="opacity:0.6;">${escapeHtml(s.muni||'sin municipio')}</span></div>`).join(''));
  }

  if(st.overflowItems.length){
    sections.push(`<h4 style="font-size:12px;margin:0.75rem 0 0.4rem;color:#791F1F;">Sin hueco disponible hoy (${st.overflowItems.length})</h4>`);
    sections.push(st.overflowItems.map(s=>`<div style="font-size:11px;padding:3px 6px;background:#FBE4D5;border-radius:4px;margin-bottom:3px;display:flex;justify-content:space-between;align-items:center;">
      <span>${s.pedidoNum?'#'+escapeHtml(s.pedidoNum)+' — ':''}${escapeHtml(s.client||'—')} <span style="opacity:0.6;">${escapeHtml(s.muni||'')}</span></span>
      <button onclick="moveOverflowToNextDay(${JSON.stringify(s.id)})" class="btn-secondary" style="font-size:10px;padding:2px 6px;">→ día siguiente</button>
    </div>`).join(''));
  }

  if(!st.cisternaProposals.length&&!st.existingRouteAdditions.length&&!st.newRoutes.length&&!st.noZoneItems.length&&!st.overflowItems.length){
    sections.push('<div style="font-size:12px;color:var(--color-text-tertiary);padding:1rem 0;">No hay pedidos sin asignar este día.</div>');
  }
  document.getElementById('propose-routes-content').innerHTML=sections.join('');
}
function updateCisternaProposalDriver(idx,value){
  const st=routeProposalState;if(!st)return;
  const p=st.cisternaProposals[idx];
  if(!value){p.driverId=null;p.vehicleId=null;p.horaSalida=null;renderProposeRoutesContent();return;}
  const driverId=parseInt(value);
  const def=DEFAULT_DRIVER_VEHICLE.find(d=>d.driverId===driverId);
  p.driverId=driverId;
  p.vehicleId=def?def.vehicleId:(VEHICLES.find(v=>v.type==='tractora')?.id||null);
  p.horaSalida=p.horaSalida||ROUTE_WINDOW_START;
  renderProposeRoutesContent();
}
function updateRouteProposalDriver(idx,value){
  const st=routeProposalState;if(!st)return;
  const r=st.newRoutes[idx];
  r.driverId=parseInt(value)||null;
  const def=DEFAULT_DRIVER_VEHICLE.find(d=>d.driverId===r.driverId);
  if(def)r.vehicleId=def.vehicleId;
  renderProposeRoutesContent();
}
function updateRouteProposalVehicle(idx,value){
  const st=routeProposalState;if(!st)return;
  st.newRoutes[idx].vehicleId=value||null;
  renderProposeRoutesContent();
}
function updateRouteProposalHora(idx,value){
  const st=routeProposalState;if(!st)return;
  st.newRoutes[idx].horaSalida=value||null;
  renderProposeRoutesContent();
}
function removeStopFromNewRoute(routeIdx,serviceId){
  const st=routeProposalState;if(!st)return;
  const r=st.newRoutes[routeIdx];
  r.serviceIds=r.serviceIds.filter(id=>id!==serviceId);
  if(!r.serviceIds.length)st.newRoutes.splice(routeIdx,1);
  renderProposeRoutesContent();
}
function removeStopFromExistingRoute(routeIdx,serviceId){
  const st=routeProposalState;if(!st)return;
  const r=st.existingRouteAdditions[routeIdx];
  r.addedServiceIds=r.addedServiceIds.filter(id=>id!==serviceId);
  if(!r.addedServiceIds.length)st.existingRouteAdditions.splice(routeIdx,1);
  renderProposeRoutesContent();
}
function moveOverflowToNextDay(serviceId){
  const s=services.find(x=>x.id===serviceId);
  if(!s)return;
  let d=new Date(s.day+'T00:00:00');
  do{d.setDate(d.getDate()+1);}while(isWeekend(d));
  s.day=localDateKey(d);
  saveData('reordenacion',{summary:'Pedido movido al día siguiente por falta de hueco en la propuesta de rutas',day:s.day,pedidoNum:s.pedidoNum,type:s.type});
  renderProposeRoutes(routeProposalState.dayKey);
  renderCalendar();
}
function confirmProposedRoutesUI(){
  const st=routeProposalState;
  if(!st){closeProposeRoutes();return;}
  const total=st.cisternaProposals.filter(p=>p.driverId).length
    +st.existingRouteAdditions.reduce((t,r)=>t+r.addedServiceIds.length,0)
    +st.newRoutes.reduce((t,r)=>t+(r.driverId?r.serviceIds.length:0),0);
  if(!total){alert('No hay ninguna asignación propuesta que confirmar.');return;}
  const conflicts=collectRouteProposalConflicts(st);
  if(conflicts.length){
    const proceed=confirm(`Se han detectado ${conflicts.length} solape(s) horario(s) en la propuesta:\n\n${conflicts.join('\n')}\n\n¿Confirmar de todas formas? (revisa esas rutas después)`);
    if(!proceed)return;
  }else{
    if(!confirm(`Se van a asignar ${total} pedido(s)/servicio(s) según la propuesta. ¿Confirmar?`))return;
  }
  applyRouteProposals(st);
  saveData('alta',{summary:`Propuesta de rutas aplicada (${total} pedidos)${conflicts.length?' con '+conflicts.length+' solape(s) aceptado(s)':''}`,day:st.dayKey,affectedCount:total});
  closeProposeRoutes();
  renderCalendar();
}
document.getElementById('f-client').addEventListener('input',updatePinStatus);
