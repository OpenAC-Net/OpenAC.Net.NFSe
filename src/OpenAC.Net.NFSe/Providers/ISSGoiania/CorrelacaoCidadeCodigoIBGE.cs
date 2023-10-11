using System;
using System.Collections.Generic;

namespace OpenAC.Net.NFSe.Providers;

internal static class CorrelacaoCidadeGoianiaXCodigoIBGE
{
    public static string GetCodigoCidadeFromCodigoIBGE(string codigoIBGE)
    {
        if (!_codigoCidadeGoiania.ContainsKey(codigoIBGE))
            throw new NotImplementedException("Código IBGE não encontrado na correlação para Goiânia");

        var ret = _codigoCidadeGoiania[codigoIBGE];

        if (string.IsNullOrEmpty(ret))
            throw new NotImplementedException("Código de Cidade no banco de dados de Goiânia não encontrado na correlação para o IBGE");

        return ret;
    }

    #region Cidades Goiania X Código IBGE

    private static Dictionary<string, string> _codigoCidadeGoiania = new()
    {
            { "1200013", "107200" }, // ACRELÂNDIA/AC
            { "1200054", "078600" }, // ASSIS BRASIL/AC
            { "1200104", "078700" }, // BRASILÉIA/AC
            { "1200138", "107300" }, // BUJARI/AC
            { "1200179", "107400" }, // CAPIXABA/AC
            { "1200203", "077600" }, // CRUZEIRO DO SUL/AC
            { "1200252", "107500" }, // EPITACIOLÂNDIA/AC
            { "1200302", "077700" }, // FEIJÓ/AC
            { "1200328", "107600" }, // JORDÃO/AC
            { "1200336", "077800" }, // MÂNCIO LIMA/AC
            { "1200344", "078800" }, // MANOEL URBANO/AC
            { "1200351", "107700" }, // MARECHAL THAUMATURGO/AC
            { "1200385", "078900" }, // PLÁCIDO DE CASTRO/AC
            { "1200807", "067700" }, // PORTO ACRE/AC
            { "1200393", "107800" }, // PORTO WALTER/AC
            { "1200401", "079000" }, // RIO BRANCO/AC
            { "1200427", "107900" }, // RODRIGUES ALVES/AC
            { "1200435", "108000" }, // SANTA ROSA DO PURUS/AC
            { "1200500", "079200" }, // SENA MADUREIRA/AC
            { "1200450", "079100" }, // SENADOR GUIOMARD/AC
            { "1200609", "077900" }, // TARAUACÁ/AC
            { "1200708", "079300" }, // XAPURI/AC
            { "2700102", "308100" }, // ÁGUA BRANCA/AL
            { "2700201", "313000" }, // ANADIA/AL
            { "2700300", "313100" }, // ARAPIRACA/AL
            { "2700409", "315600" }, // ATALAIA/AL
            { "2700508", "318300" }, // BARRA DE SANTO ANTÔNIO/AL
            { "2700607", "319700" }, // BARRA DE SÃO MIGUEL/AL
            { "2700706", "309200" }, // BATALHA/AL
            { "2700805", "311500" }, // BELÉM/AL
            { "2700904", "309300" }, // BELO MONTE/AL
            { "2701001", "319800" }, // BOCA DA MATA/AL
            { "2701100", "315700" }, // BRANQUINHA/AL
            { "2701209", "311600" }, // CACIMBINHAS/AL
            { "2701308", "315800" }, // CAJUEIRO/AL
            { "2701357", "190900" }, // CAMPESTRE/AL
            { "2701407", "319900" }, // CAMPO ALEGRE/AL
            { "2701506", "313200" }, // CAMPO GRANDE/AL
            { "2701605", "308200" }, // CANAPI/AL
            { "2701704", "315900" }, // CAPELA/AL
            { "2701803", "309400" }, // CARNEIROS/AL
            { "2701902", "316000" }, // CHÃ PRETA/AL
            { "2702009", "313300" }, // COITÉ DO NÓIA/AL
            { "2702108", "316100" }, // COLÔNIA LEOPOLDINA/AL
            { "2702207", "320700" }, // COQUEIRO SECO/AL
            { "2702306", "320000" }, // CORURIPE/AL
            { "2702355", "313400" }, // CRAÍBAS/AL
            { "2702405", "308300" }, // DELMIRO GOUVEIA/AL
            { "2702504", "309500" }, // DOIS RIACHOS/AL
            { "2702553", "115500" }, // ESTRELA DE ALAGOAS/AL
            { "2702603", "313500" }, // FEIRA GRANDE/AL
            { "2702702", "314800" }, // FELIZ DESERTO/AL
            { "2702801", "316200" }, // FLEXEIRAS/AL
            { "2702900", "313600" }, // GIRAU DO PONCIANO/AL
            { "2703007", "316300" }, // IBATEGUARA/AL
            { "2703106", "311700" }, // IGACI/AL
            { "2703205", "314900" }, // IGREJA NOVA/AL
            { "2703304", "308400" }, // INHAPI/AL
            { "2703403", "309600" }, // JACARÉ DOS HOMENS/AL
            { "2703502", "316400" }, // JACUÍPE/AL
            { "2703601", "318400" }, // JAPARATINGA/AL
            { "2703700", "309700" }, // JARAMATAIA/AL
            { "2703759", "228600" }, // JEQUIÁ DA PRAIA/AL
            { "2703809", "316500" }, // JOAQUIM GOMES/AL
            { "2703908", "316600" }, // JUNDIÁ/AL
            { "2704005", "313700" }, // JUNQUEIRO/AL
            { "2704104", "313800" }, // LAGOA DA CANOA/AL
            { "2704203", "313900" }, // LIMOEIRO DE ANADIA/AL
            { "2704302", "320800" }, // MACEIÓ/AL
            { "2704401", "309800" }, // MAJOR ISIDORO/AL
            { "2704906", "311900" }, // MAR VERMELHO/AL
            { "2704500", "318500" }, // MARAGOGI/AL
            { "2704609", "309900" }, // MARAVILHA/AL
            { "2704708", "320900" }, // MARECHAL DEODORO/AL
            { "2704807", "311800" }, // MARIBONDO/AL
            { "2705002", "308500" }, // MATA GRANDE/AL
            { "2705101", "318600" }, // MATRIZ DE CAMARAGIBE/AL
            { "2705200", "316700" }, // MESSIAS/AL
            { "2705309", "312000" }, // MINADOR DO NEGRÃO/AL
            { "2705408", "310000" }, // MONTEIRÓPOLIS/AL
            { "2705507", "316800" }, // MURICI/AL
            { "2705606", "316900" }, // NOVO LINO/AL
            { "2705705", "310100" }, // OLHO D'ÁGUA DAS FLORES/AL
            { "2705804", "308600" }, // OLHO D'ÁGUA DO CASADO/AL
            { "2705903", "314000" }, // OLHO D'ÁGUA GRANDE/AL
            { "2706000", "310200" }, // OLIVENÇA/AL
            { "2706109", "310300" }, // OURO BRANCO/AL
            { "2706208", "310400" }, // PALESTINA/AL
            { "2706307", "312100" }, // PALMEIRA DOS ÍNDIOS/AL
            { "2706406", "310500" }, // PÃO DE AÇÚCAR/AL
            { "2706422", "115600" }, // PARICONHA/AL
            { "2706448", "115700" }, // PARIPUEIRA/AL
            { "2706505", "318700" }, // PASSO DE CAMARAGIBE/AL
            { "2706604", "312200" }, // PAULO JACINTO/AL
            { "2706703", "315000" }, // PENEDO/AL
            { "2706802", "315100" }, // PIAÇABUÇU/AL
            { "2706901", "321000" }, // PILAR/AL
            { "2707008", "317000" }, // PINDOBA/AL
            { "2707107", "308700" }, // PIRANHAS/AL
            { "2707206", "310600" }, // POÇO DAS TRINCHEIRAS/AL
            { "2707305", "318800" }, // PORTO CALVO/AL
            { "2707404", "318900" }, // PORTO DE PEDRAS/AL
            { "2707503", "315200" }, // PORTO REAL DO COLÉGIO/AL
            { "2707602", "312300" }, // QUEBRANGULO/AL
            { "2707701", "321100" }, // RIO LARGO/AL
            { "2707800", "320100" }, // ROTEIRO/AL
            { "2707909", "321200" }, // SANTA LUZIA DO NORTE/AL
            { "2708006", "310700" }, // SANTANA DO IPANEMA/AL
            { "2708105", "317100" }, // SANTANA DO MUNDAÚ/AL
            { "2708204", "314100" }, // SÃO BRÁS/AL
            { "2708303", "317200" }, // SÃO JOSÉ DA LAJE/AL
            { "2708402", "310800" }, // SÃO JOSÉ DA TAPERA/AL
            { "2708501", "319000" }, // SÃO LUÍS DO QUITUNDE/AL
            { "2708600", "320200" }, // SÃO MIGUEL DOS CAMPOS/AL
            { "2708709", "319100" }, // SÃO MIGUEL DOS MILAGRES/AL
            { "2708808", "314200" }, // SÃO SEBASTIÃO/AL
            { "2708907", "321300" }, // SATUBA/AL
            { "2708956", "310900" }, // SENADOR RUI PALMEIRA/AL
            { "2709004", "312400" }, // TANQUE D'ARCA/AL
            { "2709103", "314300" }, // TAQUARANA/AL
            { "2709152", "023200" }, // TEOTÔNIO VILELA/AL
            { "2709202", "314400" }, // TRAIPU/AL
            { "2709301", "317300" }, // UNIÃO DOS PALMARES/AL
            { "2709400", "317400" }, // VIÇOSA/AL
            { "1300029", "085000" }, // ALVARÃES/AM
            { "1300060", "079800" }, // AMATURÁ/AM
            { "1300086", "085100" }, // ANAMÃ/AM
            { "1300102", "085200" }, // ANORI/AM
            { "1300144", "002300" }, // APUÍ/AM
            { "1300201", "079900" }, // ATALAIA DO NORTE/AM
            { "1300300", "086200" }, // AUTAZES/AM
            { "1300409", "083900" }, // BARCELOS/AM
            { "1300508", "086300" }, // BARREIRINHA/AM
            { "1300607", "080000" }, // BENJAMIN CONSTANT/AM
            { "1300631", "086400" }, // BERURI/AM
            { "1300680", "088100" }, // BOA VISTA DO RAMOS/AM
            { "1300706", "082000" }, // BOCA DO ACRE/AM
            { "1300805", "082900" }, // BORBA/AM
            { "1300839", "086500" }, // CAAPIRANGA/AM
            { "1300904", "082100" }, // CANUTAMA/AM
            { "1301001", "081100" }, // CARAUARI/AM
            { "1301100", "086600" }, // CAREIRO/AM
            { "1301159", "002200" }, // CAREIRO DA VÁRZEA/AM
            { "1301209", "085300" }, // COARI/AM
            { "1301308", "085400" }, // CODAJÁS/AM
            { "1301407", "081200" }, // EIRUNEPÉ/AM
            { "1301506", "081300" }, // ENVIRA/AM
            { "1301605", "080100" }, // FONTE BOA/AM
            { "1301654", "002100" }, // GUAJARÁ/AM
            { "1301704", "083000" }, // HUMAITÁ/AM
            { "1301803", "081400" }, // IPIXUNA/AM
            { "1301852", "086700" }, // IRANDUBA/AM
            { "1301902", "086800" }, // ITACOATIARA/AM
            { "1301951", "081500" }, // ITAMARATI/AM
            { "1302009", "086900" }, // ITAPIRANGA/AM
            { "1302108", "085500" }, // JAPURÁ/AM
            { "1302207", "081600" }, // JURUÁ/AM
            { "1302306", "080200" }, // JUTAÍ/AM
            { "1302405", "082200" }, // LÁBREA/AM
            { "1302504", "087000" }, // MANACAPURU/AM
            { "1302553", "088200" }, // MANAQUIRI/AM
            { "1302603", "087100" }, // MANAUS/AM
            { "1302702", "083100" }, // MANICORÉ/AM
            { "1302801", "085600" }, // MARAÃ/AM
            { "1302900", "087200" }, // MAUÉS/AM
            { "1303007", "087300" }, // NHAMUNDÁ/AM
            { "1303106", "087400" }, // NOVA OLINDA DO NORTE/AM
            { "1303205", "084000" }, // NOVO AIRÃO/AM
            { "1303304", "083200" }, // NOVO ARIPUANÃ/AM
            { "1303403", "087500" }, // PARINTINS/AM
            { "1303502", "082300" }, // PAUINI/AM
            { "1303536", "088300" }, // PRESIDENTE FIGUEIREDO/AM
            { "1303569", "087600" }, // RIO PRETO DA EVA/AM
            { "1303601", "084100" }, // SANTA ISABEL DO RIO NEGRO/AM
            { "1303700", "080300" }, // SANTO ANTÔNIO DO IÇÁ/AM
            { "1303809", "084200" }, // SÃO GABRIEL DA CACHOEIRA/AM
            { "1303908", "080400" }, // SÃO PAULO DE OLIVENÇA/AM
            { "1303957", "087700" }, // SÃO SEBASTIÃO DO UATUMÃ/AM
            { "1304005", "087800" }, // SILVES/AM
            { "1304062", "080500" }, // TABATINGA/AM
            { "1304104", "082400" }, // TAPAUÁ/AM
            { "1304203", "085700" }, // TEFÉ/AM
            { "1304237", "080600" }, // TONANTINS/AM
            { "1304260", "085800" }, // UARINI/AM
            { "1304302", "087900" }, // URUCARÁ/AM
            { "1304401", "088000" }, // URUCURITUBA/AM
            { "1600105", "117400" }, // AMAPÁ/AP
            { "1600154", "" }, // AMAPARI/AP
            { "1600204", "117500" }, // CALÇOENE/AP
            { "1600212", "110400" }, // CUTIAS/AP
            { "1600238", "008800" }, // FERREIRA GOMES/AP
            { "1600253", "152100" }, // ITAUBAL/AP
            { "1600279", "008600" }, // LARANJAL DO JARI/AP
            { "1600303", "116300" }, // MACAPÁ/AP
            { "1600402", "116400" }, // MAZAGÃO/AP
            { "1600501", "117600" }, // OIAPOQUE/AP
            { "1600535", "067800" }, // PORTO GRANDE/AP
            { "1600550", "110500" }, // PRACUÚBA/AP
            { "1600600", "008500" }, // SANTANA/AP
            { "1600055", "176100" }, // SERRA DO NAVIO/AP
            { "1600709", "008700" }, // TARTARUGALZINHO/AP
            { "1600808", "152700" }, // VITÓRIA DO JARI/AP
            { "2900108", "344600" }, // ABAÍRA/BA
            { "2900207", "358900" }, // ABARÉ/BA
            { "2900306", "381500" }, // ACAJUTIBA/BA
            { "2900355", "033500" }, // ADUSTINA/BA
            { "2900405", "367700" }, // ÁGUA FRIA/BA
            { "2900603", "372100" }, // AIQUARA/BA
            { "2900702", "381600" }, // ALAGOINHAS/BA
            { "2900801", "409100" }, // ALCOBAÇA/BA
            { "2900900", "401900" }, // ALMADINA/BA
            { "2901007", "372200" }, // AMARGOSA/BA
            { "2901106", "386000" }, // AMÉLIA RODRIGUES/BA
            { "2901155", "342300" }, // AMÉRICA DOURADA/BA
            { "2901205", "376600" }, // ANAGÉ/BA
            { "2901304", "344800" }, // ANDARAÍ/BA
            { "2901353", "030800" }, // ANDORINHA/BA
            { "2901403", "332500" }, // ANGICAL/BA
            { "2901502", "367800" }, // ANGUERA/BA
            { "2901601", "381700" }, // ANTAS/BA
            { "2901700", "367900" }, // ANTÔNIO CARDOSO/BA
            { "2901809", "356900" }, // ANTÔNIO GONÇALVES/BA
            { "2901908", "381800" }, // APORÁ/BA
            { "2901957", "029600" }, // APUAREMA/BA
            { "2902054", "041300" }, // ARAÇÁS/BA
            { "2902005", "351600" }, // ARACATU/BA
            { "2902104", "365900" }, // ARACI/BA
            { "2902203", "381900" }, // ARAMARI/BA
            { "2902252", "404700" }, // ARATACA/BA
            { "2902302", "386100" }, // ARATUÍPE/BA
            { "2902401", "402000" }, // AURELINO LEAL/BA
            { "2902500", "332600" }, // BAIANÓPOLIS/BA
            { "2902609", "360800" }, // BAIXA GRANDE/BA
            { "2902658", "035900" }, // BANZAÊ/BA
            { "2902708", "336200" }, // BARRA/BA
            { "2902807", "344900" }, // BARRA DA ESTIVA/BA
            { "2902906", "376700" }, // BARRA DO CHOÇA/BA
            { "2903003", "341000" }, // BARRA DO MENDES/BA
            { "2903102", "402100" }, // BARRA DO ROCHA/BA
            { "2903201", "332700" }, // BARREIRAS/BA
            { "2903235", "342600" }, // BARRO ALTO/BA
            { "2903276", "367300" }, // BARROCAS/BA
            { "2903409", "402300" }, // BELMONTE/BA
            { "2903508", "081900" }, // BELO CAMPO/BA
            { "2903607", "366000" }, // BIRITINGA/BA
            { "2903706", "376900" }, // BOA NOVA/BA
            { "2903805", "360900" }, // BOA VISTA DO TUPIM/BA
            { "2903904", "339800" }, // BOM JESUS DA LAPA/BA
            { "2903953", "041200" }, // BOM JESUS DA SERRA/BA
            { "2904001", "345000" }, // BONINAL/BA
            { "2904050", "030400" }, // BONITO/BA
            { "2904100", "345100" }, // BOQUIRA/BA
            { "2904209", "345200" }, // BOTUPORÃ/BA
            { "2904308", "372300" }, // BREJÕES/BA
            { "2904407", "332800" }, // BREJOLÂNDIA/BA
            { "2904506", "345300" }, // BROTAS DE MACAÚBAS/BA
            { "2904605", "351700" }, // BRUMADO/BA
            { "2904704", "402400" }, // BUERAREMA/BA
            { "2904753", "337100" }, // BURITIRAMA/BA
            { "2904803", "377000" }, // CAATIBA/BA
            { "2904852", "031600" }, // CABACEIRAS DO PARAGUAÇU/BA
            { "2904902", "386200" }, // CACHOEIRA/BA
            { "2905008", "351800" }, // CACULÉ/BA
            { "2905107", "361000" }, // CAÉM/BA
            { "2905156", "029800" }, // CAETANOS/BA
            { "2905206", "351900" }, // CAETITÉ/BA
            { "2905305", "341100" }, // CAFARNAUM/BA
            { "2905404", "397300" }, // CAIRU/BA
            { "2905503", "357000" }, // CALDEIRÃO GRANDE/BA
            { "2905602", "402500" }, // CAMACAN/BA
            { "2905701", "392800" }, // CAMAÇARI/BA
            { "2905800", "397400" }, // CAMAMU/BA
            { "2905909", "336300" }, // CAMPO ALEGRE DE LOURDES/BA
            { "2906006", "342001" }, // CAMPO FORMOSO/BA
            { "2906105", "334900" }, // CANÁPOLIS/BA
            { "2906204", "341200" }, // CANARANA/BA
            { "2906303", "402600" }, // CANAVIEIRAS/BA
            { "2906402", "366100" }, // CANDEAL/BA
            { "2906501", "392900" }, // CANDEIAS/BA
            { "2906600", "352000" }, // CANDIBA/BA
            { "2906709", "377100" }, // CÂNDIDO SALES/BA
            { "2906808", "364300" }, // CANSANÇÃO/BA
            { "2906824", "030601" }, // CANUDOS/BA
            { "2906857", "367200" }, // CAPELA DO ALTO ALEGRE/BA
            { "2906873", "362500" }, // CAPIM GROSSO/BA
            { "2906899", "030900" }, // CARAÍBAS/BA
            { "2906907", "409200" }, // CARAVELAS/BA
            { "2907004", "384500" }, // CARDEAL DA SILVA/BA
            { "2907103", "339900" }, // CARINHANHA/BA
            { "2907202", "336400" }, // CASA NOVA/BA
            { "2907301", "368000" }, // CASTRO ALVES/BA
            { "2907400", "332900" }, // CATOLÂNDIA/BA
            { "2907509", "393000" }, // CATU/BA
            { "2907558", "031900" }, // CATURAMA/BA
            { "2907608", "341300" }, // CENTRAL/BA
            { "2907707", "359000" }, // CHORROCHÓ/BA
            { "2907806", "382000" }, // CÍCERO DANTAS/BA
            { "2907905", "382100" }, // CIPÓ/BA
            { "2908002", "402700" }, // COARACI/BA
            { "2908101", "335000" }, // COCOS/BA
            { "2908200", "386300" }, // CONCEIÇÃO DA FEIRA/BA
            { "2908309", "386400" }, // CONCEIÇÃO DO ALMEIDA/BA
            { "2908408", "366200" }, // CONCEIÇÃO DO COITÉ/BA
            { "2908507", "386500" }, // CONCEIÇÃO DO JACUÍPE/BA
            { "2908606", "384600" }, // CONDE/BA
            { "2908705", "352100" }, // CONDEÚBA/BA
            { "2908804", "345400" }, // CONTENDAS DO SINCORÁ/BA
            { "2908903", "368100" }, // CORAÇÃO DE MARIA/BA
            { "2909000", "352200" }, // CORDEIROS/BA
            { "2909109", "335100" }, // CORIBE/BA
            { "2909208", "380400" }, // CORONEL JOÃO SÁ/BA
            { "2909307", "335200" }, // CORRENTINA/BA
            { "2909406", "333000" }, // COTEGIPE/BA
            { "2909505", "372400" }, // CRAVOLÂNDIA/BA
            { "2909604", "382200" }, // CRISÓPOLIS/BA
            { "2909703", "333100" }, // CRISTÓPOLIS/BA
            { "2909802", "386600" }, // CRUZ DAS ALMAS/BA
            { "2909901", "359100" }, // CURAÇÁ/BA
            { "2910008", "377200" }, // DÁRIO MEIRA/BA
            { "2910057", "398100" }, // DIAS D'ÁVILA/BA
            { "2910107", "352300" }, // DOM BASÍLIO/BA
            { "2910206", "386700" }, // DOM MACEDO COSTA/BA
            { "2910305", "368200" }, // ELÍSIO MEDRADO/BA
            { "2910404", "379100" }, // ENCRUZILHADA/BA
            { "2900504", "033600" }, // ÉRICO CARDOSO/BA
            { "2910602", "384800" }, // ESPLANADA/BA
            { "2910701", "364400" }, // EUCLIDES DA CUNHA/BA
            { "2910727", "410000" }, // EUNÁPOLIS/BA
            { "2910750", "383200" }, // FÁTIMA/BA
            { "2910776", "038800" }, // FEIRA DA MATA/BA
            { "2910800", "368300" }, // FEIRA DE SANTANA/BA
            { "2910859", "357700" }, // FILADÉLFIA/BA
            { "2910909", "399700" }, // FIRMINO ALVES/BA
            { "2911006", "399800" }, // FLORESTA AZUL/BA
            { "2911105", "333200" }, // FORMOSA DO RIO PRETO/BA
            { "2911204", "402800" }, // GANDU/BA
            { "2911253", "367400" }, // GAVIÃO/BA
            { "2911303", "341400" }, // GENTIO DO OURO/BA
            { "2911402", "380500" }, // GLÓRIA/BA
            { "2911501", "402900" }, // GONGOGI/BA
            { "2903300", "" }, // GOVERNADOR LOMANTO JÚNIOR/BA
            { "2911600", "386800" }, // GOVERNADOR MANGABEIRA/BA
            { "2911659", "354100" }, // GUAJERU/BA
            { "2911709", "352400" }, // GUANAMBI/BA
            { "2911808", "407600" }, // GUARATINGA/BA
            { "2911857", "383300" }, // HELIÓPOLIS/BA
            { "2911907", "368400" }, // IAÇU/BA
            { "2912004", "352500" }, // IBIASSUCÊ/BA
            { "2912103", "403000" }, // IBICARAÍ/BA
            { "2912202", "345500" }, // IBICOARA/BA
            { "2912301", "399900" }, // IBICUÍ/BA
            { "2912400", "341500" }, // IBIPEBA/BA
            { "2912509", "345600" }, // IBIPITANGA/BA
            { "2912608", "361100" }, // IBIQUERA/BA
            { "2912707", "397500" }, // IBIRAPITANGA/BA
            { "2912806", "407700" }, // IBIRAPUÃ/BA
            { "2912905", "403100" }, // IBIRATAIA/BA
            { "2913002", "345700" }, // IBITIARA/BA
            { "2913101", "341600" }, // IBITITÁ/BA
            { "2913200", "336500" }, // IBOTIRAMA/BA
            { "2913309", "366300" }, // ICHU/BA
            { "2913408", "352600" }, // IGAPORÃ/BA
            { "2913457", "030200" }, // IGRAPIÚNA/BA
            { "2913507", "400000" }, // IGUAÍ/BA
            { "2913606", "403200" }, // ILHÉUS/BA
            { "2913705", "382300" }, // INHAMBUPE/BA
            { "2913804", "368500" }, // IPECAETÁ/BA
            { "2913903", "403300" }, // IPIAÚ/BA
            { "2914000", "368600" }, // IPIRÁ/BA
            { "2914109", "345800" }, // IPUPIARA/BA
            { "2914208", "372500" }, // IRAJUBA/BA
            { "2914307", "345900" }, // IRAMAIA/BA
            { "2914406", "346000" }, // IRAQUARA/BA
            { "2914505", "368700" }, // IRARÁ/BA
            { "2914604", "341700" }, // IRECÊ/BA
            { "2914653", "042800" }, // ITABELA/BA
            { "2914703", "361200" }, // ITABERABA/BA
            { "2914802", "403400" }, // ITABUNA/BA
            { "2914901", "403500" }, // ITACARÉ/BA
            { "2915007", "346100" }, // ITAETÉ/BA
            { "2915106", "372600" }, // ITAGI/BA
            { "2915205", "400100" }, // ITAGIBÁ/BA
            { "2915304", "400200" }, // ITAGIMIRIM/BA
            { "2915353", "038600" }, // ITAGUAÇU DA BAHIA/BA
            { "2915403", "400300" }, // ITAJU DO COLÔNIA/BA
            { "2915502", "403600" }, // ITAJUÍPE/BA
            { "2915601", "407800" }, // ITAMARAJU/BA
            { "2915700", "403700" }, // ITAMARI/BA
            { "2915809", "379200" }, // ITAMBÉ/BA
            { "2915908", "384900" }, // ITANAGRA/BA
            { "2916005", "407900" }, // ITANHÉM/BA
            { "2916104", "386900" }, // ITAPARICA/BA
            { "2916203", "403800" }, // ITAPÉ/BA
            { "2916302", "400400" }, // ITAPEBI/BA
            { "2916401", "379300" }, // ITAPETINGA/BA
            { "2916500", "382400" }, // ITAPICURU/BA
            { "2916609", "403900" }, // ITAPITANGA/BA
            { "2916708", "372700" }, // ITAQUARA/BA
            { "2916807", "379400" }, // ITARANTIM/BA
            { "2916856", "042700" }, // ITATIM/BA
            { "2916906", "372800" }, // ITIRUÇU/BA
            { "2917003", "364500" }, // ITIÚBA/BA
            { "2917102", "400500" }, // ITORORÓ/BA
            { "2917201", "346200" }, // ITUAÇU/BA
            { "2917300", "397600" }, // ITUBERÁ/BA
            { "2917334", "041000" }, // IUIU/BA
            { "2917359", "042600" }, // JABORANDI/BA
            { "2917409", "352700" }, // JACARACI/BA
            { "2917508", "361300" }, // JACOBINA/BA
            { "2917607", "372900" }, // JAGUAQUARA/BA
            { "2917706", "357200" }, // JAGUARARI/BA
            { "2917805", "387000" }, // JAGUARIPE/BA
            { "2917904", "385000" }, // JANDAÍRA/BA
            { "2918001", "373000" }, // JEQUIÉ/BA
            { "2918100", "380600" }, // JEREMOABO/BA
            { "2918308", "373200" }, // JITAÚNA/BA
            { "2918357", "342500" }, // JOÃO DOURADO/BA
            { "2918407", "359200" }, // JUAZEIRO/BA
            { "2918456", "041400" }, // JUCURUÇU/BA
            { "2918506", "341800" }, // JUSSARA/BA
            { "2918555", "404800" }, // JUSSARI/BA
            { "2918605", "346300" }, // JUSSIAPE/BA
            { "2918704", "373300" }, // LAFAIETE COUTINHO/BA
            { "2918753", "033300" }, // LAGOA REAL/BA
            { "2918803", "373400" }, // LAJE/BA
            { "2918902", "408000" }, // LAJEDÃO/BA
            { "2919009", "361400" }, // LAJEDINHO/BA
            { "2919058", "029700" }, // LAJEDO DO TABOCAL/BA
            { "2919108", "366400" }, // LAMARÃO/BA
            { "2919157", "342700" }, // LAPÃO/BA
            { "2919207", "393100" }, // LAURO DE FREITAS/BA
            { "2919306", "346400" }, // LENÇÓIS/BA
            { "2919405", "352800" }, // LICÍNIO DE ALMEIDA/BA
            { "2919504", "352900" }, // LIVRAMENTO DE NOSSA SENHORA/BA
            { "2919553", "" }, // LUÍS EDUARDO MAGALHÃES/BA
            { "2919603", "361500" }, // MACAJUBA/BA
            { "2919702", "379500" }, // MACARANI/BA
            { "2919801", "346500" }, // MACAÚBAS/BA
            { "2919900", "359300" }, // MACURURÉ/BA
            { "2919926", "031000" }, // MADRE DE DEUS/BA
            { "2919959", "354200" }, // MAETINGA/BA
            { "2920007", "379600" }, // MAIQUINIQUE/BA
            { "2920106", "361600" }, // MAIRI/BA
            { "2920205", "340000" }, // MALHADA/BA
            { "2920304", "353000" }, // MALHADA DE PEDRAS/BA
            { "2920403", "377300" }, // MANOEL VITORINO/BA
            { "2920452", "333700" }, // MANSIDÃO/BA
            { "2920502", "373500" }, // MARACÁS/BA
            { "2920601", "387100" }, // MARAGOGIPE/BA
            { "2920700", "397700" }, // MARAÚ/BA
            { "2920809", "373600" }, // MARCIONÍLIO SOUZA/BA
            { "2920908", "404000" }, // MASCOTE/BA
            { "2921005", "393200" }, // MATA DE SÃO JOÃO/BA
            { "2921054", "033400" }, // MATINA/BA
            { "2921104", "408100" }, // MEDEIROS NETO/BA
            { "2921203", "361700" }, // MIGUEL CALMON/BA
            { "2921302", "373700" }, // MILAGRES/BA
            { "2921401", "357300" }, // MIRANGABA/BA
            { "2921450", "029900" }, // MIRANTE/BA
            { "2921500", "364600" }, // MONTE SANTO/BA
            { "2921609", "336600" }, // MORPARÁ/BA
            { "2921708", "341900" }, // MORRO DO CHAPÉU/BA
            { "2921807", "353100" }, // MORTUGABA/BA
            { "2921906", "346600" }, // MUCUGÊ/BA
            { "2922003", "409300" }, // MUCURI/BA
            { "2922052", "030600" }, // MULUNGU DO MORRO/BA
            { "2922102", "361800" }, // MUNDO NOVO/BA
            { "2922201", "387200" }, // MUNIZ FERREIRA/BA
            { "2922250", "" }, // MUQUÉM DE SÃO FRANCISCO/BA
            { "2922300", "387300" }, // MURITIBA/BA
            { "2922409", "373800" }, // MUTUÍPE/BA
            { "2922508", "387400" }, // NAZARÉ/BA
            { "2922607", "397800" }, // NILO PEÇANHA/BA
            { "2922656", "365200" }, // NORDESTINA/BA
            { "2922706", "377400" }, // NOVA CANAÃ/BA
            { "2922730", "036100" }, // NOVA FÁTIMA/BA
            { "2922755", "030100" }, // NOVA IBIÁ/BA
            { "2922805", "373900" }, // NOVA ITARANA/BA
            { "2922854", "030500" }, // NOVA REDENÇÃO/BA
            { "2922904", "382500" }, // NOVA SOURE/BA
            { "2923001", "409400" }, // NOVA VIÇOSA/BA
            { "2923035", "032000" }, // NOVO HORIZONTE/BA
            { "2923050", "036000" }, // NOVO TRIUNFO/BA
            { "2923100", "382600" }, // OLINDINA/BA
            { "2923209", "346700" }, // OLIVEIRA DOS BREJINHOS/BA
            { "2923308", "368800" }, // OURIÇANGAS/BA
            { "2923357", "031100" }, // OUROLÂNDIA/BA
            { "2923407", "353200" }, // PALMAS DE MONTE ALTO/BA
            { "2923506", "346800" }, // PALMEIRAS/BA
            { "2923605", "346900" }, // PARAMIRIM/BA
            { "2923704", "340100" }, // PARATINGA/BA
            { "2923803", "382700" }, // PARIPIRANGA/BA
            { "2923902", "400600" }, // PAU BRASIL/BA
            { "2924009", "380700" }, // PAULO AFONSO/BA
            { "2924058", "367100" }, // PÉ DE SERRA/BA
            { "2924108", "368900" }, // PEDRÃO/BA
            { "2924207", "380800" }, // PEDRO ALEXANDRE/BA
            { "2924306", "347000" }, // PIATÃ/BA
            { "2924405", "336700" }, // PILÃO ARCADO/BA
            { "2924504", "353300" }, // PINDAÍ/BA
            { "2924603", "357400" }, // PINDOBAÇU/BA
            { "2924652", "369700" }, // PINTADAS/BA
            { "2924678", "030300" }, // PIRAÍ DO NORTE/BA
            { "2924702", "353400" }, // PIRIPÁ/BA
            { "2924801", "361900" }, // PIRITIBA/BA
            { "2924900", "374000" }, // PLANALTINO/BA
            { "2925006", "377500" }, // PLANALTO/BA
            { "2925105", "377600" }, // POÇÕES/BA
            { "2925204", "393300" }, // POJUCA/BA
            { "2925253", "031300" }, // PONTO NOVO/BA
            { "2910503", "384703" }, // PORTO SAUÍPE/BA
            { "2925303", "409500" }, // PORTO SEGURO/BA
            { "2925402", "400700" }, // POTIRAGUÁ/BA
            { "2925501", "409600" }, // PRADO/BA
            { "2925600", "342000" }, // PRESIDENTE DUTRA/BA
            { "2925709", "353500" }, // PRESIDENTE JÂNIO QUADROS/BA
            { "2925758", "038500" }, // PRESIDENTE TANCREDO NEVES/BA
            { "2925808", "364700" }, // QUEIMADAS/BA
            { "2925907", "364800" }, // QUIJINGUE/BA
            { "2925931", "031400" }, // QUIXABEIRA/BA
            { "2925956", "369600" }, // RAFAEL JAMBEIRO/BA
            { "2926004", "336800" }, // REMANSO/BA
            { "2926103", "366500" }, // RETIROLÂNDIA/BA
            { "2926202", "333300" }, // RIACHÃO DAS NEVES/BA
            { "2926301", "366600" }, // RIACHÃO DO JACUÍPE/BA
            { "2926400", "353600" }, // RIACHO DE SANTANA/BA
            { "2926509", "382800" }, // RIBEIRA DO AMPARO/BA
            { "2926608", "382900" }, // RIBEIRA DO POMBAL/BA
            { "2926657", "041100" }, // RIBEIRÃO DO LARGO/BA
            { "2926707", "347100" }, // RIO DE CONTAS/BA
            { "2926806", "353700" }, // RIO DO ANTÔNIO/BA
            { "2926905", "347200" }, // RIO DO PIRES/BA
            { "2927002", "383000" }, // RIO REAL/BA
            { "2927101", "359400" }, // RODELAS/BA
            { "2927200", "362000" }, // RUY BARBOSA/BA
            { "2927309", "387500" }, // SALINAS DA MARGARIDA/BA
            { "2927408", "393400" }, // SALVADOR/BA
            { "2927507", "369000" }, // SANTA BÁRBARA/BA
            { "2927606", "380900" }, // SANTA BRÍGIDA/BA
            { "2927705", "409700" }, // SANTA CRUZ CABRÁLIA/BA
            { "2927804", "400800" }, // SANTA CRUZ DA VITÓRIA/BA
            { "2927903", "374100" }, // SANTA INÊS/BA
            { "2928059", "409900" }, // SANTA LUZIA/BA
            { "2928109", "335300" }, // SANTA MARIA DA VITÓRIA/BA
            { "2928406", "333400" }, // SANTA RITA DE CÁSSIA/BA
            { "2928505", "" }, // SANTA TEREZINHA/BA
            { "2928000", "366700" }, // SANTALUZ/BA
            { "2928208", "335400" }, // SANTANA/BA
            { "2928307", "369100" }, // SANTANÓPOLIS/BA
            { "2928604", "387600" }, // SANTO AMARO/BA
            { "2928703", "387700" }, // SANTO ANTÔNIO DE JESUS/BA
            { "2928802", "369300" }, // SANTO ESTEVÃO/BA
            { "2928901", "333500" }, // SÃO DESIDÉRIO/BA
            { "2928950", "036200" }, // SÃO DOMINGOS/BA
            { "2929107", "387900" }, // SÃO FELIPE/BA
            { "2929008", "387800" }, // SÃO FÉLIX/BA
            { "2929057", "035800" }, // SÃO FÉLIX DO CORIBE/BA
            { "2929206", "393500" }, // SÃO FRANCISCO DO CONDE/BA
            { "2929255", "342400" }, // SÃO GABRIEL/BA
            { "2929305", "388000" }, // SÃO GONÇALO DOS CAMPOS/BA
            { "2929354", "038300" }, // SÃO JOSÉ DA VITÓRIA/BA
            { "2929370", "031500" }, // SÃO JOSÉ DO JACUÍPE/BA
            { "2929404", "374200" }, // SÃO MIGUEL DAS MATAS/BA
            { "2929503", "388100" }, // SÃO SEBASTIÃO DO PASSE/BA
            { "2929602", "388200" }, // SAPEAÇU/BA
            { "2929701", "383100" }, // SÁTIRO DIAS/BA
            { "2929750", "031700" }, // SAUBARA/BA
            { "2929800", "357500" }, // SAÚDE/BA
            { "2929909", "347300" }, // SEABRA/BA
            { "2930006", "353800" }, // SEBASTIÃO LARANJEIRAS/BA
            { "2930105", "357600" }, // SENHOR DO BONFIM/BA
            { "2930204", "336900" }, // SENTO SÉ/BA
            { "2930154", "038900" }, // SERRA DO RAMALHO/BA
            { "2930303", "335500" }, // SERRA DOURADA/BA
            { "2930402", "369400" }, // SERRA PRETA/BA
            { "2930501", "366800" }, // SERRINHA/BA
            { "2930600", "362100" }, // SERROLÂNDIA/BA
            { "2930709", "393600" }, // SIMÕES FILHO/BA
            { "2930758", "030700" }, // SÍTIO DO MATO/BA
            { "2930766", "030000" }, // SÍTIO DO QUINTO/BA
            { "2930774", "039000" }, // SOBRADINHO/BA
            { "2930808", "342100" }, // SOUTO SOARES/BA
            { "2930907", "333600" }, // TABOCAS DO BREJO VELHO/BA
            { "2931004", "347400" }, // TANHAÇU/BA
            { "2931053", "347700" }, // TANQUE NOVO/BA
            { "2931103", "369500" }, // TANQUINHO/BA
            { "2931202", "397900" }, // TAPEROÁ/BA
            { "2931301", "362200" }, // TAPIRAMUTÁ/BA
            { "2931350", "409800" }, // TEIXEIRA DE FREITAS/BA
            { "2931400", "388300" }, // TEODORO SAMPAIO/BA
            { "2931509", "366900" }, // TEOFILÂNDIA/BA
            { "2931608", "404100" }, // TEOLÂNDIA/BA
            { "2931707", "388400" }, // TERRA NOVA/BA
            { "2931806", "353900" }, // TREMEDAL/BA
            { "2931905", "364900" }, // TUCANO/BA
            { "2932002", "365000" }, // UAUÁ/BA
            { "2932101", "374300" }, // UBAÍRA/BA
            { "2932200", "404200" }, // UBAITABA/BA
            { "2932309", "404300" }, // UBATÃ/BA
            { "2932408", "342200" }, // UIBAÍ/BA
            { "2932457", "031200" }, // UMBURANAS/BA
            { "2932507", "404400" }, // UNA/BA
            { "2932606", "354000" }, // URANDI/BA
            { "2932705", "404500" }, // URUÇUCA/BA
            { "2932804", "347500" }, // UTINGA/BA
            { "2932903", "398000" }, // VALENÇA/BA
            { "2933000", "367000" }, // VALENTE/BA
            { "2933059", "362400" }, // VÁRZEA DA ROÇA/BA
            { "2933109", "362300" }, // VÁRZEA DO POÇO/BA
            { "2933158", "361309" }, // VÁRZEA NOVA/BA
            { "2933174", "031800" }, // VARZEDO/BA
            { "2933208", "388500" }, // VERA CRUZ/BA
            { "2933257", "038400" }, // VEREDA/BA
            { "2933307", "377700" }, // VITÓRIA DA CONQUISTA/BA
            { "2933406", "347600" }, // WAGNER/BA
            { "2933455", "333800" }, // WANDERLEY/BA
            { "2933505", "404600" }, // WENCESLAU GUIMARÃES/BA
            { "2933604", "337000" }, // XIQUE-XIQUE/BA
            { "2300101", "210600" }, // ABAIARA/CE
            { "2300150", "018300" }, // ACARAPE/CE
            { "2300200", "155800" }, // ACARAÚ/CE
            { "2300309", "202800" }, // ACOPIARA/CE
            { "2300408", "200800" }, // AIUABA/CE
            { "2300507", "167900" }, // ALCÂNTARAS/CE
            { "2300606", "208000" }, // ALTANEIRA/CE
            { "2300705", "185800" }, // ALTO SANTO/CE
            { "2300754", "161000" }, // AMONTADA/CE
            { "2300804", "208100" }, // ANTONINA DO NORTE/CE
            { "2300903", "160000" }, // APUIARÉS/CE
            { "2301000", "180600" }, // AQUIRAZ/CE
            { "2301109", "185900" }, // ARACATI/CE
            { "2301208", "189500" }, // ARACOIABA/CE
            { "2301257", "068100" }, // ARARENDÁ/CE
            { "2301307", "213100" }, // ARARIPE/CE
            { "2301406", "189600" }, // ARATUBA/CE
            { "2301505", "200900" }, // ARNEIROZ/CE
            { "2301604", "208200" }, // ASSARÉ/CE
            { "2301703", "210700" }, // AURORA/CE
            { "2301802", "205900" }, // BAIXIO/CE
            { "2301851", "192900" }, // BANABUIÚ/CE
            { "2301901", "215200" }, // BARBALHA/CE
            { "2301950", "190500" }, // BARREIRA/CE
            { "2302008", "210800" }, // BARRO/CE
            { "2302057", "156500" }, // BARROQUINHA/CE
            { "2302107", "189700" }, // BATURITÉ/CE
            { "2302206", "184000" }, // BEBERIBE/CE
            { "2302305", "155900" }, // BELA CRUZ/CE
            { "2302404", "192500" }, // BOA VIAGEM/CE
            { "2302503", "210900" }, // BREJO SANTO/CE
            { "2302602", "156000" }, // CAMOCIM/CE
            { "2302701", "213200" }, // CAMPOS SALES/CE
            { "2302800", "172700" }, // CANINDÉ/CE
            { "2302909", "189800" }, // CAPISTRANO/CE
            { "2303006", "172800" }, // CARIDADE/CE
            { "2303105", "168000" }, // CARIRÉ/CE
            { "2303204", "208300" }, // CARIRIAÇU/CE
            { "2303303", "202900" }, // CARIÚS/CE
            { "2303402", "165100" }, // CARNAUBAL/CE
            { "2303501", "184100" }, // CASCAVEL/CE
            { "2303600", "201000" }, // CATARINA/CE
            { "2303659", "067900" }, // CATUNDA/CE
            { "2303709", "180700" }, // CAUCAIA/CE
            { "2303808", "206000" }, // CEDRO/CE
            { "2303907", "156100" }, // CHAVAL/CE
            { "2303931", "068200" }, // CHORÓ/CE
            { "2303956", "184300" }, // CHOROZINHO/CE
            { "2304004", "168100" }, // COREAÚ/CE
            { "2304103", "177700" }, // CRATEÚS/CE
            { "2304202", "215300" }, // CRATO/CE
            { "2304236", "018500" }, // CROATÁ/CE
            { "2304251", "018600" }, // CRUZ/CE
            { "2304269", "018800" }, // DEPUTADO IRAPUAN PINHEIRO/CE
            { "2304277", "199900" }, // ERERÊ/CE
            { "2304285", "181100" }, // EUSÉBIO/CE
            { "2304301", "208400" }, // FARIAS BRITO/CE
            { "2304350", "169200" }, // FORQUILHA/CE
            { "2304400", "180800" }, // FORTALEZA/CE
            { "2304459", "068000" }, // FORTIM/CE
            { "2304509", "168200" }, // FRECHEIRINHA/CE
            { "2304608", "172900" }, // GENERAL SAMPAIO/CE
            { "2304657", "165800" }, // GRAÇA/CE
            { "2304707", "156200" }, // GRANJA/CE
            { "2304806", "208500" }, // GRANJEIRO/CE
            { "2304905", "168300" }, // GROAIRAS/CE
            { "2304954", "018400" }, // GUAIÚBA/CE
            { "2305001", "165200" }, // GUARACIABA DO NORTE/CE
            { "2305100", "189900" }, // GUARAMIRANGA/CE
            { "2305209", "173000" }, // HIDROLÂNDIA/CE
            { "2305233", "184400" }, // HORIZONTE/CE
            { "2305266", "193000" }, // IBARETAMA/CE
            { "2305308", "165300" }, // IBIAPINA/CE
            { "2305332", "186900" }, // IBICUITINGA/CE
            { "2305357", "187000" }, // ICAPUÍ/CE
            { "2305407", "206100" }, // ICÓ/CE
            { "2305506", "203000" }, // IGUATU/CE
            { "2305605", "177800" }, // INDEPENDÊNCIA/CE
            { "2305654", "175800" }, // IPAPORANGA/CE
            { "2305704", "206200" }, // IPAUMIRIM/CE
            { "2305803", "168400" }, // IPU/CE
            { "2305902", "175500" }, // IPUEIRAS/CE
            { "2306009", "199700" }, // IRACEMA/CE
            { "2306108", "160100" }, // IRAUÇUBA/CE
            { "2306207", "186000" }, // ITAIÇABA/CE
            { "2306256", "114900" }, // ITAITINGA/CE
            { "2306306", "" }, // ITAPAJÉ/CE
            { "2306405", "160300" }, // ITAPIPOCA/CE
            { "2306504", "190000" }, // ITAPIÚNA/CE
            { "2306553", "156600" }, // ITAREMA/CE
            { "2306603", "192600" }, // ITATIRA/CE
            { "2306702", "198600" }, // JAGUARETAMA/CE
            { "2306801", "198700" }, // JAGUARIBARA/CE
            { "2306900", "198800" }, // JAGUARIBE/CE
            { "2307007", "186100" }, // JAGUARUANA/CE
            { "2307106", "215400" }, // JARDIM/CE
            { "2307205", "211000" }, // JATI/CE
            { "2307254", "115000" }, // JIJOCA DE JERICOACOARA/CE
            { "2307304", "215500" }, // JUAZEIRO DO NORTE/CE
            { "2307403", "203100" }, // JUCÁS/CE
            { "2307502", "206300" }, // LAVRAS DA MANGABEIRA/CE
            { "2307601", "186200" }, // LIMOEIRO DO NORTE/CE
            { "2307635", "193100" }, // MADALENA/CE
            { "2307650", "181200" }, // MARACANAÚ/CE
            { "2307700", "180900" }, // MARANGUAPE/CE
            { "2307809", "156300" }, // MARCO/CE
            { "2307908", "156400" }, // MARTINÓPOLE/CE
            { "2308005", "168500" }, // MASSAPÊ/CE
            { "2308104", "211100" }, // MAURITI/CE
            { "2308203", "168600" }, // MERUOCA/CE
            { "2308302", "211200" }, // MILAGRES/CE
            { "2308351", "196400" }, // MILHÃ/CE
            { "2308377", "161100" }, // MIRAÍMA/CE
            { "2308401", "215600" }, // MISSÃO VELHA/CE
            { "2308500", "195900" }, // MOMBAÇA/CE
            { "2308609", "177900" }, // MONSENHOR TABOSA/CE
            { "2308708", "186300" }, // MORADA NOVA/CE
            { "2308807", "168700" }, // MORAÚJO/CE
            { "2308906", "158500" }, // MORRINHOS/CE
            { "2309003", "168800" }, // MUCAMBO/CE
            { "2309102", "190100" }, // MULUNGU/CE
            { "2309201", "213300" }, // NOVA OLINDA/CE
            { "2309300", "175600" }, // NOVA RUSSAS/CE
            { "2309409", "178000" }, // NOVO ORIENTE/CE
            { "2309458", "190600" }, // OCARA/CE
            { "2309508", "203200" }, // ORÓS/CE
            { "2309607", "184200" }, // PACAJUS/CE
            { "2309706", "181000" }, // PACATUBA/CE
            { "2309805", "190200" }, // PACOTI/CE
            { "2309904", "168900" }, // PACUJÁ/CE
            { "2310001", "186400" }, // PALHANO/CE
            { "2310100", "190300" }, // PALMÁCIA/CE
            { "2310209", "160400" }, // PARACURU/CE
            { "2310258", "161200" }, // PARAIPABA/CE
            { "2310308", "201100" }, // PARAMBU/CE
            { "2310407", "173100" }, // PARAMOTI/CE
            { "2310506", "189510" }, // PEDRA BRANCA/CE
            { "2310605", "211300" }, // PENAFORTE/CE
            { "2310704", "160500" }, // PENTECOSTE/CE
            { "2310803", "199800" }, // PEREIRO/CE
            { "2310852", "184500" }, // PINDORETAMA/CE
            { "2310902", "196100" }, // PIQUET CARNEIRO/CE
            { "2310951", "169300" }, // PIRES FERREIRA/CE
            { "2311009", "175700" }, // PORANGA/CE
            { "2311108", "211400" }, // PORTEIRAS/CE
            { "2311207", "213400" }, // POTENGI/CE
            { "2311231", "200000" }, // POTIRETAMA/CE
            { "2311264", "020200" }, // QUITERIANÓPOLIS/CE
            { "2311306", "192700" }, // QUIXADÁ/CE
            { "2311355", "018700" }, // QUIXELÔ/CE
            { "2311405", "192800" }, // QUIXERAMOBIM/CE
            { "2311504", "186500" }, // QUIXERÉ/CE
            { "2311603", "190400" }, // REDENÇÃO/CE
            { "2311702", "169000" }, // RERIUTABA/CE
            { "2311801", "186600" }, // RUSSAS/CE
            { "2311900", "201200" }, // SABOEIRO/CE
            { "2311959", "020300" }, // SALITRE/CE
            { "2312205", "173200" }, // SANTA QUITÉRIA/CE
            { "2312007", "158600" }, // SANTANA DO ACARAÚ/CE
            { "2312106", "213500" }, // SANTANA DO CARIRI/CE
            { "2312304", "165400" }, // SÃO BENEDITO/CE
            { "2312403", "160600" }, // SÃO GONÇALO DO AMARANTE/CE
            { "2312502", "186700" }, // SÃO JOÃO DO JAGUARIBE/CE
            { "2312601", "160700" }, // SÃO LUÍS DO CURU/CE
            { "2312700", "196200" }, // SENADOR POMPEU/CE
            { "2312809", "158700" }, // SENADOR SÁ/CE
            { "2312908", "169100" }, // SOBRAL/CE
            { "2313005", "196300" }, // SOLONÓPOLE/CE
            { "2313104", "186800" }, // TABULEIRO DO NORTE/CE
            { "2313203", "178100" }, // TAMBORIL/CE
            { "2313252", "020500" }, // TARRAFAS/CE
            { "2313302", "201300" }, // TAUÁ/CE
            { "2313351", "161300" }, // TEJUÇUOCA/CE
            { "2313401", "165500" }, // TIANGUÁ/CE
            { "2313500", "160800" }, // TRAIRI/CE
            { "2313559", "161400" }, // TURURU/CE
            { "2313609", "165600" }, // UBAJARA/CE
            { "2313708", "206400" }, // UMARI/CE
            { "2313757", "161500" }, // UMIRIM/CE
            { "2313807", "160900" }, // URUBURETAMA/CE
            { "2313906", "158800" }, // URUOCA/CE
            { "2313955", "169400" }, // VARJOTA/CE
            { "2314003", "208600" }, // VÁRZEA ALEGRE/CE
            { "2314102", "165700" }, // VIÇOSA DO CEARÁ/CE
            { "5300108", "073900" }, // BRASÍLIA/DF
            { "3200102", "573700" }, // AFONSO CLÁUDIO/ES
            { "3200169", "043200" }, // ÁGUA DOCE DO NORTE/ES
            { "3200136", "567200" }, // ÁGUIA BRANCA/ES
            { "3200201", "579800" }, // ALEGRE/ES
            { "3200300", "573800" }, // ALFREDO CHAVES/ES
            { "3200359", "567300" }, // ALTO RIO NOVO/ES
            { "3200409", "584000" }, // ANCHIETA/ES
            { "3200508", "579900" }, // APIACÁ/ES
            { "3200607", "571000" }, // ARACRUZ/ES
            { "3200706", "580000" }, // ATÍLIO VIVÁCQUA/ES
            { "3200805", "566300" }, // BAIXO GUANDU/ES
            { "3200904", "566400" }, // BARRA DE SÃO FRANCISCO/ES
            { "3201001", "566500" }, // BOA ESPERANÇA/ES
            { "3201100", "580100" }, // BOM JESUS DO NORTE/ES
            { "3201159", "176200" }, // BREJETUBA/ES
            { "3201209", "580200" }, // CACHOEIRO DE ITAPEMIRIM/ES
            { "3201308", "585900" }, // CARIACICA/ES
            { "3201407", "578100" }, // CASTELO/ES
            { "3201506", "566600" }, // COLATINA/ES
            { "3201605", "571100" }, // CONCEIÇÃO DA BARRA/ES
            { "3201704", "578200" }, // CONCEIÇÃO DO CASTELO/ES
            { "3201803", "580300" }, // DIVINO DE SÃO LOURENÇO/ES
            { "3201902", "573900" }, // DOMINGOS MARTINS/ES
            { "3202009", "580400" }, // DORES DO RIO PRETO/ES
            { "3202108", "565100" }, // ECOPORANGA/ES
            { "3202207", "571200" }, // FUNDÃO/ES
            { "3202256", "218200" }, // GOVERNADOR LINDENBERG/ES
            { "3202306", "580500" }, // GUAÇUÍ/ES
            { "3202405", "584100" }, // GUARAPARI/ES
            { "3202454", "578300" }, // IBATIBA/ES
            { "3202504", "574000" }, // IBIRAÇU/ES
            { "3202553", "579805" }, // IBITIRAMA/ES
            { "3202603", "584200" }, // ICONHA/ES
            { "3202652", "074300" }, // IRUPI/ES
            { "3202702", "574100" }, // ITAGUAÇU/ES
            { "3202801", "584300" }, // ITAPEMIRIM/ES
            { "3202900", "574200" }, // ITARANA/ES
            { "3203007", "578400" }, // IÚNA/ES
            { "3203056", "571300" }, // JAGUARÉ/ES
            { "3203106", "580600" }, // JERÔNIMO MONTEIRO/ES
            { "3203130", "574600" }, // JOÃO NEIVA/ES
            { "3203163", "574500" }, // LARANJA DA TERRA/ES
            { "3203205", "571400" }, // LINHARES/ES
            { "3203304", "566700" }, // MANTENÓPOLIS/ES
            { "3203320", "152800" }, // MARATAÍZES/ES
            { "3203346", "074200" }, // MARECHAL FLORIANO/ES
            { "3203353", "566800" }, // MARILÂNDIA/ES
            { "3203403", "580700" }, // MIMOSO DO SUL/ES
            { "3203502", "565200" }, // MONTANHA/ES
            { "3203601", "565300" }, // MUCURICI/ES
            { "3203700", "578500" }, // MUNIZ FREIRE/ES
            { "3203809", "580800" }, // MUQUI/ES
            { "3203908", "566900" }, // NOVA VENÉCIA/ES
            { "3204005", "567000" }, // PANCAS/ES
            { "3204054", "571800" }, // PEDRO CANÁRIO/ES
            { "3204104", "571500" }, // PINHEIROS/ES
            { "3204203", "584400" }, // PIÚMA/ES
            { "3204252", "152900" }, // PONTO BELO/ES
            { "3204302", "584500" }, // PRESIDENTE KENNEDY/ES
            { "3204351", "571600" }, // RIO BANANAL/ES
            { "3204401", "584600" }, // RIO NOVO DO SUL/ES
            { "3204500", "574300" }, // SANTA LEOPOLDINA/ES
            { "3204559", "043100" }, // SANTA MARIA DE JETIBÁ/ES
            { "3204609", "574400" }, // SANTA TERESA/ES
            { "3204658", "117100" }, // SÃO DOMINGOS DO NORTE/ES
            { "3204708", "567100" }, // SÃO GABRIEL DA PALHA/ES
            { "3204807", "580900" }, // SÃO JOSÉ DO CALÇADO/ES
            { "3204906", "571700" }, // SÃO MATEUS/ES
            { "3204955", "154300" }, // SÃO ROQUE DO CANAÃ/ES
            { "3205002", "586000" }, // SERRA/ES
            { "3205010", "154400" }, // SOORETAMA/ES
            { "3205036", "581000" }, // VARGEM ALTA/ES
            { "3205069", "043000" }, // VENDA NOVA DO IMIGRANTE/ES
            { "3205101", "586100" }, // VIANA/ES
            { "3205150", "117200" }, // VILA PAVÃO/ES
            { "3205176", "154500" }, // VILA VALÉRIO/ES
            { "3205200", "586200" }, // VILA VELHA/ES
            { "3205309", "586300" }, // VITÓRIA/ES
            { "5200050", "149600" }, // ABADIA DE GOIÁS/GO
            { "5200100", "018900" }, // ABADIÂNIA/GO
            { "5200134", "032100" }, // ACREÚNA/GO
            { "5200159", "029100" }, // ADELÂNDIA/GO
            { "5200175", "020000" }, // ÁGUA FRIA DE GOIÁS/GO
            { "5200209", "033700" }, // ÁGUA LIMPA/GO
            { "5200258", "149400" }, // ÁGUAS LINDAS DE GOIÁS/GO
            { "5200308", "019000" }, // ALEXÂNIA/GO
            { "5200506", "039100" }, // ALOÂNDIA/GO
            { "5200555", "012108" }, // ALTO HORIZONTE/GO
            { "5200605", "014000" }, // ALTO PARAÍSO DE GOIÁS/GO
            { "5200803", "015300" }, // ALVORADA DO NORTE/GO
            { "5200829", "151400" }, // AMARALINA/GO
            { "5200852", "023300" }, // AMERICANO DO BRASIL/GO
            { "5200902", "023400" }, // AMORINÓPOLIS/GO
            { "5201108", "023500" }, // ANÁPOLIS/GO
            { "5201207", "036300" }, // ANHANGÜERA/GO
            { "5201306", "023600" }, // ANICUNS/GO
            { "5201405", "033800" }, // APARECIDA DE GOIÂNIA/GO
            { "5201454", "022600" }, // APARECIDA DO RIO DOCE/GO
            { "5201504", "032200" }, // APORÉ/GO
            { "5201603", "023700" }, // ARAÇU/GO
            { "5201702", "020900" }, // ARAGARÇAS/GO
            { "5201801", "033900" }, // ARAGOIÂNIA/GO
            { "5202155", "016700" }, // ARAGUAPAZ/GO
            { "5202353", "021000" }, // ARENÓPOLIS/GO
            { "5202502", "016800" }, // ARUANÃ/GO
            { "5202601", "023800" }, // AURILÂNDIA/GO
            { "5202809", "023900" }, // AVELINÓPOLIS/GO
            { "5203104", "021100" }, // BALIZA/GO
            { "5203203", "024000" }, // BARRO ALTO/GO
            { "5203302", "034000" }, // BELA VISTA DE GOIÁS/GO
            { "5203401", "021200" }, // BOM JARDIM DE GOIÁS/GO
            { "5203500", "" }, // BOM JESUS DE GOIÁS/GO
            { "5203559", "038100" }, // BONFINÓPOLIS/GO
            { "5203575", "151200" }, // BONÓPOLIS/GO
            { "5203609", "024100" }, // BRAZABRANTES/GO
            { "5203807", "016900" }, // BRITÂNIA/GO
            { "5203906", "039300" }, // BURITI ALEGRE/GO
            { "5203939", "053400" }, // BURITI DE GOIÁS/GO
            { "5203962", "052600" }, // BURITINÓPOLIS/GO
            { "5204003", "019100" }, // CABECEIRAS/GO
            { "5204102", "039400" }, // CACHOEIRA ALTA/GO
            { "5204201", "024200" }, // CACHOEIRA DE GOIÁS/GO
            { "5204250", "039500" }, // CACHOEIRA DOURADA/GO
            { "5204300", "039600" }, // CAÇU/GO
            { "5204409", "021300" }, // CAIAPÔNIA/GO
            { "5204508", "034100" }, // CALDAS NOVAS/GO
            { "5204557", "053100" }, // CALDAZINHA/GO
            { "5204607", "024300" }, // CAMPESTRE DE GOIÁS/GO
            { "5204656", "011600" }, // CAMPINAÇU/GO
            { "5204706", "011700" }, // CAMPINORTE/GO
            { "5204805", "036400" }, // CAMPO ALEGRE DE GOIÁS/GO
            { "5204854", "175900" }, // CAMPO LIMPO DE GOIÁS/GO
            { "5204904", "009400" }, // CAMPOS BELOS/GO
            { "5204953", "013200" }, // CAMPOS VERDES/GO
            { "5205000", "024400" }, // CARMO DO RIO VERDE/GO
            { "5205059", "022400" }, // CASTELÂNDIA/GO
            { "5205109", "036500" }, // CATALÃO/GO
            { "5205208", "024500" }, // CATURAÍ/GO
            { "5205307", "014100" }, // CAVALCANTE/GO
            { "5205406", "024600" }, // CERES/GO
            { "5205455", "035500" }, // CEZARINA/GO
            { "5205471", "020400" }, // CHAPADÃO DO CÉU/GO
            { "5205497", "022700" }, // CIDADE OCIDENTAL/GO
            { "5205513", "022200" }, // COCALZINHO DE GOIÁS/GO
            { "5205521", "014500" }, // COLINAS DO SUL/GO
            { "5205703", "024700" }, // CÓRREGO DO OURO/GO
            { "5205802", "019200" }, // CORUMBÁ DE GOIÁS/GO
            { "5205901", "036600" }, // CORUMBAÍBA/GO
            { "5206206", "019300" }, // CRISTALINA/GO
            { "5206305", "034200" }, // CRISTIANÓPOLIS/GO
            { "5206404", "011800" }, // CRIXÁS/GO
            { "5206503", "034300" }, // CROMÍNIA/GO
            { "5206602", "036700" }, // CUMARI/GO
            { "5206701", "015400" }, // DAMIANÓPOLIS/GO
            { "5206800", "024800" }, // DAMOLÂNDIA/GO
            { "5206909", "036800" }, // DAVINÓPOLIS/GO
            { "5207105", "021400" }, // DIORAMA/GO
            { "5208301", "015600" }, // DIVINÓPOLIS DE GOIÁS/GO
            { "5207253", "021500" }, // DOVERLÂNDIA/GO
            { "5207352", "035600" }, // EDEALINA/GO
            { "5207402", "034400" }, // EDÉIA/GO
            { "5207501", "011900" }, // ESTRELA DO NORTE/GO
            { "5207535", "017600" }, // FAINA/GO
            { "5207600", "024900" }, // FAZENDA NOVA/GO
            { "5207808", "025000" }, // FIRMINÓPOLIS/GO
            { "5207907", "015500" }, // FLORES DE GOIÁS/GO
            { "5208004", "019400" }, // FORMOSA/GO
            { "5208103", "012000" }, // FORMOSO/GO
            { "5208152", "175400" }, // GAMELEIRA DE GOIÁS/GO
            { "5208400", "025100" }, // GOIANÁPOLIS/GO
            { "5208509", "036900" }, // GOIANDIRA/GO
            { "5208608", "025200" }, // GOIANÉSIA/GO
            { "5208707", "025300" }, // GOIÂNIA/GO
            { "5208806", "025400" }, // GOIANIRA/GO
            { "5208905", "017000" }, // GOIÁS/GO
            { "5209101", "039700" }, // GOIATUBA/GO
            { "5209150", "040900" }, // GOUVELÂNDIA/GO
            { "5209200", "034500" }, // GUAPÓ/GO
            { "5209291", "053000" }, // GUARAÍTA/GO
            { "5209408", "015700" }, // GUARANI DE GOIÁS/GO
            { "5209457", "013600" }, // GUARINOS/GO
            { "5209606", "025500" }, // HEITORAÍ/GO
            { "5209705", "034600" }, // HIDROLÂNDIA/GO
            { "5209804", "025600" }, // HIDROLINA/GO
            { "5209903", "015800" }, // IACIARA/GO
            { "5209937", "052900" }, // INACIOLÂNDIA/GO
            { "5209952", "032300" }, // INDIARA/GO
            { "5210000", "025700" }, // INHUMAS/GO
            { "5210109", "037000" }, // IPAMERI/GO
            { "5210158", "175300" }, // IPIRANGA DE GOIÁS/GO
            { "5210208", "025800" }, // IPORÁ/GO
            { "5210307", "025900" }, // ISRAELÂNDIA/GO
            { "5210406", "026000" }, // ITABERAÍ/GO
            { "5210562", "029200" }, // ITAGUARI/GO
            { "5210604", "026100" }, // ITAGUARU/GO
            { "5210802", "039800" }, // ITAJÁ/GO
            { "5210901", "026200" }, // ITAPACI/GO
            { "5211008", "017100" }, // ITAPIRAPUÃ/GO
            { "5211206", "026300" }, // ITAPURANGA/GO
            { "5211305", "039900" }, // ITARUMÃ/GO
            { "5211404", "026400" }, // ITAUÇU/GO
            { "5211503", "040000" }, // ITUMBIARA/GO
            { "5211602", "026500" }, // IVOLÂNDIA/GO
            { "5211701", "032400" }, // JANDAIA/GO
            { "5211800", "026600" }, // JARAGUÁ/GO
            { "5211909", "032500" }, // JATAÍ/GO
            { "5212006", "026700" }, // JAUPACI/GO
            { "5212055", "022500" }, // JESÚPOLIS/GO
            { "5212105", "040100" }, // JOVIÂNIA/GO
            { "5212204", "017200" }, // JUSSARA/GO
            { "5212303", "037100" }, // LEOPOLDO DE BULHÕES/GO
            { "5212501", "019500" }, // LUZIÂNIA/GO
            { "5212600", "034700" }, // MAIRIPOTABA/GO
            { "5212709", "015900" }, // MAMBAÍ/GO
            { "5212808", "012100" }, // MARA ROSA/GO
            { "5212907", "034800" }, // MARZAGÃO/GO
            { "5212956", "017400" }, // MATRINCHÃ/GO
            { "5213004", "040200" }, // MAURILÂNDIA/GO
            { "5213053", "020100" }, // MIMOSO DE GOIÁS/GO
            { "5213087", "012200" }, // MINAÇU/GO
            { "5213103", "021600" }, // MINEIROS/GO
            { "5213400", "026800" }, // MOIPORÁ/GO
            { "5213509", "009700" }, // MONTE ALEGRE DE GOIÁS/GO
            { "5213707", "021700" }, // MONTES CLAROS DE GOIÁS/GO
            { "5213756", "033000" }, // MONTIVIDIU/GO
            { "5213772", "053200" }, // MONTIVIDIU DO NORTE/GO
            { "5213806", "040300" }, // MORRINHOS/GO
            { "5213855", "029300" }, // MORRO AGUDO DE GOIÁS/GO
            { "5213905", "026900" }, // MOSSÂMEDES/GO
            { "5214002", "017300" }, // MOZARLÂNDIA/GO
            { "5214051", "012300" }, // MUNDO NOVO/GO
            { "5214101", "012400" }, // MUTUNÓPOLIS/GO
            { "5214408", "027000" }, // NAZÁRIO/GO
            { "5214507", "027100" }, // NERÓPOLIS/GO
            { "5214606", "014200" }, // NIQUELÂNDIA/GO
            { "5214705", "027200" }, // NOVA AMÉRICA/GO
            { "5214804", "037200" }, // NOVA AURORA/GO
            { "5214838", "012500" }, // NOVA CRIXÁS/GO
            { "5214861", "027300" }, // NOVA GLÓRIA/GO
            { "5214879", "020800" }, // NOVA IGUAÇU DE GOIÁS/GO
            { "5214903", "014300" }, // NOVA ROMA/GO
            { "5215009", "027400" }, // NOVA VENEZA/GO
            { "5215207", "027500" }, // NOVO BRASIL/GO
            { "5215231", "148500" }, // NOVO GAMA/GO
            { "5215256", "013300" }, // NOVO PLANALTO/GO
            { "5215306", "037300" }, // ORIZONA/GO
            { "5215405", "027600" }, // OURO VERDE DE GOIÁS/GO
            { "5215504", "037400" }, // OUVIDOR/GO
            { "5215603", "019600" }, // PADRE BERNARDO/GO
            { "5215652", "022100" }, // PALESTINA DE GOIÁS/GO
            { "5215702", "034900" }, // PALMEIRAS DE GOIÁS/GO
            { "5215801", "037500" }, // PALMELO/GO
            { "5215900", "032600" }, // PALMINÓPOLIS/GO
            { "5216007", "040400" }, // PANAMÁ/GO
            { "5216304", "040500" }, // PARANAIGUARA/GO
            { "5216403", "032700" }, // PARAÚNA/GO
            { "5216452", "022900" }, // PEROLÂNDIA/GO
            { "5216809", "027700" }, // PETROLINA DE GOIÁS/GO
            { "5216908", "012600" }, // PILAR DE GOIÁS/GO
            { "5217104", "035000" }, // PIRACANJUBA/GO
            { "5217203", "021800" }, // PIRANHAS/GO
            { "5217302", "019700" }, // PIRENÓPOLIS/GO
            { "5217401", "037600" }, // PIRES DO RIO/GO
            { "5217609", "" }, // PLANALTINA DE GOIÁS/GO
            { "5217708", "035100" }, // PONTALINA/GO
            { "5218003", "012700" }, // PORANGATU/GO
            { "5218052", "015603" }, // PORTEIRÃO/GO
            { "5218102", "021900" }, // PORTELÂNDIA/GO
            { "5218300", "016000" }, // POSSE/GO
            { "5218391", "022800" }, // PROFESSOR JAMIL/GO
            { "5218508", "040600" }, // QUIRINÓPOLIS/GO
            { "5218607", "027800" }, // RIALMA/GO
            { "5218706", "027900" }, // RIANÁPOLIS/GO
            { "5218789", "035700" }, // RIO QUENTE/GO
            { "5218805", "019207" }, // RIO VERDE/GO
            { "5218904", "028000" }, // RUBIATABA/GO
            { "5219001", "028100" }, // SANCLERLÂNDIA/GO
            { "5219100", "028200" }, // SANTA BÁRBARA DE GOIÁS/GO
            { "5219209", "035200" }, // SANTA CRUZ DE GOIÁS/GO
            { "5219258", "017500" }, // SANTA FÉ DE GOIÁS/GO
            { "5219308", "040700" }, // SANTA HELENA DE GOIÁS/GO
            { "5219357", "028300" }, // SANTA ISABEL/GO
            { "5219407", "022000" }, // SANTA RITA DO ARAGUAIA/GO
            { "5219456", "151300" }, // SANTA RITA DO NOVO DESTINO/GO
            { "5219506", "028400" }, // SANTA ROSA DE GOIÁS/GO
            { "5219605", "012800" }, // SANTA TEREZA DE GOIÁS/GO
            { "5219704", "012900" }, // SANTA TEREZINHA DE GOIÁS/GO
            { "5219712", "053300" }, // SANTO ANTÔNIO DA BARRA/GO
            { "5219738", "020700" }, // SANTO ANTÔNIO DE GOIÁS/GO
            { "5219753", "019900" }, // SANTO ANTÔNIO DO DESCOBERTO/GO
            { "5219803", "016100" }, // SÃO DOMINGOS/GO
            { "5219902", "028500" }, // SÃO FRANCISCO DE GOIÁS/GO
            { "5220058", "033200" }, // SÃO JOÃO DA PARAÚNA/GO
            { "5220009", "014400" }, // SÃO JOÃO D'ALIANÇA/GO
            { "5220108", "028600" }, // SÃO LUÍS DE MONTES BELOS/GO
            { "5220157", "029400" }, // SÃO LUIZ DO NORTE/GO
            { "5220207", "013000" }, // SÃO MIGUEL DO ARAGUAIA/GO
            { "5220264", "038200" }, // SÃO MIGUEL DO PASSA QUATRO/GO
            { "5220280", "029403" }, // SÃO PATRÍCIO/GO
            { "5220405", "040800" }, // SÃO SIMÃO/GO
            { "5220454", "029500" }, // SENADOR CANEDO/GO
            { "5220504", "032900" }, // SERRANÓPOLIS/GO
            { "5220603", "037700" }, // SILVÂNIA/GO
            { "5220686", "016300" }, // SIMOLÂNDIA/GO
            { "5220702", "016200" }, // SÍTIO D'ABADIA/GO
            { "5221007", "028700" }, // TAQUARAL DE GOIÁS/GO
            { "5221080", "014600" }, // TERESINA DE GOIÁS/GO
            { "5221197", "052500" }, // TEREZÓPOLIS DE GOIÁS/GO
            { "5221304", "037800" }, // TRÊS RANCHOS/GO
            { "5221403", "028800" }, // TRINDADE/GO
            { "5221452", "013500" }, // TROMBAS/GO
            { "5221502", "028900" }, // TURVÂNIA/GO
            { "5221551", "033100" }, // TURVELÂNDIA/GO
            { "5221577", "052700" }, // UIRAPURU/GO
            { "5221601", "013100" }, // URUAÇU/GO
            { "5221700", "029000" }, // URUANA/GO
            { "5221809", "037900" }, // URUTAÍ/GO
            { "5221858", "148400" }, // VALPARAÍSO DE GOIÁS/GO
            { "5221908", "035300" }, // VARJÃO/GO
            { "5222005", "038000" }, // VIANÓPOLIS/GO
            { "5222054", "035400" }, // VICENTINÓPOLIS/GO
            { "5222203", "014503" }, // VILA BOA/GO
            { "5222302", "151900" }, // VILA PROPÍCIO/GO
            { "2100055", "124800" }, // AÇAILÂNDIA/MA
            { "2100105", "135400" }, // AFONSO CUNHA/MA
            { "2100154", "154600" }, // ÁGUA DOCE DO MARANHÃO/MA
            { "2100204", "120200" }, // ALCÂNTARA/MA
            { "2100303", "133400" }, // ALDEIAS ALTAS/MA
            { "2100402", "123500" }, // ALTAMIRA DO MARANHÃO/MA
            { "2100436", "155500" }, // ALTO ALEGRE DO MARANHÃO/MA
            { "2100477", "155600" }, // ALTO ALEGRE DO PINDARÉ/MA
            { "2100501", "137300" }, // ALTO PARNAÍBA/MA
            { "2100550", "155700" }, // AMAPÁ DO MARANHÃO/MA
            { "2100600", "124900" }, // AMARANTE DO MARANHÃO/MA
            { "2100709", "120300" }, // ANAJATUBA/MA
            { "2100808", "129900" }, // ANAPURUS/MA
            { "2100832", "156700" }, // APICUM-AÇU/MA
            { "2100873", "156800" }, // ARAGUANÃ/MA
            { "2100907", "130000" }, // ARAIÓSES/MA
            { "2100956", "009000" }, // ARAME/MA
            { "2101004", "120400" }, // ARARI/MA
            { "2101103", "128700" }, // AXIXÁ/MA
            { "2101202", "131600" }, // BACABAL/MA
            { "2101251", "156900" }, // BACABEIRA/MA
            { "2101301", "120500" }, // BACURI/MA
            { "2101350", "176300" }, // BACURITUBA/MA
            { "2101400", "137400" }, // BALSAS/MA
            { "2101509", "139200" }, // BARÃO DE GRAJAÚ/MA
            { "2101608", "125700" }, // BARRA DO CORDA/MA
            { "2101707", "128800" }, // BARREIRINHAS/MA
            { "2101772", "157000" }, // BELA VISTA DO MARANHÃO/MA
            { "2101731", "157100" }, // BELÁGUA/MA
            { "2101806", "138400" }, // BENEDITO LEITE/MA
            { "2101905", "120600" }, // BEQUIMÃO/MA
            { "2101939", "157200" }, // BERNARDO DO MEARIM/MA
            { "2101970", "157300" }, // BOA VISTA DO GURUPI/MA
            { "2102002", "123600" }, // BOM JARDIM/MA
            { "2102036", "157400" }, // BOM JESUS DAS SELVAS/MA
            { "2102077", "157500" }, // BOM LUGAR/MA
            { "2102101", "130100" }, // BREJO/MA
            { "2102150", "157600" }, // BREJO DE AREIA/MA
            { "2102200", "130200" }, // BURITI/MA
            { "2102309", "136500" }, // BURITI BRAVO/MA
            { "2102325", "157700" }, // BURITICUPU/MA
            { "2102374", "157900" }, // CACHOEIRA GRANDE/MA
            { "2102408", "120700" }, // CAJAPIÓ/MA
            { "2102507", "120800" }, // CAJARI/MA
            { "2102556", "158000" }, // CAMPESTRE DO MARANHÃO/MA
            { "2102606", "119000" }, // CÂNDIDO MENDES/MA
            { "2102705", "133500" }, // CANTANHEDE/MA
            { "2102754", "158100" }, // CAPINZAL DO NORTE/MA
            { "2102804", "137500" }, // CAROLINA/MA
            { "2102903", "119100" }, // CARUTAPERA/MA
            { "2103000", "133600" }, // CAXIAS/MA
            { "2103109", "120900" }, // CEDRAL/MA
            { "2103125", "158200" }, // CENTRAL DO MARANHÃO/MA
            { "2103158", "158300" }, // CENTRO DO GUILHERME/MA
            { "2103174", "158400" }, // CENTRO NOVO DO MARANHÃO/MA
            { "2103208", "135500" }, // CHAPADINHA/MA
            { "2103257", "158900" }, // CIDELÂNDIA/MA
            { "2103307", "133700" }, // CODÓ/MA
            { "2103406", "130300" }, // COELHO NETO/MA
            { "2103505", "136600" }, // COLINAS/MA
            { "2103554", "159000" }, // CONCEIÇÃO DO LAGO-AÇU/MA
            { "2103604", "133800" }, // COROATÁ/MA
            { "2103703", "121000" }, // CURURUPU/MA
            { "2103752", "159100" }, // DAVINÓPOLIS/MA
            { "2103802", "126600" }, // DOM PEDRO/MA
            { "2103901", "130400" }, // DUQUE BACELAR/MA
            { "2104008", "131700" }, // ESPERANTINÓPOLIS/MA
            { "2104057", "137600" }, // ESTREITO/MA
            { "2104073", "159200" }, // FEIRA NOVA DO MARANHÃO/MA
            { "2104081", "159300" }, // FERNANDO FALCÃO/MA
            { "2104099", "159400" }, // FORMOSA DA SERRA NEGRA/MA
            { "2104107", "137700" }, // FORTALEZA DOS NOGUEIRAS/MA
            { "2104206", "136700" }, // FORTUNA/MA
            { "2104305", "119200" }, // GODOFREDO VIANA/MA
            { "2104404", "126700" }, // GONÇALVES DIAS/MA
            { "2104503", "126800" }, // GOVERNADOR ARCHER/MA
            { "2104552", "" }, // GOVERNADOR EDSON LOBÃO/MA
            { "2104602", "126900" }, // GOVERNADOR EUGÊNIO BARROS/MA
            { "2104628", "159600" }, // GOVERNADOR LUIZ ROCHA/MA
            { "2104651", "159700" }, // GOVERNADOR NEWTON BELLO/MA
            { "2104677", "159800" }, // GOVERNADOR NUNES FREIRE/MA
            { "2104701", "127000" }, // GRAÇA ARANHA/MA
            { "2104800", "125800" }, // GRAJAÚ/MA
            { "2104909", "121100" }, // GUIMARÃES/MA
            { "2105005", "128900" }, // HUMBERTO DE CAMPOS/MA
            { "2105104", "129000" }, // ICATU/MA
            { "2105153", "159900" }, // IGARAPÉ DO MEIO/MA
            { "2105203", "131800" }, // IGARAPÉ GRANDE/MA
            { "2105302", "125000" }, // IMPERATRIZ/MA
            { "2105351", "161600" }, // ITAIPAVA DO GRAJAÚ/MA
            { "2105401", "133900" }, // ITAPECURU MIRIM/MA
            { "2105427", "161700" }, // ITINGA DO MARANHÃO/MA
            { "2105450", "161800" }, // JATOBÁ/MA
            { "2105476", "161900" }, // JENIPAPO DOS VIEIRAS/MA
            { "2105500", "125100" }, // JOÃO LISBOA/MA
            { "2105609", "131900" }, // JOSELÂNDIA/MA
            { "2105658", "162000" }, // JUNCO DO MARANHÃO/MA
            { "2105708", "123700" }, // LAGO DA PEDRA/MA
            { "2105807", "132000" }, // LAGO DO JUNCO/MA
            { "2105948", "162100" }, // LAGO DOS RODRIGUES/MA
            { "2105906", "132100" }, // LAGO VERDE/MA
            { "2105922", "162200" }, // LAGOA DO MATO/MA
            { "2105963", "162300" }, // LAGOA GRANDE DO MARANHÃO/MA
            { "2105989", "162400" }, // LAJEADO NOVO/MA
            { "2106003", "132200" }, // LIMA CAMPOS/MA
            { "2106102", "138500" }, // LORETO/MA
            { "2106201", "119300" }, // LUÍS DOMINGUES/MA
            { "2106300", "130500" }, // MAGALHÃES DE ALMEIDA/MA
            { "2106326", "162500" }, // MARACAÇUMÉ/MA
            { "2106359", "162600" }, // MARAJÁ DO SENA/MA
            { "2106375", "162700" }, // MARANHÃOZINHO/MA
            { "2106409", "135600" }, // MATA ROMA/MA
            { "2106508", "121200" }, // MATINHA/MA
            { "2106607", "134000" }, // MATÕES/MA
            { "2106631", "162800" }, // MATÕES DO NORTE/MA
            { "2106672", "162900" }, // MILAGRES DO MARANHÃO/MA
            { "2106706", "139300" }, // MIRADOR/MA
            { "2106755", "008900" }, // MIRANDA DO NORTE/MA
            { "2106805", "121300" }, // MIRINZAL/MA
            { "2106904", "123800" }, // MONÇÃO/MA
            { "2107001", "125200" }, // MONTES ALTOS/MA
            { "2107100", "129100" }, // MORROS/MA
            { "2107209", "135700" }, // NINA RODRIGUES/MA
            { "2107258", "163000" }, // NOVA COLINAS/MA
            { "2107308", "139400" }, // NOVA IORQUE/MA
            { "2107357", "163100" }, // NOVA OLINDA DO MARANHÃO/MA
            { "2107407", "132300" }, // OLHO D'ÁGUA DAS CUNHÃS/MA
            { "2107456", "163200" }, // OLINDA NOVA DO MARANHÃO/MA
            { "2107506", "127800" }, // PAÇO DO LUMIAR/MA
            { "2107605", "121400" }, // PALMEIRÂNDIA/MA
            { "2107704", "139500" }, // PARAIBANO/MA
            { "2107803", "134100" }, // PARNARAMA/MA
            { "2107902", "136800" }, // PASSAGEM FRANCA/MA
            { "2108009", "139600" }, // PASTOS BONS/MA
            { "2108058", "176400" }, // PAULINO NEVES/MA
            { "2108108", "123900" }, // PAULO RAMOS/MA
            { "2108207", "132400" }, // PEDREIRAS/MA
            { "2108256", "163300" }, // PEDRO DO ROSÁRIO/MA
            { "2108306", "121500" }, // PENALVA/MA
            { "2108405", "121600" }, // PERI MIRIM/MA
            { "2108454", "176500" }, // PERITORÓ/MA
            { "2108504", "124000" }, // PINDARÉ MIRIM/MA
            { "2108603", "121700" }, // PINHEIRO/MA
            { "2108702", "132500" }, // PIO XII/MA
            { "2108801", "134200" }, // PIRAPEMAS/MA
            { "2108900", "132600" }, // POÇÃO DE PEDRAS/MA
            { "2109007", "125300" }, // PORTO FRANCO/MA
            { "2109056", "163400" }, // PORTO RICO DO MARANHÃO/MA
            { "2109106", "127100" }, // PRESIDENTE DUTRA/MA
            { "2109205", "129200" }, // PRESIDENTE JUSCELINO/MA
            { "2109239", "163500" }, // PRESIDENTE MÉDICI/MA
            { "2109270", "163600" }, // PRESIDENTE SARNEY/MA
            { "2109304", "135800" }, // PRESIDENTE VARGAS/MA
            { "2109403", "129300" }, // PRIMEIRA CRUZ/MA
            { "2109452", "163700" }, // RAPOSA/MA
            { "2109502", "137800" }, // RIACHÃO/MA
            { "2109551", "163800" }, // RIBAMAR FIQUENE/MA
            { "2109601", "127900" }, // ROSÁRIO/MA
            { "2109700", "138600" }, // SAMBAÍBA/MA
            { "2109759", "163900" }, // SANTA FILOMENA DO MARANHÃO/MA
            { "2109809", "121800" }, // SANTA HELENA/MA
            { "2109908", "124100" }, // SANTA INÊS/MA
            { "2110005", "124200" }, // SANTA LUZIA/MA
            { "2110039", "010400" }, // SANTA LUZIA DO PARUÁ/MA
            { "2110104", "130600" }, // SANTA QUITÉRIA DO MARANHÃO/MA
            { "2110203", "134300" }, // SANTA RITA/MA
            { "2110237", "164000" }, // SANTANA DO MARANHÃO/MA
            { "2110278", "129301" }, // SANTO AMARO/MA
            { "2110302", "132700" }, // SANTO ANTÔNIO DOS LOPES/MA
            { "2110401", "135900" }, // SÃO BENEDITO DO RIO PRETO/MA
            { "2110500", "121900" }, // SÃO BENTO/MA
            { "2110609", "130700" }, // SÃO BERNARDO/MA
            { "2110658", "164200" }, // SÃO DOMINGOS DO AZEITÃO/MA
            { "2110708", "127200" }, // SÃO DOMINGOS DO MARANHÃO/MA
            { "2110807", "138700" }, // SÃO FÉLIX DE BALSAS/MA
            { "2110856", "191000" }, // SÃO FRANCISCO DO BREJÃO/MA
            { "2110906", "139700" }, // SÃO FRANCISCO DO MARANHÃO/MA
            { "2111003", "122000" }, // SÃO JOÃO BATISTA/MA
            { "2111029", "164300" }, // SÃO JOÃO DO CARÚ/MA
            { "2111052", "164400" }, // SÃO JOÃO DO PARAÍSO/MA
            { "2111078", "164500" }, // SÃO JOÃO DO SÓTER/MA
            { "2111102", "139800" }, // SÃO JOÃO DOS PATOS/MA
            { "2111201", "128000" }, // SÃO JOSÉ DE RIBAMAR/MA
            { "2111250", "191100" }, // SÃO JOSÉ DOS BASÍLIOS/MA
            { "2111300", "128100" }, // SÃO LUÍS/MA
            { "2111409", "132800" }, // SÃO LUÍS GONZAGA DO MARANHÃO/MA
            { "2111508", "132900" }, // SÃO MATEUS DO MARANHÃO/MA
            { "2111532", "164600" }, // SÃO PEDRO DA ÁGUA BRANCA/MA
            { "2111573", "164700" }, // SÃO PEDRO DOS CRENTES/MA
            { "2111607", "138800" }, // SÃO RAIMUNDO DAS MANGABEIRAS/MA
            { "2111631", "191200" }, // SÃO RAIMUNDO DO DOCA BEZERRA/MA
            { "2111672", "164800" }, // SÃO ROBERTO/MA
            { "2111706", "122100" }, // SÃO VICENTE FERRER/MA
            { "2111722", "164900" }, // SATUBINHA/MA
            { "2111748", "165000" }, // SENADOR ALEXANDRE COSTA/MA
            { "2111763", "" }, // SENADOR LA ROQUE/MA
            { "2111789", "166000" }, // SERRANO DO MARANHÃO/MA
            { "2111805", "125900" }, // SÍTIO NOVO/MA
            { "2111904", "139900" }, // SUCUPIRA DO NORTE/MA
            { "2111953", "166100" }, // SUCUPIRA DO RIACHÃO/MA
            { "2112001", "137900" }, // TASSO FRAGOSO/MA
            { "2112100", "134400" }, // TIMBIRAS/MA
            { "2112209", "134500" }, // TIMON/MA
            { "2112233", "166200" }, // TRIZIDELA DO VALE/MA
            { "2112274", "166300" }, // TUFILÂNDIA/MA
            { "2112308", "127300" }, // TUNTUM/MA
            { "2112407", "119400" }, // TURIAÇU/MA
            { "2112456", "166400" }, // TURILÂNDIA/MA
            { "2112506", "130800" }, // TUTÓIA/MA
            { "2112605", "136000" }, // URBANO SANTOS/MA
            { "2112704", "136100" }, // VARGEM GRANDE/MA
            { "2112803", "122200" }, // VIANA/MA
            { "2112852", "166500" }, // VILA NOVA DOS MARTÍRIOS/MA
            { "2112902", "122300" }, // VITÓRIA DO MEARIM/MA
            { "2113009", "124300" }, // VITORINO FREIRE/MA
            { "2114007", "010900" }, // ZÉ DOCA/MA
            { "3100104", "445400" }, // ABADIA DOS DOURADOS/MG
            { "3100203", "450500" }, // ABAETÉ/MG
            { "3100302", "478400" }, // ABRE CAMPO/MG
            { "3100401", "478500" }, // ACAIACA/MG
            { "3100500", "458800" }, // AÇUCENA/MG
            { "3100609", "458900" }, // ÁGUA BOA/MG
            { "3100708", "457400" }, // ÁGUA COMPRIDA/MG
            { "3100807", "509700" }, // AGUANIL/MG
            { "3100906", "440800" }, // ÁGUAS FORMOSAS/MG
            { "3101003", "419700" }, // ÁGUAS VERMELHAS/MG
            { "3101102", "473200" }, // AIMORÉS/MG
            { "3101201", "524600" }, // AIURUOCA/MG
            { "3101300", "524700" }, // ALAGOA/MG
            { "3101409", "518400" }, // ALBERTINA/MG
            { "3101508", "495500" }, // ALÉM PARAÍBA/MG
            { "3101607", "505200" }, // ALFENAS/MG
            { "3101631", "070800" }, // ALFREDO VASCONCELOS/MG
            { "3101706", "431400" }, // ALMENARA/MG
            { "3101805", "464600" }, // ALPERCATA/MG
            { "3101904", "505300" }, // ALPINÓPOLIS/MG
            { "3102001", "505400" }, // ALTEROSA/MG
            { "3153509", "483400" }, // ALTO JEQUITIBÁ/MG
            { "3102100", "485600" }, // ALTO RIO DOCE/MG
            { "3102209", "473300" }, // ALVARENGA/MG
            { "3102308", "543700" }, // ALVINÓPOLIS/MG
            { "3102407", "459000" }, // ALVORADA DE MINAS/MG
            { "3102506", "" }, // AMPARO DA SERRA/MG
            { "3102605", "516500" }, // ANDRADAS/MG
            { "3102803", "524800" }, // ANDRELÂNDIA/MG
            { "3102852", "167000" }, // ANGELÂNDIA/MG
            { "3102902", "550400" }, // ANTÔNIO CARLOS/MG
            { "3103009", "535100" }, // ANTÔNIO DIAS/MG
            { "3103108", "489200" }, // ANTÔNIO PRADO DE MINAS/MG
            { "3103207", "532300" }, // ARAÇAÍ/MG
            { "3103306", "555000" }, // ARACITABA/MG
            { "3103405", "429100" }, // ARAÇUAÍ/MG
            { "3103504", "452600" }, // ARAGUARI/MG
            { "3103603", "524900" }, // ARANTINA/MG
            { "3103702", "485700" }, // ARAPONGA/MG
            { "3103751", "072800" }, // ARAPORÃ/MG
            { "3103801", "447900" }, // ARAPUÁ/MG
            { "3103900", "500900" }, // ARAÚJOS/MG
            { "3104007", "499000" }, // ARAXÁ/MG
            { "3104106", "513500" }, // ARCEBURGO/MG
            { "3104205", "501000" }, // ARCOS/MG
            { "3104304", "505500" }, // AREADO/MG
            { "3104403", "495600" }, // ARGIRITA/MG
            { "3104452", "435001" }, // ARICANDUVA/MG
            { "3104502", "416200" }, // ARINOS/MG
            { "3104601", "492800" }, // ASTOLFO DUTRA/MG
            { "3104700", "440900" }, // ATALÉIA/MG
            { "3104809", "442900" }, // AUGUSTO DE LIMA/MG
            { "3104908", "525000" }, // BAEPENDI/MG
            { "3105004", "532400" }, // BALDIM/MG
            { "3105103", "501100" }, // BAMBUÍ/MG
            { "3105202", "431500" }, // BANDEIRA/MG
            { "3105301", "516600" }, // BANDEIRA DO SUL/MG
            { "3105400", "535200" }, // BARÃO DE COCAIS/MG
            { "3105509", "489300" }, // BARÃO DE MONTE ALTO/MG
            { "3105608", "550500" }, // BARBACENA/MG
            { "3105707", "478700" }, // BARRA LONGA/MG
            { "3105905", "550600" }, // BARROSO/MG
            { "3106002", "535300" }, // BELA VISTA DE MINAS/MG
            { "3106101", "555100" }, // BELMIRO BRAGA/MG
            { "3106200", "560600" }, // BELO HORIZONTE/MG
            { "3106309", "438807" }, // BELO ORIENTE/MG
            { "3106408", "543800" }, // BELO VALE/MG
            { "3106507", "433900" }, // BERILO/MG
            { "3106655", "420201" }, // BERIZAL/MG
            { "3106606", "441000" }, // BERTÓPOLIS/MG
            { "3106705", "560700" }, // BETIM/MG
            { "3106804", "555200" }, // BIAS FORTES/MG
            { "3106903", "555300" }, // BICAS/MG
            { "3107000", "450600" }, // BIQUINHAS/MG
            { "3107109", "505600" }, // BOA ESPERANÇA/MG
            { "3107208", "525100" }, // BOCAINA DE MINAS/MG
            { "3107307", "421700" }, // BOCAIÚVA/MG
            { "3107406", "501200" }, // BOM DESPACHO/MG
            { "3107505", "525200" }, // BOM JARDIM DE MINAS/MG
            { "3107604", "513600" }, // BOM JESUS DA PENHA/MG
            { "3107703", "535400" }, // BOM JESUS DO AMPARO/MG
            { "3107802", "469400" }, // BOM JESUS DO GALHO/MG
            { "3107901", "518500" }, // BOM REPOUSO/MG
            { "3108008", "509800" }, // BOM SUCESSO/MG
            { "3108107", "543900" }, // BONFIM/MG
            { "3108206", "416300" }, // BONFINÓPOLIS DE MINAS/MG
            { "3108255", "167100" }, // BONITO DE MINAS/MG
            { "3108305", "518600" }, // BORDA DA MATA/MG
            { "3108404", "516700" }, // BOTELHOS/MG
            { "3108503", "428000" }, // BOTUMIRIM/MG
            { "3108701", "485800" }, // BRÁS PIRES/MG
            { "3108552", "152000" }, // BRASILÂNDIA DE MINAS/MG
            { "3108602", "421800" }, // BRASÍLIA DE MINAS/MG
            { "3108909", "528000" }, // BRASÓPOLIS/MG
            { "3108800", "459200" }, // BRAÚNAS/MG
            { "3109006", "544000" }, // BRUMADINHO/MG
            { "3109105", "528100" }, // BUENO BRANDÃO/MG
            { "3109204", "443000" }, // BUENÓPOLIS/MG
            { "3109253", "470001" }, // BUGRE/MG
            { "3109303", "416400" }, // BURITIS/MG
            { "3109402", "418800" }, // BURITIZEIRO/MG
            { "3109451", "417101" }, // CABECEIRA GRANDE/MG
            { "3109501", "513700" }, // CABO VERDE/MG
            { "3109600", "532500" }, // CACHOEIRA DA PRATA/MG
            { "3109709", "518700" }, // CACHOEIRA DE MINAS/MG
            { "3102704", "429000" }, // CACHOEIRA DE PAJEÚ/MG
            { "3109808", "452700" }, // CACHOEIRA DOURADA/MG
            { "3109907", "532600" }, // CAETANÓPOLIS/MG
            { "3110004", "556004" }, // CAETÉ/MG
            { "3110103", "482400" }, // CAIANA/MG
            { "3110202", "485900" }, // CAJURI/MG
            { "3110301", "516800" }, // CALDAS/MG
            { "3110400", "509900" }, // CAMACHO/MG
            { "3110509", "528200" }, // CAMANDUCAIA/MG
            { "3110608", "528300" }, // CAMBUÍ/MG
            { "3110707", "518800" }, // CAMBUQUIRA/MG
            { "3110806", "464700" }, // CAMPANÁRIO/MG
            { "3110905", "518900" }, // CAMPANHA/MG
            { "3111002", "516900" }, // CAMPESTRE/MG
            { "3111101", "455100" }, // CAMPINA VERDE/MG
            { "3111150", "421802" }, // CAMPO AZUL/MG
            { "3111200", "510000" }, // CAMPO BELO/MG
            { "3111309", "505700" }, // CAMPO DO MEIO/MG
            { "3111408", "457500" }, // CAMPO FLORIDO/MG
            { "3111507", "499100" }, // CAMPOS ALTOS/MG
            { "3111606", "505800" }, // CAMPOS GERAIS/MG
            { "3111903", "510100" }, // CANA VERDE/MG
            { "3111705", "486000" }, // CANAÃ/MG
            { "3111804", "452800" }, // CANÁPOLIS/MG
            { "3112000", "510200" }, // CANDEIAS/MG
            { "3112059", "460401" }, // CANTAGALO/MG
            { "3112109", "482500" }, // CAPARAÓ/MG
            { "3112208", "550700" }, // CAPELA NOVA/MG
            { "3112307", "434000" }, // CAPELINHA/MG
            { "3112406", "513800" }, // CAPETINGA/MG
            { "3112505", "560900" }, // CAPIM BRANCO/MG
            { "3112604", "452900" }, // CAPINÓPOLIS/MG
            { "3112653", "069900" }, // CAPITÃO ANDRADE/MG
            { "3112703", "421900" }, // CAPITÃO ENÉAS/MG
            { "3112802", "505900" }, // CAPITÓLIO/MG
            { "3112901", "482600" }, // CAPUTIRA/MG
            { "3113008", "429200" }, // CARAÍ/MG
            { "3113107", "550800" }, // CARANAÍBA/MG
            { "3113206", "550900" }, // CARANDAÍ/MG
            { "3113305", "489400" }, // CARANGOLA/MG
            { "3113404", "469500" }, // CARATINGA/MG
            { "3113503", "434100" }, // CARBONITA/MG
            { "3113602", "519000" }, // CAREAÇU/MG
            { "3113701", "441100" }, // CARLOS CHAGAS/MG
            { "3113800", "459300" }, // CARMÉSIA/MG
            { "3113909", "519100" }, // CARMO DA CACHOEIRA/MG
            { "3114006", "510300" }, // CARMO DA MATA/MG
            { "3114105", "519200" }, // CARMO DE MINAS/MG
            { "3114204", "541400" }, // CARMO DO CAJURU/MG
            { "3114303", "448000" }, // CARMO DO PARANAÍBA/MG
            { "3114402", "506000" }, // CARMO DO RIO CLARO/MG
            { "3114501", "510400" }, // CARMÓPOLIS DE MINAS/MG
            { "3114550", "071000" }, // CARNEIRINHO/MG
            { "3114600", "525300" }, // CARRANCAS/MG
            { "3114709", "519300" }, // CARVALHÓPOLIS/MG
            { "3114808", "525400" }, // CARVALHOS/MG
            { "3114907", "544100" }, // CASA GRANDE/MG
            { "3115003", "445500" }, // CASCALHO RICO/MG
            { "3115102", "506100" }, // CÁSSIA/MG
            { "3115300", "495700" }, // CATAGUASES/MG
            { "3115359", "537003" }, // CATAS ALTAS/MG
            { "3115409", "544200" }, // CATAS ALTAS DA NORUEGA/MG
            { "3115458", "115900" }, // CATUJI/MG
            { "3115474", "167200" }, // CATUTI/MG
            { "3115508", "519400" }, // CAXAMBU/MG
            { "3115607", "450700" }, // CEDRO DO ABAETÉ/MG
            { "3115706", "468100" }, // CENTRAL DE MINAS/MG
            { "3115805", "453000" }, // CENTRALINA/MG
            { "3115904", "555400" }, // CHÁCARA/MG
            { "3116001", "482700" }, // CHALÉ/MG
            { "3116100", "434200" }, // CHAPADA DO NORTE/MG
            { "3116159", "167300" }, // CHAPADA GAÚCHA/MG
            { "3116209", "555500" }, // CHIADOR/MG
            { "3116308", "486100" }, // CIPOTÂNEA/MG
            { "3116407", "513900" }, // CLARAVAL/MG
            { "3116506", "422000" }, // CLARO DOS POÇÕES/MG
            { "3116605", "510500" }, // CLÁUDIO/MG
            { "3116704", "486200" }, // COIMBRA/MG
            { "3116803", "459400" }, // COLUNA/MG
            { "3116902", "455200" }, // COMENDADOR GOMES/MG
            { "3117009", "429300" }, // COMERCINHO/MG
            { "3117108", "506200" }, // CONCEIÇÃO DA APARECIDA/MG
            { "3115201", "551000" }, // CONCEIÇÃO DA BARRA DE MINAS/MG
            { "3117306", "457600" }, // CONCEIÇÃO DAS ALAGOAS/MG
            { "3117207", "519500" }, // CONCEIÇÃO DAS PEDRAS/MG
            { "3117405", "473400" }, // CONCEIÇÃO DE IPANEMA/MG
            { "3117504", "535500" }, // CONCEIÇÃO DO MATO DENTRO/MG
            { "3117603", "501300" }, // CONCEIÇÃO DO PARÁ/MG
            { "3117702", "519600" }, // CONCEIÇÃO DO RIO VERDE/MG
            { "3117801", "519700" }, // CONCEIÇÃO DOS OUROS/MG
            { "3117836", "411503" }, // CÔNEGO MARINHO/MG
            { "3117876", "176600" }, // CONFINS/MG
            { "3117900", "519800" }, // CONGONHAL/MG
            { "3118007", "544300" }, // CONGONHAS/MG
            { "3118106", "535600" }, // CONGONHAS DO NORTE/MG
            { "3118205", "457700" }, // CONQUISTA/MG
            { "3118304", "544400" }, // CONSELHEIRO LAFAIETE/MG
            { "3118403", "473500" }, // CONSELHEIRO PENA/MG
            { "3118502", "528400" }, // CONSOLAÇÃO/MG
            { "3118601", "561000" }, // CONTAGEM/MG
            { "3118700", "506300" }, // COQUEIRAL/MG
            { "3118809", "422100" }, // CORAÇÃO DE JESUS/MG
            { "3118908", "532700" }, // CORDISBURGO/MG
            { "3119005", "519900" }, // CORDISLÂNDIA/MG
            { "3119104", "443100" }, // CORINTO/MG
            { "3119203", "464800" }, // COROACI/MG
            { "3119302", "445600" }, // COROMANDEL/MG
            { "3119401", "535700" }, // CORONEL FABRICIANO/MG
            { "3119500", "429400" }, // CORONEL MURTA/MG
            { "3119609", "555600" }, // CORONEL PACHECO/MG
            { "3119708", "551100" }, // CORONEL XAVIER CHAVES/MG
            { "3119807", "501400" }, // CÓRREGO DANTA/MG
            { "3119906", "528500" }, // CÓRREGO DO BOM JESUS/MG
            { "3119955", "510703" }, // CÓRREGO FUNDO/MG
            { "3120003", "469600" }, // CÓRREGO NOVO/MG
            { "3120102", "434300" }, // COUTO DE MAGALHÃES DE MINAS/MG
            { "3120151", "440801" }, // CRISÓLITA/MG
            { "3120201", "510600" }, // CRISTAIS/MG
            { "3120300", "428100" }, // CRISTÁLIA/MG
            { "3120409", "544500" }, // CRISTIANO OTONI/MG
            { "3120508", "528600" }, // CRISTINA/MG
            { "3120607", "544600" }, // CRUCILÂNDIA/MG
            { "3120706", "445700" }, // CRUZEIRO DA FORTALEZA/MG
            { "3120805", "525500" }, // CRUZÍLIA/MG
            { "3120839", "473505" }, // CUPARAQUE/MG
            { "3120870", "419701" }, // CURRAL DE DENTRO/MG
            { "3120904", "443200" }, // CURVELO/MG
            { "3121001", "434400" }, // DATAS/MG
            { "3121100", "528700" }, // DELFIM MOREIRA/MG
            { "3121209", "506400" }, // DELFINÓPOLIS/MG
            { "3121258", "167400" }, // DELTA/MG
            { "3121308", "555700" }, // DESCOBERTO/MG
            { "3121407", "551200" }, // DESTERRO DE ENTRE RIOS/MG
            { "3121506", "551300" }, // DESTERRO DO MELO/MG
            { "3121605", "434500" }, // DIAMANTINA/MG
            { "3121704", "478800" }, // DIOGO DE VASCONCELOS/MG
            { "3121803", "535800" }, // DIONÍSIO/MG
            { "3121902", "492900" }, // DIVINÉSIA/MG
            { "3122009", "482800" }, // DIVINO/MG
            { "3122108", "473600" }, // DIVINO DAS LARANJEIRAS/MG
            { "3122207", "459500" }, // DIVINOLÂNDIA DE MINAS/MG
            { "3122306", "541500" }, // DIVINÓPOLIS/MG
            { "3122355", "419702" }, // DIVISA ALEGRE/MG
            { "3122405", "506500" }, // DIVISA NOVA/MG
            { "3122454", "070100" }, // DIVISÓPOLIS/MG
            { "3122470", "167500" }, // DOM BOSCO/MG
            { "3122504", "469700" }, // DOM CAVATI/MG
            { "3122603", "459600" }, // DOM JOAQUIM/MG
            { "3122702", "478900" }, // DOM SILVÉRIO/MG
            { "3122801", "528800" }, // DOM VIÇOSO/MG
            { "3122900", "495800" }, // DONA EUZÉBIA/MG
            { "3123007", "551400" }, // DORES DE CAMPOS/MG
            { "3123106", "459700" }, // DORES DE GUANHÃES/MG
            { "3123205", "501500" }, // DORES DO INDAIÁ/MG
            { "3123304", "486300" }, // DORES DO TURVO/MG
            { "3123403", "501600" }, // DORESÓPOLIS/MG
            { "3123502", "445800" }, // DOURADOQUARA/MG
            { "3123528", "070600" }, // DURANDÉ/MG
            { "3123601", "506600" }, // ELÓI MENDES/MG
            { "3123700", "469800" }, // ENGENHEIRO CALDAS/MG
            { "3123809", "422200" }, // ENGENHEIRO NAVARRO/MG
            { "3123858", "116000" }, // ENTRE FOLHAS/MG
            { "3123908", "551500" }, // ENTRE RIOS DE MINAS/MG
            { "3124005", "486400" }, // ERVÁLIA/MG
            { "3124104", "561100" }, // ESMERALDAS/MG
            { "3124203", "482900" }, // ESPERA FELIZ/MG
            { "3124302", "414400" }, // ESPINOSA/MG
            { "3124401", "520000" }, // ESPÍRITO SANTO DO DOURADO/MG
            { "3124500", "520100" }, // ESTIVA/MG
            { "3124609", "495900" }, // ESTRELA DALVA/MG
            { "3124708", "501700" }, // ESTRELA DO INDAIÁ/MG
            { "3124807", "445900" }, // ESTRELA DO SUL/MG
            { "3124906", "489500" }, // EUGENÓPOLIS/MG
            { "3125002", "555800" }, // EWBANK DA CÂMARA/MG
            { "3125101", "528900" }, // EXTREMA/MG
            { "3125200", "506700" }, // FAMA/MG
            { "3125309", "489600" }, // FARIA LEMOS/MG
            { "3125408", "434600" }, // FELÍCIO DOS SANTOS/MG
            { "3125606", "431600" }, // FELISBURGO/MG
            { "3125705", "450800" }, // FELIXLÂNDIA/MG
            { "3125804", "469900" }, // FERNANDES TOURINHO/MG
            { "3125903", "535900" }, // FERROS/MG
            { "3125952", "070900" }, // FERVEDOURO/MG
            { "3126000", "541600" }, // FLORESTAL/MG
            { "3126109", "510700" }, // FORMIGA/MG
            { "3126208", "416500" }, // FORMOSO/MG
            { "3126307", "514000" }, // FORTALEZA DE MINAS/MG
            { "3126406", "532800" }, // FORTUNA DE MINAS/MG
            { "3126505", "434800" }, // FRANCISCO BADARÓ/MG
            { "3126604", "422300" }, // FRANCISCO DUMONT/MG
            { "3126703", "422400" }, // FRANCISCO SÁ/MG
            { "3126752", "438502" }, // FRANCISCÓPOLIS/MG
            { "3126802", "438200" }, // FREI GASPAR/MG
            { "3126901", "464900" }, // FREI INOCÊNCIO/MG
            { "3126950", "461001" }, // FREI LAGONEGRO/MG
            { "3127008", "455300" }, // FRONTEIRA/MG
            { "3127057", "441200" }, // FRONTEIRA DOS VALES/MG
            { "3127073", "167600" }, // FRUTA DE LEITE/MG
            { "3127107", "455400" }, // FRUTAL/MG
            { "3127206", "532900" }, // FUNILÂNDIA/MG
            { "3127305", "473700" }, // GALILÉIA/MG
            { "3127339", "414601" }, // GAMELEIRAS/MG
            { "3127354", "422801" }, // GLAUCILÂNDIA/MG
            { "3127370", "473507" }, // GOIABEIRA/MG
            { "3127388", "493501" }, // GOIANÁ/MG
            { "3127404", "529000" }, // GONÇALVES/MG
            { "3127503", "459800" }, // GONZAGA/MG
            { "3127602", "434900" }, // GOUVEA/MG
            { "3127701", "465000" }, // GOVERNADOR VALADARES/MG
            { "3127800", "428200" }, // GRÃO MOGOL/MG
            { "3127909", "446000" }, // GRUPIARA/MG
            { "3128006", "459900" }, // GUANHÃES/MG
            { "3128105", "506800" }, // GUAPÉ/MG
            { "3128204", "486500" }, // GUARACIABA/MG
            { "3128253", "421701" }, // GUARACIAMA/MG
            { "3128303", "514100" }, // GUARANÉSIA/MG
            { "3128402", "493000" }, // GUARANI/MG
            { "3128501", "555900" }, // GUARARÁ/MG
            { "3128600", "416600" }, // GUARDA-MOR/MG
            { "3128709", "514200" }, // GUAXUPÉ/MG
            { "3128808", "493100" }, // GUIDOVAL/MG
            { "3128907", "448100" }, // GUIMARÂNIA/MG
            { "3129004", "493200" }, // GUIRICEMA/MG
            { "3129103", "453100" }, // GURINHATÃ/MG
            { "3129202", "520200" }, // HELIODORA/MG
            { "3129301", "470000" }, // IAPU/MG
            { "3129400", "551600" }, // IBERTIOGA/MG
            { "3129509", "499200" }, // IBIÁ/MG
            { "3129608", "422500" }, // IBIAÍ/MG
            { "3129657", "423403" }, // IBIRACATU/MG
            { "3129707", "514300" }, // IBIRACI/MG
            { "3129806", "561200" }, // IBIRITÉ/MG
            { "3129905", "517000" }, // IBITIÚRA DE MINAS/MG
            { "3130002", "510800" }, // IBITURUNA/MG
            { "3130051", "116100" }, // ICARAÍ DE MINAS/MG
            { "3130101", "541700" }, // IGARAPÉ/MG
            { "3130200", "541800" }, // IGARATINGA/MG
            { "3130309", "501800" }, // IGUATAMA/MG
            { "3130408", "525600" }, // IJACI/MG
            { "3130507", "506900" }, // ILICÍNEA/MG
            { "3130556", "167700" }, // IMBÉ DE MINAS/MG
            { "3130606", "520300" }, // INCONFIDENTES/MG
            { "3130655", "419801" }, // INDAIABIRA/MG
            { "3130705", "446100" }, // INDIANÓPOLIS/MG
            { "3130804", "525700" }, // INGAÍ/MG
            { "3130903", "470100" }, // INHAPIM/MG
            { "3131000", "533000" }, // INHAÚMA/MG
            { "3131109", "443300" }, // INIMUTABA/MG
            { "3131158", "116200" }, // IPABA/MG
            { "3131208", "473800" }, // IPANEMA/MG
            { "3131307", "536000" }, // IPATINGA/MG
            { "3131406", "453200" }, // IPIAÇU/MG
            { "3131505", "517100" }, // IPUIÚNA/MG
            { "3131604", "499300" }, // IRAÍ DE MINAS/MG
            { "3131703", "536100" }, // ITABIRA/MG
            { "3131802", "" }, // ITABIRINHA/MG
            { "3131901", "544700" }, // ITABIRITO/MG
            { "3132008", "428300" }, // ITACAMBIRA/MG
            { "3132107", "411400" }, // ITACARAMBI/MG
            { "3132206", "510900" }, // ITAGUARA/MG
            { "3132305", "438300" }, // ITAIPÉ/MG
            { "3132404", "529100" }, // ITAJUBÁ/MG
            { "3132503", "435000" }, // ITAMARANDIBA/MG
            { "3132602", "496000" }, // ITAMARATI DE MINAS/MG
            { "3132701", "465100" }, // ITAMBACURI/MG
            { "3132800", "536200" }, // ITAMBÉ DO MATO DENTRO/MG
            { "3132909", "514400" }, // ITAMOGI/MG
            { "3133006", "529200" }, // ITAMONTE/MG
            { "3133105", "529300" }, // ITANHANDU/MG
            { "3133204", "473900" }, // ITANHOMI/MG
            { "3133303", "429500" }, // ITAOBIM/MG
            { "3133402", "455500" }, // ITAPAGIPE/MG
            { "3133501", "511000" }, // ITAPECERICA/MG
            { "3133600", "529400" }, // ITAPEVA/MG
            { "3133709", "544800" }, // ITATIAIUÇU/MG
            { "3133758", "508000" }, // ITAÚ DE MINAS/MG
            { "3133808", "541900" }, // ITAÚNA/MG
            { "3133907", "544900" }, // ITAVERAVA/MG
            { "3134004", "429600" }, // ITINGA/MG
            { "3134103", "474000" }, // ITUETA/MG
            { "3134202", "453300" }, // ITUIUTABA/MG
            { "3134301", "525800" }, // ITUMIRIM/MG
            { "3134400", "455600" }, // ITURAMA/MG
            { "3134509", "525900" }, // ITUTINGA/MG
            { "3134608", "533100" }, // JABOTICATUBAS/MG
            { "3134707", "431700" }, // JACINTO/MG
            { "3134806", "514500" }, // JACUÍ/MG
            { "3134905", "520400" }, // JACUTINGA/MG
            { "3135001", "536300" }, // JAGUARAÇU/MG
            { "3135050", "116500" }, // JAÍBA/MG
            { "3135076", "070000" }, // JAMPRUCA/MG
            { "3135100", "422600" }, // JANAÚBA/MG
            { "3135209", "411500" }, // JANUÁRIA/MG
            { "3135308", "501900" }, // JAPARAÍBA/MG
            { "3135357", "167800" }, // JAPONVAR/MG
            { "3135407", "545000" }, // JECEABA/MG
            { "3135456", "169500" }, // JENIPAPO DE MINAS/MG
            { "3135506", "479000" }, // JEQUERI/MG
            { "3135605", "422700" }, // JEQUITAÍ/MG
            { "3135704", "533200" }, // JEQUITIBÁ/MG
            { "3135803", "431800" }, // JEQUITINHONHA/MG
            { "3135902", "520500" }, // JESUÂNIA/MG
            { "3136009", "431900" }, // JOAÍMA/MG
            { "3136108", "460000" }, // JOANÉSIA/MG
            { "3136207", "536400" }, // JOÃO MONLEVADE/MG
            { "3136306", "416700" }, // JOÃO PINHEIRO/MG
            { "3136405", "443400" }, // JOAQUIM FELÍCIO/MG
            { "3136504", "432000" }, // JORDÂNIA/MG
            { "3136520", "433901" }, // JOSÉ GONÇALVES DE MINAS/MG
            { "3136553", "239400" }, // JOSÉ RAYDAN/MG
            { "3136579", "428202" }, // JOSENÓPOLIS/MG
            { "3136652", "071200" }, // JUATUBA/MG
            { "3136702", "556000" }, // JUIZ DE FORA/MG
            { "3136801", "422800" }, // JURAMENTO/MG
            { "3136900", "514600" }, // JURUAIA/MG
            { "3136959", "176700" }, // JUVENÍLIA/MG
            { "3137007", "438400" }, // LADAINHA/MG
            { "3137106", "416800" }, // LAGAMAR/MG
            { "3137205", "502000" }, // LAGOA DA PRATA/MG
            { "3137304", "422900" }, // LAGOA DOS PATOS/MG
            { "3137403", "551700" }, // LAGOA DOURADA/MG
            { "3137502", "448200" }, // LAGOA FORMOSA/MG
            { "3137536", "072900" }, // LAGOA GRANDE/MG
            { "3137601", "561400" }, // LAGOA SANTA/MG
            { "3137700", "483000" }, // LAJINHA/MG
            { "3137809", "520600" }, // LAMBARI/MG
            { "3137908", "486600" }, // LAMIM/MG
            { "3138005", "496100" }, // LARANJAL/MG
            { "3138104", "443500" }, // LASSANCE/MG
            { "3138203", "526000" }, // LAVRAS/MG
            { "3138302", "502100" }, // LEANDRO FERREIRA/MG
            { "3138351", "435101" }, // LEME DO PRADO/MG
            { "3138401", "496200" }, // LEOPOLDINA/MG
            { "3138500", "526100" }, // LIBERDADE/MG
            { "3138609", "556100" }, // LIMA DUARTE/MG
            { "3138625", "116600" }, // LIMEIRA DO OESTE/MG
            { "3138658", "071300" }, // LONTRA/MG
            { "3138674", "483101" }, // LUISBURGO/MG
            { "3138682", "421804" }, // LUISLÂNDIA/MG
            { "3138708", "526200" }, // LUMINÁRIAS/MG
            { "3138807", "502200" }, // LUZ/MG
            { "3138906", "441300" }, // MACHACALIS/MG
            { "3139003", "507000" }, // MACHADO/MG
            { "3139102", "526300" }, // MADRE DE DEUS DE MINAS/MG
            { "3139201", "438500" }, // MALACACHETA/MG
            { "3139250", "072500" }, // MAMONAS/MG
            { "3139300", "411600" }, // MANGA/MG
            { "3139409", "483100" }, // MANHUAÇU/MG
            { "3139508", "483200" }, // MANHUMIRIM/MG
            { "3139607", "468300" }, // MANTENA/MG
            { "3139805", "556200" }, // MAR DE ESPANHA/MG
            { "3139706", "533300" }, // MARAVILHAS/MG
            { "3139904", "529500" }, // MARIA DA FÉ/MG
            { "3140001", "545100" }, // MARIANA/MG
            { "3140100", "465200" }, // MARILAC/MG
            { "3140159", "169600" }, // MÁRIO CAMPOS/MG
            { "3140209", "556300" }, // MARIPÁ DE MINAS/MG
            { "3140308", "536500" }, // MARLIÉRIA/MG
            { "3140407", "529600" }, // MARMELÓPOLIS/MG
            { "3140506", "450900" }, // MARTINHO CAMPOS/MG
            { "3140530", "483202" }, // MARTINS SOARES/MG
            { "3140555", "070200" }, // MATA VERDE/MG
            { "3140605", "460100" }, // MATERLÂNDIA/MG
            { "3140704", "542000" }, // MATEUS LEME/MG
            { "3171501", "466100" }, // MATHIAS LOBATO/MG
            { "3140803", "556400" }, // MATIAS BARBOSA/MG
            { "3140852", "072600" }, // MATIAS CARDOSO/MG
            { "3140902", "483300" }, // MATIPÓ/MG
            { "3141009", "414500" }, // MATO VERDE/MG
            { "3141108", "561500" }, // MATOZINHOS/MG
            { "3141207", "448300" }, // MATUTINA/MG
            { "3141306", "502300" }, // MEDEIROS/MG
            { "3141405", "429700" }, // MEDINA/MG
            { "3141504", "468400" }, // MENDES PIMENTEL/MG
            { "3141603", "556500" }, // MERCÊS/MG
            { "3141702", "460200" }, // MESQUITA/MG
            { "3141801", "435100" }, // MINAS NOVAS/MG
            { "3141900", "526400" }, // MINDURI/MG
            { "3142007", "423000" }, // MIRABELA/MG
            { "3142106", "489700" }, // MIRADOURO/MG
            { "3142205", "489800" }, // MIRAÍ/MG
            { "3142254", "411603" }, // MIRAVÂNIA/MG
            { "3142304", "545200" }, // MOEDA/MG
            { "3142403", "502400" }, // MOEMA/MG
            { "3142502", "443600" }, // MONJOLOS/MG
            { "3142601", "520700" }, // MONSENHOR PAULO/MG
            { "3142700", "411700" }, // MONTALVÂNIA/MG
            { "3142809", "453400" }, // MONTE ALEGRE DE MINAS/MG
            { "3142908", "414600" }, // MONTE AZUL/MG
            { "3143005", "514700" }, // MONTE BELO/MG
            { "3143104", "446200" }, // MONTE CARMELO/MG
            { "3143153", "169700" }, // MONTE FORMOSO/MG
            { "3143203", "514800" }, // MONTE SANTO DE MINAS/MG
            { "3143401", "520800" }, // MONTE SIÃO/MG
            { "3143302", "423100" }, // MONTES CLAROS/MG
            { "3143450", "071400" }, // MONTEZUMA/MG
            { "3143500", "451000" }, // MORADA NOVA DE MINAS/MG
            { "3143609", "443700" }, // MORRO DA GARÇA/MG
            { "3143708", "536600" }, // MORRO DO PILAR/MG
            { "3143807", "529700" }, // MUNHOZ/MG
            { "3143906", "489900" }, // MURIAÉ/MG
            { "3144003", "474100" }, // MUTUM/MG
            { "3144102", "514900" }, // MUZAMBINHO/MG
            { "3144201", "465300" }, // NACIP RAYDAN/MG
            { "3144300", "441400" }, // NANUQUE/MG
            { "3144359", "458804" }, // NAQUE/MG
            { "3144375", "169800" }, // NATALÂNDIA/MG
            { "3144409", "520900" }, // NATÉRCIA/MG
            { "3144508", "551800" }, // NAZARENO/MG
            { "3144607", "507100" }, // NEPOMUCENO/MG
            { "3144656", "169900" }, // NINHEIRA/MG
            { "3144672", "468303" }, // NOVA BELÉM/MG
            { "3144706", "536700" }, // NOVA ERA/MG
            { "3144805", "561600" }, // NOVA LIMA/MG
            { "3144904", "465400" }, // NOVA MÓDICA/MG
            { "3145000", "499400" }, // NOVA PONTE/MG
            { "3145059", "170000" }, // NOVA PORTEIRINHA/MG
            { "3145109", "515000" }, // NOVA RESENDE/MG
            { "3145208", "502500" }, // NOVA SERRANA/MG
            { "3136603", "" }, // NOVA UNIÃO/MG
            { "3145307", "429800" }, // NOVO CRUZEIRO/MG
            { "3145356", "170100" }, // NOVO ORIENTE DE MINAS/MG
            { "3145372", "170200" }, // NOVORIZONTE/MG
            { "3145406", "556600" }, // OLARIA/MG
            { "3145455", "421702" }, // OLHOS D'ÁGUA/MG
            { "3145505", "521000" }, // OLÍMPIO NORONHA/MG
            { "3145604", "511100" }, // OLIVEIRA/MG
            { "3145703", "556700" }, // OLIVEIRA FORTES/MG
            { "3145802", "542100" }, // ONÇA DE PITANGUI/MG
            { "3145851", "479201" }, // ORATÓRIOS/MG
            { "3145877", "482802" }, // ORIZÂNIA/MG
            { "3145901", "545300" }, // OURO BRANCO/MG
            { "3146008", "521100" }, // OURO FINO/MG
            { "3146107", "545400" }, // OURO PRETO/MG
            { "3146206", "441500" }, // OURO VERDE DE MINAS/MG
            { "3146255", "428203" }, // PADRE CARVALHO/MG
            { "3146305", "429900" }, // PADRE PARAÍSO/MG
            { "3146552", "414703" }, // PAI PEDRO/MG
            { "3146404", "451100" }, // PAINEIRAS/MG
            { "3146503", "502600" }, // PAINS/MG
            { "3146602", "556800" }, // PAIVA/MG
            { "3146701", "496300" }, // PALMA/MG
            { "3146750", "070300" }, // PALMÓPOLIS/MG
            { "3146909", "533400" }, // PAPAGAIOS/MG
            { "3147105", "542200" }, // PARÁ DE MINAS/MG
            { "3147006", "416900" }, // PARACATU/MG
            { "3147204", "507200" }, // PARAGUAÇU/MG
            { "3147303", "529800" }, // PARAISÓPOLIS/MG
            { "3147402", "533500" }, // PARAOPEBA/MG
            { "3147600", "529900" }, // PASSA QUATRO/MG
            { "3147709", "511200" }, // PASSA TEMPO/MG
            { "3147808", "526500" }, // PASSA VINTE/MG
            { "3147501", "536800" }, // PASSABÉM/MG
            { "3147907", "507300" }, // PASSOS/MG
            { "3147956", "423002" }, // PATIS/MG
            { "3148004", "448400" }, // PATOS DE MINAS/MG
            { "3148103", "446300" }, // PATROCÍNIO/MG
            { "3148202", "490000" }, // PATROCÍNIO DO MURIAÉ/MG
            { "3148301", "486700" }, // PAULA CÂNDIDO/MG
            { "3148400", "460300" }, // PAULISTAS/MG
            { "3148509", "438600" }, // PAVÃO/MG
            { "3148608", "460400" }, // PEÇANHA/MG
            { "3148707", "430000" }, // PEDRA AZUL/MG
            { "3148756", "478402" }, // PEDRA BONITA/MG
            { "3148806", "486800" }, // PEDRA DO ANTA/MG
            { "3148905", "511300" }, // PEDRA DO INDAIÁ/MG
            { "3149002", "490100" }, // PEDRA DOURADA/MG
            { "3149101", "521200" }, // PEDRALVA/MG
            { "3149150", "072700" }, // PEDRAS DE MARIA DA CRUZ/MG
            { "3149200", "499500" }, // PEDRINÓPOLIS/MG
            { "3149309", "561700" }, // PEDRO LEOPOLDO/MG
            { "3149408", "556900" }, // PEDRO TEIXEIRA/MG
            { "3149507", "557000" }, // PEQUERI/MG
            { "3149606", "533600" }, // PEQUI/MG
            { "3149705", "502700" }, // PERDIGÃO/MG
            { "3149804", "499600" }, // PERDIZES/MG
            { "3149903", "511400" }, // PERDÕES/MG
            { "3149952", "458806" }, // PERIQUITO/MG
            { "3150000", "465500" }, // PESCADOR/MG
            { "3150109", "493300" }, // PIAU/MG
            { "3150158", "170300" }, // PIEDADE DE CARATINGA/MG
            { "3150208", "479100" }, // PIEDADE DE PONTE NOVA/MG
            { "3150307", "526600" }, // PIEDADE DO RIO GRANDE/MG
            { "3150406", "545500" }, // PIEDADE DOS GERAIS/MG
            { "3150505", "502800" }, // PIMENTA/MG
            { "3150539", "" }, // PINGO-D'ÁGUA/MG
            { "3150570", "170500" }, // PINTÓPOLIS/MG
            { "3150604", "511500" }, // PIRACEMA/MG
            { "3150703", "455700" }, // PIRAJUBA/MG
            { "3150802", "486900" }, // PIRANGA/MG
            { "3150901", "530000" }, // PIRANGUÇU/MG
            { "3151008", "521300" }, // PIRANGUINHO/MG
            { "3151107", "496400" }, // PIRAPETINGA/MG
            { "3151206", "418900" }, // PIRAPORA/MG
            { "3151305", "493400" }, // PIRAÚBA/MG
            { "3151404", "502900" }, // PITANGUI/MG
            { "3151503", "503000" }, // PIUMHI/MG
            { "3151602", "455800" }, // PLANURA/MG
            { "3151701", "521400" }, // POÇO FUNDO/MG
            { "3151800", "517200" }, // POÇOS DE CALDAS/MG
            { "3151909", "474200" }, // POCRANE/MG
            { "3152006", "451200" }, // POMPÉU/MG
            { "3152105", "479200" }, // PONTE NOVA/MG
            { "3152131", "170600" }, // PONTO CHIQUE/MG
            { "3152170", "170700" }, // PONTO DOS VOLANTES/MG
            { "3152204", "414700" }, // PORTEIRINHA/MG
            { "3152303", "487000" }, // PORTO FIRME/MG
            { "3152402", "438700" }, // POTÉ/MG
            { "3152501", "521500" }, // POUSO ALEGRE/MG
            { "3152600", "530100" }, // POUSO ALTO/MG
            { "3152709", "551900" }, // PRADOS/MG
            { "3152808", "455900" }, // PRATA/MG
            { "3152907", "507400" }, // PRATÁPOLIS/MG
            { "3153004", "499700" }, // PRATINHA/MG
            { "3153103", "487100" }, // PRESIDENTE BERNARDES/MG
            { "3153202", "443800" }, // PRESIDENTE JUSCELINO/MG
            { "3153301", "435200" }, // PRESIDENTE KUBITSCHEK/MG
            { "3153400", "417000" }, // PRESIDENTE OLEGÁRIO/MG
            { "3153608", "561800" }, // PRUDENTE DE MORAIS/MG
            { "3153707", "451300" }, // QUARTEL GERAL/MG
            { "3153806", "545600" }, // QUELUZITA/MG
            { "3153905", "561900" }, // RAPOSOS/MG
            { "3154002", "479300" }, // RAUL SOARES/MG
            { "3154101", "496500" }, // RECREIO/MG
            { "3154150", "483102" }, // REDUTO/MG
            { "3154200", "552000" }, // RESENDE COSTA/MG
            { "3154309", "474300" }, // RESPLENDOR/MG
            { "3154408", "552100" }, // RESSAQUINHA/MG
            { "3154457", "116700" }, // RIACHINHO/MG
            { "3154507", "414800" }, // RIACHO DOS MACHADOS/MG
            { "3154606", "562000" }, // RIBEIRÃO DAS NEVES/MG
            { "3154705", "511600" }, // RIBEIRÃO VERMELHO/MG
            { "3154804", "562100" }, // RIO ACIMA/MG
            { "3154903", "479400" }, // RIO CASCA/MG
            { "3155108", "432100" }, // RIO DO PRADO/MG
            { "3155009", "479500" }, // RIO DOCE/MG
            { "3155207", "487200" }, // RIO ESPERA/MG
            { "3155306", "545700" }, // RIO MANSO/MG
            { "3155405", "493500" }, // RIO NOVO/MG
            { "3155504", "448500" }, // RIO PARANAÍBA/MG
            { "3155603", "419800" }, // RIO PARDO DE MINAS/MG
            { "3155702", "536900" }, // RIO PIRACICABA/MG
            { "3155801", "493600" }, // RIO POMBA/MG
            { "3155900", "557100" }, // RIO PRETO/MG
            { "3156007", "460500" }, // RIO VERMELHO/MG
            { "3156106", "552200" }, // RITÁPOLIS/MG
            { "3156205", "557200" }, // ROCHEDO DE MINAS/MG
            { "3156304", "493700" }, // RODEIRO/MG
            { "3156403", "446400" }, // ROMARIA/MG
            { "3156452", "489906" }, // ROSÁRIO DA LIMEIRA/MG
            { "3156502", "419900" }, // RUBELITA/MG
            { "3156601", "432200" }, // RUBIM/MG
            { "3156700", "562200" }, // SABARÁ/MG
            { "3156809", "460600" }, // SABINÓPOLIS/MG
            { "3156908", "499800" }, // SACRAMENTO/MG
            { "3157005", "420000" }, // SALINAS/MG
            { "3157104", "432300" }, // SALTO DA DIVISA/MG
            { "3157203", "469504" }, // SANTA BÁRBARA/MG
            { "3157252", "116800" }, // SANTA BÁRBARA DO LESTE/MG
            { "3157278", "557101" }, // SANTA BÁRBARA DO MONTE VERDE/MG
            { "3157302", "552300" }, // SANTA BÁRBARA DO TUGÚRIO/MG
            { "3157336", "552701" }, // SANTA CRUZ DE MINAS/MG
            { "3157377", "420002" }, // SANTA CRUZ DE SALINAS/MG
            { "3157401", "479600" }, // SANTA CRUZ DO ESCALVADO/MG
            { "3157500", "465600" }, // SANTA EFIGÊNIA DE MINAS/MG
            { "3157609", "419000" }, // SANTA FÉ DE MINAS/MG
            { "3157658", "170800" }, // SANTA HELENA DE MINAS/MG
            { "3157708", "499900" }, // SANTA JULIANA/MG
            { "3157807", "562300" }, // SANTA LUZIA/MG
            { "3157906", "483500" }, // SANTA MARGARIDA/MG
            { "3158003", "537100" }, // SANTA MARIA DE ITABIRA/MG
            { "3158102", "432400" }, // SANTA MARIA DO SALTO/MG
            { "3158201", "460700" }, // SANTA MARIA DO SUAÇUÍ/MG
            { "3159209", "517300" }, // SANTA RITA DE CALDAS/MG
            { "3159308", "557400" }, // SANTA RITA DE JACUTINGA/MG
            { "3159357", "116900" }, // SANTA RITA DE MINAS/MG
            { "3159407", "557500" }, // SANTA RITA DO IBITIPOCA/MG
            { "3159506", "474400" }, // SANTA RITA DO ITUETO/MG
            { "3159605", "521600" }, // SANTA RITA DO SAPUCAÍ/MG
            { "3159704", "503100" }, // SANTA ROSA DA SERRA/MG
            { "3159803", "453500" }, // SANTA VITÓRIA/MG
            { "3158300", "507500" }, // SANTANA DA VARGEM/MG
            { "3158409", "496600" }, // SANTANA DE CATAGUASES/MG
            { "3158508", "533700" }, // SANTANA DE PIRAPAMA/MG
            { "3158607", "557300" }, // SANTANA DO DESERTO/MG
            { "3158706", "526700" }, // SANTANA DO GARAMBÉU/MG
            { "3158805", "511700" }, // SANTANA DO JACARÉ/MG
            { "3158904", "483600" }, // SANTANA DO MANHUAÇU/MG
            { "3158953", "070500" }, // SANTANA DO PARAÍSO/MG
            { "3159001", "533800" }, // SANTANA DO RIACHO/MG
            { "3159100", "545800" }, // SANTANA DOS MONTES/MG
            { "3159902", "511800" }, // SANTO ANTÔNIO DO AMPARO/MG
            { "3160009", "496700" }, // SANTO ANTÔNIO DO AVENTUREIRO/MG
            { "3160108", "479700" }, // SANTO ANTÔNIO DO GRAMA/MG
            { "3160207", "460800" }, // SANTO ANTÔNIO DO ITAMBÉ/MG
            { "3160306", "432500" }, // SANTO ANTÔNIO DO JACINTO/MG
            { "3160405", "503200" }, // SANTO ANTÔNIO DO MONTE/MG
            { "3160454", "419803" }, // SANTO ANTÔNIO DO RETIRO/MG
            { "3160504", "537200" }, // SANTO ANTÔNIO DO RIO ABAIXO/MG
            { "3160603", "443900" }, // SANTO HIPÓLITO/MG
            { "3160702", "557600" }, // SANTOS DUMONT/MG
            { "3160801", "521700" }, // SÃO BENTO ABADE/MG
            { "3160900", "545900" }, // SÃO BRÁS DO SUAÇUÍ/MG
            { "3160959", "470102" }, // SÃO DOMINGOS DAS DORES/MG
            { "3161007", "537300" }, // SÃO DOMINGOS DO PRATA/MG
            { "3161056", "170900" }, // SÃO FÉLIX DE MINAS/MG
            { "3161106", "411800" }, // SÃO FRANCISCO/MG
            { "3161205", "511900" }, // SÃO FRANCISCO DE PAULA/MG
            { "3161304", "456000" }, // SÃO FRANCISCO DE SALES/MG
            { "3161403", "490200" }, // SÃO FRANCISCO DO GLÓRIA/MG
            { "3161502", "422104" }, // SÃO GERALDO/MG
            { "3161601", "465700" }, // SÃO GERALDO DA PIEDADE/MG
            { "3161650", "473701" }, // SÃO GERALDO DO BAIXIO/MG
            { "3161700", "448600" }, // SÃO GONÇALO DO ABAETÉ/MG
            { "3161809", "542300" }, // SÃO GONÇALO DO PARÁ/MG
            { "3161908", "537400" }, // SÃO GONÇALO DO RIO ABAIXO/MG
            { "3125507", "" }, // SÃO GONÇALO DO RIO PRETO/MG
            { "3162005", "521800" }, // SÃO GONÇALO DO SAPUCAÍ/MG
            { "3162104", "448700" }, // SÃO GOTARDO/MG
            { "3162203", "507600" }, // SÃO JOÃO BATISTA DO GLÓRIA/MG
            { "3162252", "422105" }, // SÃO JOÃO DA LAGOA/MG
            { "3162302", "521900" }, // SÃO JOÃO DA MATA/MG
            { "3162401", "423200" }, // SÃO JOÃO DA PONTE/MG
            { "3162450", "171000" }, // SÃO JOÃO DAS MISSÕES/MG
            { "3162500", "552400" }, // SÃO JOÃO DEL REI/MG
            { "3162559", "070700" }, // SÃO JOÃO DO MANHUAÇU/MG
            { "3162575", "117000" }, // SÃO JOÃO DO MANTENINHA/MG
            { "3162609", "470200" }, // SÃO JOÃO DO ORIENTE/MG
            { "3162658", "422106" }, // SÃO JOÃO DO PACUÍ/MG
            { "3162708", "420100" }, // SÃO JOÃO DO PARAÍSO/MG
            { "3162807", "460900" }, // SÃO JOÃO EVANGELISTA/MG
            { "3162906", "557700" }, // SÃO JOÃO NEPOMUCENO/MG
            { "3162922", "235700" }, // SÃO JOAQUIM DE BICAS/MG
            { "3162948", "505301" }, // SÃO JOSÉ DA BARRA/MG
            { "3162955", "069800" }, // SÃO JOSÉ DA LAPA/MG
            { "3163003", "465800" }, // SÃO JOSÉ DA SAFIRA/MG
            { "3163102", "542400" }, // SÃO JOSÉ DA VARGINHA/MG
            { "3163201", "522000" }, // SÃO JOSÉ DO ALEGRE/MG
            { "3163300", "465900" }, // SÃO JOSÉ DO DIVINO/MG
            { "3163409", "537500" }, // SÃO JOSÉ DO GOIABAL/MG
            { "3163508", "461000" }, // SÃO JOSÉ DO JACURI/MG
            { "3163607", "483700" }, // SÃO JOSÉ DO MANTIMENTO/MG
            { "3163706", "522100" }, // SÃO LOURENÇO/MG
            { "3163805", "487300" }, // SÃO MIGUEL DO ANTA/MG
            { "3163904", "515100" }, // SÃO PEDRO DA UNIÃO/MG
            { "3164100", "461100" }, // SÃO PEDRO DO SUAÇUÍ/MG
            { "3164001", "479800" }, // SÃO PEDRO DOS FERROS/MG
            { "3164209", "419100" }, // SÃO ROMÃO/MG
            { "3164308", "503300" }, // SÃO ROQUE DE MINAS/MG
            { "3164407", "522200" }, // SÃO SEBASTIÃO DA BELA VISTA/MG
            { "3164431", "489802" }, // SÃO SEBASTIÃO DA VARGEM ALEGRE/MG
            { "3164472", "215000" }, // SÃO SEBASTIÃO DO ANTA/MG
            { "3164506", "461200" }, // SÃO SEBASTIÃO DO MARANHÃO/MG
            { "3164605", "512000" }, // SÃO SEBASTIÃO DO OESTE/MG
            { "3164704", "515200" }, // SÃO SEBASTIÃO DO PARAÍSO/MG
            { "3164803", "537600" }, // SÃO SEBASTIÃO DO RIO PRETO/MG
            { "3164902", "530200" }, // SÃO SEBASTIÃO DO RIO VERDE/MG
            { "3165206", "" }, // SÃO THOMÉ DAS LETRAS/MG
            { "3165008", "552500" }, // SÃO TIAGO/MG
            { "3165107", "515300" }, // SÃO TOMÁS DE AQUINO/MG
            { "3165305", "526900" }, // SÃO VICENTE DE MINAS/MG
            { "3165404", "530300" }, // SAPUCAÍ-MIRIM/MG
            { "3165503", "466000" }, // SARDOÁ/MG
            { "3165537", "561202" }, // SARZEDO/MG
            { "3165560", "171100" }, // SEM PEIXE/MG
            { "3165578", "071100" }, // SENADOR AMARAL/MG
            { "3165602", "557800" }, // SENADOR CORTES/MG
            { "3165701", "487400" }, // SENADOR FIRMINO/MG
            { "3165800", "522300" }, // SENADOR JOSÉ BENTO/MG
            { "3165909", "435300" }, // SENADOR MODESTINO GONÇALVES/MG
            { "3166006", "487500" }, // SENHORA DE OLIVEIRA/MG
            { "3166105", "461300" }, // SENHORA DO PORTO/MG
            { "3166204", "552600" }, // SENHORA DOS REMÉDIOS/MG
            { "3166303", "479900" }, // SERICITA/MG
            { "3166402", "527000" }, // SERITINGA/MG
            { "3166501", "461400" }, // SERRA AZUL DE MINAS/MG
            { "3166600", "503400" }, // SERRA DA SAUDADE/MG
            { "3166808", "446500" }, // SERRA DO SALITRE/MG
            { "3166709", "441600" }, // SERRA DOS AIMORÉS/MG
            { "3166907", "507700" }, // SERRANIA/MG
            { "3166956", "171200" }, // SERRANÓPOLIS DE MINAS/MG
            { "3167004", "527100" }, // SERRANOS/MG
            { "3167103", "435400" }, // SERRO/MG
            { "3167202", "533900" }, // SETE LAGOAS/MG
            { "3165552", "438506" }, // SETUBINHA/MG
            { "3167301", "493900" }, // SILVEIRÂNIA/MG
            { "3167400", "522400" }, // SILVIANÓPOLIS/MG
            { "3167509", "557900" }, // SIMÃO PEREIRA/MG
            { "3167608", "483800" }, // SIMONÉSIA/MG
            { "3167707", "470300" }, // SOBRÁLIA/MG
            { "3167806", "522500" }, // SOLEDADE DE MINAS/MG
            { "3167905", "494000" }, // TABULEIRO/MG
            { "3168002", "420200" }, // TAIOBEIRAS/MG
            { "3168051", "473801" }, // TAPARUBA/MG
            { "3168101", "500000" }, // TAPIRA/MG
            { "3168200", "503500" }, // TAPIRAÍ/MG
            { "3168309", "562400" }, // TAQUARAÇU DE MINAS/MG
            { "3168408", "470400" }, // TARUMIRIM/MG
            { "3168507", "487600" }, // TEIXEIRAS/MG
            { "3168606", "438800" }, // TEÓFILO OTONI/MG
            { "3168705", "537700" }, // TIMÓTEO/MG
            { "3168804", "552700" }, // TIRADENTES/MG
            { "3168903", "448800" }, // TIROS/MG
            { "3169000", "494100" }, // TOCANTINS/MG
            { "3169059", "215100" }, // TOCOS DO MOJI/MG
            { "3169109", "530400" }, // TOLEDO/MG
            { "3169208", "490300" }, // TOMBOS/MG
            { "3169307", "522600" }, // TRÊS CORAÇÕES/MG
            { "3169356", "451400" }, // TRÊS MARIAS/MG
            { "3169406", "507800" }, // TRÊS PONTAS/MG
            { "3169505", "474500" }, // TUMIRITINGA/MG
            { "3169604", "453600" }, // TUPACIGUARA/MG
            { "3169703", "435500" }, // TURMALINA/MG
            { "3169802", "522700" }, // TURVOLÂNDIA/MG
            { "3169901", "494200" }, // UBÁ/MG
            { "3170008", "423300" }, // UBAÍ/MG
            { "3170057", "070400" }, // UBAPORANGA/MG
            { "3170107", "457800" }, // UBERABA/MG
            { "3170206", "453700" }, // UBERLÂNDIA/MG
            { "3170305", "441700" }, // UMBURATIBA/MG
            { "3170404", "417100" }, // UNAÍ/MG
            { "3170438", "171300" }, // UNIÃO DE MINAS/MG
            { "3170479", "171400" }, // URUANA DE MINAS/MG
            { "3170503", "480000" }, // URUCÂNIA/MG
            { "3170529", "071500" }, // URUCUIA/MG
            { "3170578", "469511" }, // VARGEM ALEGRE/MG
            { "3170602", "503600" }, // VARGEM BONITA/MG
            { "3170651", "171500" }, // VARGEM GRANDE DO RIO PARDO/MG
            { "3170701", "507900" }, // VARGINHA/MG
            { "3170750", "171600" }, // VARJÃO DE MINAS/MG
            { "3170800", "444000" }, // VÁRZEA DA PALMA/MG
            { "3170909", "423400" }, // VARZELÂNDIA/MG
            { "3171006", "417200" }, // VAZANTE/MG
            { "3171030", "423404" }, // VERDELÂNDIA/MG
            { "3171071", "435502" }, // VEREDINHA/MG
            { "3171105", "457900" }, // VERÍSSIMO/MG
            { "3171154", "479305" }, // VERMELHO NOVO/MG
            { "3171204", "562500" }, // VESPASIANO/MG
            { "3171303", "487700" }, // VIÇOSA/MG
            { "3171402", "490400" }, // VIEIRAS/MG
            { "3171600", "430100" }, // VIRGEM DA LAPA/MG
            { "3171709", "530500" }, // VIRGÍNIA/MG
            { "3171808", "461500" }, // VIRGINÓPOLIS/MG
            { "3171907", "466200" }, // VIRGOLÂNDIA/MG
            { "3172004", "494300" }, // VISCONDE DO RIO BRANCO/MG
            { "3172103", "496800" }, // VOLTA GRANDE/MG
            { "3172202", "530600" }, // WENCESLAU BRAZ/MG
            { "5000203", "047900" }, // ÁGUA CLARA/MS
            { "5000252", "122700" }, // ALCINÓPOLIS/MS
            { "5000609", "049200" }, // AMAMBAÍ/MS
            { "5000708", "055200" }, // ANASTÁCIO/MS
            { "5000807", "049300" }, // ANAURILÂNDIA/MS
            { "5000856", "049400" }, // ANGÉLICA/MS
            { "5000906", "057400" }, // ANTÔNIO JOÃO/MS
            { "5001003", "045600" }, // APARECIDA DO TABOADO/MS
            { "5001102", "055300" }, // AQUIDAUANA/MS
            { "5001243", "049500" }, // ARAL MOREIRA/MS
            { "5001508", "041500" }, // BANDEIRANTES/MS
            { "5001904", "049600" }, // BATAGUASSU/MS
            { "5002001", "" }, // BATAYPORÃ/MS
            { "5002100", "057500" }, // BELA VISTA/MS
            { "5002159", "055400" }, // BODOQUENA/MS
            { "5002209", "057600" }, // BONITO/MS
            { "5002308", "048000" }, // BRASILÂNDIA/MS
            { "5002407", "049800" }, // CAARAPÓ/MS
            { "5002605", "044100" }, // CAMAPUÃ/MS
            { "5002704", "041600" }, // CAMPO GRANDE/MS
            { "5002803", "057700" }, // CARACOL/MS
            { "5002902", "045700" }, // CASSILÂNDIA/MS
            { "5002951", "002000" }, // CHAPADÃO DO SUL/MS
            { "5003108", "041700" }, // CORGUINHO/MS
            { "5003157", "051700" }, // CORONEL SAPUCAIA/MS
            { "5003207", "055500" }, // CORUMBÁ/MS
            { "5003256", "044200" }, // COSTA RICA/MS
            { "5003306", "044300" }, // COXIM/MS
            { "5003454", "049900" }, // DEODÁPOLIS/MS
            { "5003488", "055900" }, // DOIS IRMÃOS DO BURITI/MS
            { "5003504", "050000" }, // DOURADINA/MS
            { "5003702", "050100" }, // DOURADOS/MS
            { "5003751", "050200" }, // ELDORADO/MS
            { "5003801", "050300" }, // FÁTIMA DO SUL/MS
            { "5004007", "050400" }, // GLÓRIA DE DOURADOS/MS
            { "5004106", "057800" }, // GUIA LOPES DA LAGUNA/MS
            { "5004304", "050500" }, // IGUATEMI/MS
            { "5004403", "045800" }, // INOCÊNCIA/MS
            { "5004502", "050600" }, // ITAPORÃ/MS
            { "5004601", "051600" }, // ITAQUIRAÍ/MS
            { "5004700", "050700" }, // IVINHEMA/MS
            { "5004809", "122800" }, // JAPORÃ/MS
            { "5004908", "041800" }, // JARAGUARI/MS
            { "5005004", "057900" }, // JARDIM/MS
            { "5005103", "050800" }, // JATEÍ/MS
            { "5005152", "051800" }, // JUTÍ/MS
            { "5005202", "055600" }, // LADÁRIO/MS
            { "5005251", "122900" }, // LAGUNA CARAPÃ/MS
            { "5005400", "041900" }, // MARACAJU/MS
            { "5005608", "055700" }, // MIRANDA/MS
            { "5005681", "050900" }, // MUNDO NOVO/MS
            { "5005707", "051000" }, // NAVIRAÍ/MS
            { "5005806", "058000" }, // NIOAQUE/MS
            { "5006002", "123000" }, // NOVA ALVORADA DO SUL/MS
            { "5006200", "051100" }, // NOVA ANDRADINA/MS
            { "5006259", "123100" }, // NOVO HORIZONTE DO SUL/MS
            { "5006309", "045900" }, // PARANAÍBA/MS
            { "5006358", "051900" }, // PARANHOS/MS
            { "5006408", "044400" }, // PEDRO GOMES/MS
            { "5006606", "051200" }, // PONTA PORÃ/MS
            { "5006903", "055800" }, // PORTO MURTINHO/MS
            { "5007109", "042000" }, // RIBAS DO RIO PARDO/MS
            { "5007208", "042100" }, // RIO BRILHANTE/MS
            { "5007307", "042200" }, // RIO NEGRO/MS
            { "5007406", "044500" }, // RIO VERDE DE MATO GROSSO/MS
            { "5007505", "042300" }, // ROCHEDO/MS
            { "5007554", "017800" }, // SANTA RITA DO PARDO/MS
            { "5007695", "044600" }, // SÃO GABRIEL DO OESTE/MS
            { "5007802", "048100" }, // SELVÍRIA/MS
            { "5007703", "051300" }, // SETE QUEDAS/MS
            { "5007901", "042400" }, // SIDROLÂNDIA/MS
            { "5007935", "017900" }, // SONORA/MS
            { "5007950", "051400" }, // TACURU/MS
            { "5007976", "051500" }, // TAQUARUSSU/MS
            { "5008008", "042500" }, // TERENOS/MS
            { "5008305", "048200" }, // TRÊS LAGOAS/MS
            { "5008404", "052000" }, // VICENTINA/MS
            { "5100102", "065900" }, // ACORIZAL/MT
            { "5100201", "058700" }, // ÁGUA BOA/MT
            { "5100250", "058800" }, // ALTA FLORESTA/MT
            { "5100300", "071600" }, // ALTO ARAGUAIA/MT
            { "5100359", "123200" }, // ALTO BOA VISTA/MT
            { "5100409", "071700" }, // ALTO GARÇAS/MT
            { "5100508", "064200" }, // ALTO PARAGUAI/MT
            { "5100607", "017700" }, // ALTO TAQUARI/MT
            { "5100805", "013400" }, // APIACÁS/MT
            { "5101001", "059001" }, // ARAGUAIANA/MT
            { "5101209", "071800" }, // ARAGUAINHA/MT
            { "5101258", "062000" }, // ARAPUTANGA/MT
            { "5101308", "064300" }, // ARENÁPOLIS/MT
            { "5101407", "058900" }, // ARIPUANÃ/MT
            { "5101605", "066000" }, // BARÃO DE MELGAÇO/MT
            { "5101704", "064400" }, // BARRA DO BUGRES/MT
            { "5101803", "059000" }, // BARRA DO GARÇAS/MT
            { "5101852", "217100" }, // BOM JESUS DO ARAGUAIA/MT
            { "5101902", "014900" }, // BRASNORTE/MT
            { "5102504", "062100" }, // CÁCERES/MT
            { "5102603", "061500" }, // CAMPINÁPOLIS/MT
            { "5102637", "016500" }, // CAMPO NOVO DO PARECIS/MT
            { "5102678", "011000" }, // CAMPO VERDE/MT
            { "5102686", "191300" }, // CAMPOS DE JÚLIO/MT
            { "5102694", "210100" }, // CANABRAVA DO NORTE/MT
            { "5102702", "059100" }, // CANARANA/MT
            { "5102793", "191400" }, // CARLINDA/MT
            { "5102850", "061600" }, // CASTANHEIRA/MT
            { "5103007", "059200" }, // CHAPADA DOS GUIMARÃES/MT
            { "5103056", "016600" }, // CLÁUDIA/MT
            { "5103106", "059002" }, // COCALINHO/MT
            { "5103205", "059300" }, // COLÍDER/MT
            { "5103254", "228800" }, // COLNIZA/MT
            { "5103304", "061700" }, // COMODORO/MT
            { "5103353", "123400" }, // CONFRESA/MT
            { "5103361", "216700" }, // CONQUISTA D'OESTE/MT
            { "5103379", "124400" }, // COTRIGUAÇU/MT
            { "5103403", "066100" }, // CUIABÁ/MT
            { "5103437", "230900" }, // CURVELÂNDIA/MT
            { "5103452", "064700" }, // DENISE/MT
            { "5103502", "059400" }, // DIAMANTINO/MT
            { "5103601", "069200" }, // DOM AQUINO/MT
            { "5103700", "191500" }, // FELIZ NATAL/MT
            { "5103809", "053800" }, // FIGUEIRÓPOLIS D'OESTE/MT
            { "5103858", "191600" }, // GAÚCHA DO NORTE/MT
            { "5103908", "071900" }, // GENERAL CARNEIRO/MT
            { "5103957", "124500" }, // GLÓRIA D'OESTE/MT
            { "5104104", "015100" }, // GUARANTÃ DO NORTE/MT
            { "5104203", "072000" }, // GUIRATINGA/MT
            { "5104500", "062900" }, // INDIAVAÍ/MT
            { "5104559", "060900" }, // ITAÚBA/MT
            { "5104609", "069300" }, // ITIQUIRA/MT
            { "5104807", "069400" }, // JACIARA/MT
            { "5104906", "066700" }, // JANGADA/MT
            { "5105002", "062200" }, // JAURU/MT
            { "5105101", "059500" }, // JUARA/MT
            { "5105150", "059600" }, // JUÍNA/MT
            { "5105176", "015000" }, // JURUENA/MT
            { "5105200", "069500" }, // JUSCIMEIRA/MT
            { "5105234", "124600" }, // LAMBARI D'OESTE/MT
            { "5105259", "013800" }, // LUCAS DO RIO VERDE/MT
            { "5105309", "059700" }, // LUCIARA/MT
            { "5105580", "060602" }, // MARCELÂNDIA/MT
            { "5105606", "061300" }, // MATUPÁ/MT
            { "5105622", "062400" }, // MIRASSOL D'OESTE/MT
            { "5105903", "059800" }, // NOBRES/MT
            { "5106000", "064500" }, // NORTELÂNDIA/MT
            { "5106109", "066200" }, // NOSSA SENHORA DO LIVRAMENTO/MT
            { "5106158", "124700" }, // NOVA BANDEIRANTES/MT
            { "5106208", "059900" }, // NOVA BRASILÂNDIA/MT
            { "5106216", "015200" }, // NOVA CANÃA DO NORTE/MT
            { "5108808", "126300" }, // NOVA GUARITA/MT
            { "5106182", "191700" }, // NOVA LACERDA/MT
            { "5108857", "126400" }, // NOVA MARILÂNDIA/MT
            { "5108907", "126500" }, // NOVA MARINGÁ/MT
            { "5108956", "127400" }, // NOVA MONTE VERDE/MT
            { "5106224", "013900" }, // NOVA MUTUM/MT
            { "5106174", "228900" }, // NOVA NAZARÉ/MT
            { "5106232", "064800" }, // NOVA OLÍMPIA/MT
            { "5106190", "216500" }, // NOVA SANTA HELENA/MT
            { "5106240", "210200" }, // NOVA UBIRATÃ/MT
            { "5106257", "060000" }, // NOVA XAVANTINA/MT
            { "5106273", "014800" }, // NOVO HORIZONTE DO NORTE/MT
            { "5106265", "166600" }, // NOVO MUNDO/MT
            { "5106281", "011200" }, // NOVO SÃO JOAQUIM/MT
            { "5106299", "013700" }, // PARANAITÁ/MT
            { "5106307", "060100" }, // PARANATINGA/MT
            { "5106372", "069600" }, // PEDRA PRETA/MT
            { "5106422", "059304" }, // PEIXOTO DE AZEVEDO/MT
            { "5106455", "125400" }, // PLANALTO DA SERRA/MT
            { "5106505", "066300" }, // POCONÉ/MT
            { "5106653", "125500" }, // PONTAL DO ARAGUAIA/MT
            { "5106703", "072100" }, // PONTE BRANCA/MT
            { "5106752", "062500" }, // PONTES E LACERDA/MT
            { "5106778", "011300" }, // PORTO ALEGRE DO NORTE/MT
            { "5106802", "060200" }, // PORTO DOS GAÚCHOS/MT
            { "5106828", "063000" }, // PORTO ESPERIDIÃO/MT
            { "5106851", "067300" }, // PORTO ESTRELA/MT
            { "5107008", "" }, // POXORÉU/MT
            { "5107040", "011100" }, // PRIMAVERA DO LESTE/MT
            { "5107065", "125600" }, // QUERÊNCIA/MT
            { "5107156", "063100" }, // RESERVA DO CABAÇAL/MT
            { "5107180", "011400" }, // RIBEIRÃO CASCALHEIRA/MT
            { "5107198", "067200" }, // RIBEIRÃOZINHO/MT
            { "5107206", "062700" }, // RIO BRANCO/MT
            { "5107578", "229100" }, // RONDOLÂNDIA/MT
            { "5107602", "069700" }, // RONDONÓPOLIS/MT
            { "5107701", "066400" }, // ROSÁRIO OESTE/MT
            { "5107750", "062800" }, // SALTO DO CÉU/MT
            { "5107248", "126000" }, // SANTA CARMEM/MT
            { "5107743", "229200" }, // SANTA CRUZ DO XINGU/MT
            { "5107768", "230400" }, // SANTA RITA DO TRIVELATO/MT
            { "5107776", "060400" }, // SANTA TEREZINHA/MT
            { "5107263", "067400" }, // SANTO AFONSO/MT
            { "5107792", "216600" }, // SANTO ANTÔNIO DO LESTE/MT
            { "5107800", "066500" }, // SANTO ANTÔNIO DO LEVERGER/MT
            { "5107859", "060500" }, // SÃO FÉLIX DO ARAGUAIA/MT
            { "5107297", "069704" }, // SÃO JOSÉ DO POVO/MT
            { "5107305", "060300" }, // SÃO JOSÉ DO RIO CLARO/MT
            { "5107354", "126100" }, // SÃO JOSÉ DO XINGU/MT
            { "5107107", "062600" }, // SÃO JOSÉ DOS QUATRO MARCOS/MT
            { "5107404", "067100" }, // SÃO PEDRO DA CIPA/MT
            { "5107875", "166700" }, // SAPEZAL/MT
            { "5107883", "230500" }, // SERRA NOVA DOURADA/MT
            { "5107909", "060600" }, // SINOP/MT
            { "5107925", "061400" }, // SORRISO/MT
            { "5107941", "126200" }, // TABAPORÃ/MT
            { "5107958", "064600" }, // TANGARÁ DA SERRA/MT
            { "5108006", "014700" }, // TAPURAH/MT
            { "5108055", "016400" }, // TERRA NOVA DO NORTE/MT
            { "5108105", "072300" }, // TESOURO/MT
            { "5108204", "072400" }, // TORIXORÉU/MT
            { "5108303", "166800" }, // UNIÃO DO SUL/MT
            { "5108352", "230600" }, // VALE DE SÃO DOMINGOS/MT
            { "5108402", "066600" }, // VÁRZEA GRANDE/MT
            { "5108501", "060601" }, // VERA/MT
            { "5105507", "062300" }, // VILA BELA DA SANTÍSSIMA TRINDADE/MT
            { "5108600", "001900" }, // VILA RICA/MT
            { "1500107", "099600" }, // ABAETETUBA/PA
            { "1500131", "108100" }, // ABEL FIGUEIREDO/PA
            { "1500206", "103700" }, // ACARÁ/PA
            { "1500305", "095600" }, // AFUÁ/PA
            { "1500347", "108200" }, // ÁGUA AZUL DO NORTE/PA
            { "1500404", "090100" }, // ALENQUER/PA
            { "1500503", "092900" }, // ALMEIRIM/PA
            { "1500602", "093900" }, // ALTAMIRA/PA
            { "1500701", "095700" }, // ANAJÁS/PA
            { "1500800", "115200" }, // ANANINDEUA/PA
            { "1500859", "171800" }, // ANAPU/PA
            { "1500909", "110600" }, // AUGUSTO CORRÊA/PA
            { "1500958", "108300" }, // AURORA DO PARÁ/PA
            { "1501006", "092000" }, // AVEIRO/PA
            { "1501105", "099700" }, // BAGRE/PA
            { "1501204", "099800" }, // BAIÃO/PA
            { "1501253", "171900" }, // BANNACH/PA
            { "1501303", "099900" }, // BARCARENA/PA
            { "1501402", "115300" }, // BELÉM/PA
            { "1501451", "090703" }, // BELTERRA/PA
            { "1501501", "115400" }, // BENEVIDES/PA
            { "1501576", "005200" }, // BOM JESUS DO TOCANTINS/PA
            { "1501600", "110700" }, // BONITO/PA
            { "1501709", "110800" }, // BRAGANÇA/PA
            { "1501725", "108400" }, // BRASIL NOVO/PA
            { "1501758", "005100" }, // BREJO GRANDE DO ARAGUAIA/PA
            { "1501782", "108500" }, // BREU BRANCO/PA
            { "1501808", "095800" }, // BREVES/PA
            { "1501907", "104400" }, // BUJARU/PA
            { "1502004", "097600" }, // CACHOEIRA DO ARARI/PA
            { "1501956", "172000" }, // CACHOEIRA DO PIRIÁ/PA
            { "1502103", "100000" }, // CAMETÁ/PA
            { "1502152", "172100" }, // CANAÃ DOS CARAJÁS/PA
            { "1502202", "110900" }, // CAPANEMA/PA
            { "1502301", "104500" }, // CAPITÃO POÇO/PA
            { "1502400", "111000" }, // CASTANHAL/PA
            { "1502509", "097700" }, // CHAVES/PA
            { "1502608", "105800" }, // COLARES/PA
            { "1502707", "094600" }, // CONCEIÇÃO DO ARAGUAIA/PA
            { "1502756", "002600" }, // CONCÓRDIA DO PARÁ/PA
            { "1502764", "108600" }, // CUMARU DO NORTE/PA
            { "1502772", "005400" }, // CURIONÓPOLIS/PA
            { "1502806", "095900" }, // CURRALINHO/PA
            { "1502855", "090101" }, // CURUÁ/PA
            { "1502905", "105900" }, // CURUÇÁ/PA
            { "1502939", "005300" }, // DOM ELISEU/PA
            { "1502954", "108700" }, // ELDORADO DOS CARAJÁS/PA
            { "1503002", "090200" }, // FARO/PA
            { "1503044", "172200" }, // FLORESTA DO ARAGUAIA/PA
            { "1503077", "002500" }, // GARRAFÃO DO NORTE/PA
            { "1503093", "108800" }, // GOIANÉSIA DO PARÁ/PA
            { "1503101", "096000" }, // GURUPÁ/PA
            { "1503200", "111100" }, // IGARAPÉ-AÇU/PA
            { "1503309", "100100" }, // IGARAPÉ-MIRI/PA
            { "1503408", "111200" }, // INHANGAPI/PA
            { "1503457", "108900" }, // IPIXUNA DO PARÁ/PA
            { "1503507", "104600" }, // IRITUIA/PA
            { "1503606", "092100" }, // ITAITUBA/PA
            { "1503705", "102600" }, // ITUPIRANGA/PA
            { "1503754", "092101" }, // JACAREACANGA/PA
            { "1503804", "102700" }, // JACUNDÁ/PA
            { "1503903", "090300" }, // JURUTI/PA
            { "1504000", "100200" }, // LIMOEIRO DO AJURU/PA
            { "1504059", "002400" }, // MÃE DO RIO/PA
            { "1504109", "106000" }, // MAGALHÃES BARATA/PA
            { "1504208", "102800" }, // MARABÁ/PA
            { "1504307", "106100" }, // MARACANÃ/PA
            { "1504406", "106200" }, // MARAPANIM/PA
            { "1504422", "172300" }, // MARITUBA/PA
            { "1504455", "005700" }, // MEDICILÂNDIA/PA
            { "1504505", "096100" }, // MELGAÇO/PA
            { "1504604", "100300" }, // MOCAJUBA/PA
            { "1504703", "100400" }, // MOJU/PA
            { "1504802", "090400" }, // MONTE ALEGRE/PA
            { "1504901", "097800" }, // MUANÁ/PA
            { "1504950", "109100" }, // NOVA ESPERANÇA DO PIRIÁ/PA
            { "1504976", "172400" }, // NOVA IPIXUNA/PA
            { "1505007", "111300" }, // NOVA TIMBOTEUA/PA
            { "1505031", "109200" }, // NOVO PROGRESSO/PA
            { "1505064", "109300" }, // NOVO REPARTIMENTO/PA
            { "1505106", "090500" }, // ÓBIDOS/PA
            { "1505205", "100500" }, // OEIRAS DO PARÁ/PA
            { "1505304", "090600" }, // ORIXIMINÁ/PA
            { "1505403", "104700" }, // OURÉM/PA
            { "1505437", "005600" }, // OURILÂNDIA DO NORTE/PA
            { "1505486", "005800" }, // PACAJÁ/PA
            { "1505494", "109400" }, // PALESTINA DO PARÁ/PA
            { "1505502", "104800" }, // PARAGOMINAS/PA
            { "1505536", "090800" }, // PARAUAPEBAS/PA
            { "1505551", "109500" }, // PAU D'ARCO/PA
            { "1505601", "111400" }, // PEIXE-BOI/PA
            { "1505635", "095003" }, // PIÇARRA/PA
            { "1505650", "172600" }, // PLACAS/PA
            { "1505700", "097900" }, // PONTA DE PEDRAS/PA
            { "1505809", "096200" }, // PORTEL/PA
            { "1505908", "093000" }, // PORTO DE MOZ/PA
            { "1506005", "093100" }, // PRAINHA/PA
            { "1506104", "106300" }, // PRIMAVERA/PA
            { "1506112", "106302" }, // QUATIPURU/PA
            { "1506138", "094700" }, // REDENÇÃO/PA
            { "1506161", "094800" }, // RIO MARIA/PA
            { "1506187", "104900" }, // RONDON DO PARÁ/PA
            { "1506195", "008400" }, // RURÓPOLIS/PA
            { "1506203", "106400" }, // SALINÓPOLIS/PA
            { "1506302", "098000" }, // SALVATERRA/PA
            { "1506351", "109600" }, // SANTA BÁRBARA DO PARÁ/PA
            { "1506401", "098100" }, // SANTA CRUZ DO ARARI/PA
            { "1506500", "111500" }, // SANTA ISABEL DO PARÁ/PA
            { "1506559", "109700" }, // SANTA LUZIA DO PARÁ/PA
            { "1506583", "004000" }, // SANTA MARIA DAS BARREIRAS/PA
            { "1506609", "111600" }, // SANTA MARIA DO PARÁ/PA
            { "1506708", "094900" }, // SANTANA DO ARAGUAIA/PA
            { "1506807", "090700" }, // SANTARÉM/PA
            { "1506906", "106500" }, // SANTARÉM NOVO/PA
            { "1507003", "106600" }, // SANTO ANTÔNIO DO TAUÁ/PA
            { "1507102", "106700" }, // SÃO CAETANO DE ODIVELAS/PA
            { "1507151", "109800" }, // SÃO DOMINGOS DO ARAGUAIA/PA
            { "1507201", "105000" }, // SÃO DOMINGOS DO CAPIM/PA
            { "1507300", "094000" }, // SÃO FÉLIX DO XINGU/PA
            { "1507409", "111700" }, // SÃO FRANCISCO DO PARÁ/PA
            { "1507458", "005500" }, // SÃO GERALDO DO ARAGUAIA/PA
            { "1507466", "106702" }, // SÃO JOÃO DA PONTA/PA
            { "1507474", "106900" }, // SÃO JOÃO DE PIRABAS/PA
            { "1507508", "102900" }, // SÃO JOÃO DO ARAGUAIA/PA
            { "1507607", "111800" }, // SÃO MIGUEL DO GUAMÁ/PA
            { "1507706", "096300" }, // SÃO SEBASTIÃO DA BOA VISTA/PA
            { "1507755", "173300" }, // SAPUCAIA/PA
            { "1507805", "096400" }, // SENADOR JOSÉ PORFÍRIO/PA
            { "1507904", "098200" }, // SOURE/PA
            { "1507953", "003900" }, // TAILÂNDIA/PA
            { "1507961", "067500" }, // TERRA ALTA/PA
            { "1507979", "067600" }, // TERRA SANTA/PA
            { "1508001", "103800" }, // TOMÉ-AÇÚ/PA
            { "1508035", "110806" }, // TRACUATEUA/PA
            { "1508050", "109900" }, // TRAIRÃO/PA
            { "1508084", "094100" }, // TUCUMÃ/PA
            { "1508100", "103000" }, // TUCURUÍ/PA
            { "1508126", "110000" }, // ULIANÓPOLIS/PA
            { "1508159", "008300" }, // URUARÁ/PA
            { "1508209", "106800" }, // VIGIA/PA
            { "1508308", "114400" }, // VISEU/PA
            { "1508357", "110100" }, // VITÓRIA DO XINGU/PA
            { "1508407", "095000" }, // XINGUARA/PA
            { "2500106", "254900" }, // ÁGUA BRANCA/PB
            { "2500205", "245700" }, // AGUIAR/PB
            { "2500304", "256300" }, // ALAGOA GRANDE/PB
            { "2500403", "260900" }, // ALAGOA NOVA/PB
            { "2500502", "256400" }, // ALAGOINHA/PB
            { "2500536", "250501" }, // ALCANTIL/PB
            { "2500577", "173400" }, // ALGODÃO DE JANDAÍRA/PB
            { "2500601", "264100" }, // ALHANDRA/PB
            { "2500734", "252201" }, // AMPARO/PB
            { "2500775", "248901" }, // APARECIDA/PB
            { "2500809", "256500" }, // ARAÇAGI/PB
            { "2500908", "261000" }, // ARARA/PB
            { "2501005", "241100" }, // ARARUNA/PB
            { "2501104", "261100" }, // AREIA/PB
            { "2501153", "215700" }, // AREIA DE BARAÚNAS/PB
            { "2501203", "258800" }, // AREIAL/PB
            { "2501302", "250300" }, // AROEIRAS/PB
            { "2501351", "252301" }, // ASSUNÇÃO/PB
            { "2501401", "264200" }, // BAÍA DA TRAIÇÃO/PB
            { "2501500", "261200" }, // BANANEIRAS/PB
            { "2501534", "238400" }, // BARAÚNA/PB
            { "2501609", "241200" }, // BARRA DE SANTA ROSA/PB
            { "2501575", "191800" }, // BARRA DE SANTANA/PB
            { "2501708", "250400" }, // BARRA DE SÃO MIGUEL/PB
            { "2501807", "264300" }, // BAYEUX/PB
            { "2501906", "256600" }, // BELÉM/PB
            { "2502003", "238500" }, // BELÉM DO BREJO DO CRUZ/PB
            { "2502052", "173600" }, // BERNARDINO BATISTA/PB
            { "2502102", "242400" }, // BOA VENTURA/PB
            { "2502151", "240500" }, // BOA VISTA/PB
            { "2502201", "242500" }, // BOM JESUS/PB
            { "2502300", "238600" }, // BOM SUCESSO/PB
            { "2502409", "242600" }, // BONITO DE SANTA FÉ/PB
            { "2502508", "250500" }, // BOQUEIRÃO/PB
            { "2502706", "261300" }, // BORBOREMA/PB
            { "2502805", "238700" }, // BREJO DO CRUZ/PB
            { "2502904", "238800" }, // BREJO DOS SANTOS/PB
            { "2503001", "264400" }, // CAAPORÃ/PB
            { "2503100", "250600" }, // CABACEIRAS/PB
            { "2503209", "264500" }, // CABEDELO/PB
            { "2503308", "242700" }, // CACHOEIRA DOS ÍNDIOS/PB
            { "2503407", "245900" }, // CACIMBA DE AREIA/PB
            { "2503506", "241300" }, // CACIMBA DE DENTRO/PB
            { "2503555", "173700" }, // CACIMBAS/PB
            { "2503605", "256700" }, // CAIÇARA/PB
            { "2503704", "242800" }, // CAJAZEIRAS/PB
            { "2503753", "215800" }, // CAJAZEIRINHAS/PB
            { "2503803", "262600" }, // CALDAS BRANDÃO/PB
            { "2503902", "250700" }, // CAMALAÚ/PB
            { "2504009", "258900" }, // CAMPINA GRANDE/PB
            { "2516409", "" }, // CAMPO DE SANTANA/PB
            { "2504033", "265201" }, // CAPIM/PB
            { "2504074", "251601" }, // CARAÚBAS/PB
            { "2504108", "242900" }, // CARRAPATEIRA/PB
            { "2504157", "173800" }, // CASSERENGUE/PB
            { "2504207", "246000" }, // CATINGUEIRA/PB
            { "2504306", "238900" }, // CATOLÉ DO ROCHA/PB
            { "2504355", "250503" }, // CATURITÉ/PB
            { "2504405", "243000" }, // CONCEIÇÃO/PB
            { "2504504", "246100" }, // CONDADO/PB
            { "2504603", "264600" }, // CONDE/PB
            { "2504702", "250800" }, // CONGO/PB
            { "2504801", "246200" }, // COREMAS/PB
            { "2504850", "252001" }, // COXIXOLA/PB
            { "2504900", "264700" }, // CRUZ DO ESPÍRITO SANTO/PB
            { "2505006", "239800" }, // CUBATI/PB
            { "2505105", "241400" }, // CUITÉ/PB
            { "2505238", "265202" }, // CUITÉ DE MAMANGUAPE/PB
            { "2505204", "256800" }, // CUITEGI/PB
            { "2505279", "265203" }, // CURRAL DE CIMA/PB
            { "2505303", "243100" }, // CURRAL VELHO/PB
            { "2505352", "173900" }, // DAMIÃO/PB
            { "2505402", "255000" }, // DESTERRO/PB
            { "2505600", "243200" }, // DIAMANTE/PB
            { "2505709", "241500" }, // DONA INÊS/PB
            { "2505808", "256900" }, // DUAS ESTRADAS/PB
            { "2505907", "246400" }, // EMAS/PB
            { "2506004", "259000" }, // ESPERANÇA/PB
            { "2506103", "259100" }, // FAGUNDES/PB
            { "2506202", "239900" }, // FREI MARTINHO/PB
            { "2506251", "250301" }, // GADO BRAVO/PB
            { "2506301", "257000" }, // GUARABIRA/PB
            { "2506400", "257100" }, // GURINHÉM/PB
            { "2506509", "250900" }, // GURJÃO/PB
            { "2506608", "243300" }, // IBIARA/PB
            { "2502607", "245800" }, // IGARACY/PB
            { "2506707", "255100" }, // IMACULADA/PB
            { "2506806", "257200" }, // INGÁ/PB
            { "2506905", "262700" }, // ITABAIANA/PB
            { "2507002", "246500" }, // ITAPORANGA/PB
            { "2507101", "264800" }, // ITAPOROROCA/PB
            { "2507200", "257300" }, // ITATUBA/PB
            { "2507309", "264900" }, // JACARAÚ/PB
            { "2507408", "239000" }, // JERICÓ/PB
            { "2507507", "265000" }, // JOÃO PESSOA/PB
            { "2507606", "257400" }, // JUAREZ TÁVORA/PB
            { "2507705", "240000" }, // JUAZEIRINHO/PB
            { "2507804", "246600" }, // JUNCO DO SERIDÓ/PB
            { "2507903", "262800" }, // JURIPIRANGA/PB
            { "2508000", "255200" }, // JURU/PB
            { "2508109", "246700" }, // LAGOA/PB
            { "2508208", "257500" }, // LAGOA DE DENTRO/PB
            { "2508307", "259200" }, // LAGOA SECA/PB
            { "2508406", "246800" }, // LASTRO/PB
            { "2508505", "251000" }, // LIVRAMENTO/PB
            { "2508554", "216900" }, // LOGRADOURO/PB
            { "2508604", "265100" }, // LUCENA/PB
            { "2508703", "255300" }, // MÃE D'ÁGUA/PB
            { "2508802", "246900" }, // MALTA/PB
            { "2508901", "265200" }, // MAMANGUAPE/PB
            { "2509008", "255400" }, // MANAÍRA/PB
            { "2509057", "265602" }, // MARCAÇÃO/PB
            { "2509107", "262900" }, // MARI/PB
            { "2509156", "248902" }, // MARIZÓPOLIS/PB
            { "2509206", "259300" }, // MASSARANDUBA/PB
            { "2509305", "265300" }, // MATARACA/PB
            { "2509339", "260901" }, // MATINHAS/PB
            { "2509370", "174000" }, // MATO GROSSO/PB
            { "2509396", "174100" }, // MATURÉIA/PB
            { "2509404", "263000" }, // MOGEIRO/PB
            { "2509503", "259400" }, // MONTADAS/PB
            { "2509602", "243400" }, // MONTE HOREBE/PB
            { "2509701", "251100" }, // MONTEIRO/PB
            { "2509800", "257600" }, // MULUNGU/PB
            { "2509909", "251200" }, // NATUBA/PB
            { "2510006", "247000" }, // NAZAREZINHO/PB
            { "2510105", "241600" }, // NOVA FLORESTA/PB
            { "2510204", "247100" }, // NOVA OLINDA/PB
            { "2510303", "240100" }, // NOVA PALMEIRA/PB
            { "2510402", "247200" }, // OLHO D'ÁGUA/PB
            { "2510501", "251300" }, // OLIVEDOS/PB
            { "2510600", "251400" }, // OURO VELHO/PB
            { "2510659", "251801" }, // PARARI/PB
            { "2510709", "247300" }, // PASSAGEM/PB
            { "2510808", "247400" }, // PATOS/PB
            { "2510907", "247500" }, // PAULISTA/PB
            { "2511004", "243500" }, // PEDRA BRANCA/PB
            { "2511103", "240200" }, // PEDRA LAVRADA/PB
            { "2511202", "265400" }, // PEDRAS DE FOGO/PB
            { "2512721", "174200" }, // PEDRO RÉGIS/PB
            { "2511301", "247600" }, // PIANCÓ/PB
            { "2511400", "240300" }, // PICUÍ/PB
            { "2511509", "263100" }, // PILAR/PB
            { "2511608", "261400" }, // PILÕES/PB
            { "2511707", "257700" }, // PILÕEZINHOS/PB
            { "2511806", "261500" }, // PIRPIRITUBA/PB
            { "2511905", "265500" }, // PITIMBU/PB
            { "2512002", "259500" }, // POCINHOS/PB
            { "2512036", "244201" }, // POÇO DANTAS/PB
            { "2512077", "174300" }, // POÇO DE JOSÉ DE MOURA/PB
            { "2512101", "247700" }, // POMBAL/PB
            { "2512200", "251500" }, // PRATA/PB
            { "2512309", "255500" }, // PRINCESA ISABEL/PB
            { "2512408", "259600" }, // PUXINANÃ/PB
            { "2512507", "259700" }, // QUEIMADAS/PB
            { "2512606", "247800" }, // QUIXABÁ/PB
            { "2512705", "259800" }, // REMÍGIO/PB
            { "2512747", "174400" }, // RIACHÃO/PB
            { "2512754", "217200" }, // RIACHÃO DO BACAMARTE/PB
            { "2512762", "174500" }, // RIACHÃO DO POÇO/PB
            { "2512788", "250504" }, // RIACHO DE SANTO ANTÔNIO/PB
            { "2512804", "239100" }, // RIACHO DOS CAVALOS/PB
            { "2512903", "265600" }, // RIO TINTO/PB
            { "2513000", "247900" }, // SALGADINHO/PB
            { "2513109", "263200" }, // SALGADO DE SÃO FÉLIX/PB
            { "2513158", "" }, // SANTA CECÍLIA DE UMBUZEIRO/PB
            { "2513208", "248000" }, // SANTA CRUZ/PB
            { "2513307", "243600" }, // SANTA HELENA/PB
            { "2513356", "174700" }, // SANTA INÊS/PB
            { "2513406", "248100" }, // SANTA LUZIA/PB
            { "2513703", "244205" }, // SANTA RITA/PB
            { "2513802", "248300" }, // SANTA TERESINHA/PB
            { "2513505", "243700" }, // SANTANA DE MANGUEIRA/PB
            { "2513604", "248200" }, // SANTANA DOS GARROTES/PB
            { "2513653", "210300" }, // SANTARÉM/PB
            { "2513851", "176800" }, // SANTO ANDRÉ/PB
            { "2513927", "174800" }, // SÃO BENTINHO/PB
            { "2513901", "239200" }, // SÃO BENTO/PB
            { "2513968", "174900" }, // SÃO DOMINGOS DE POMBAL/PB
            { "2513943", "" }, // SÃO DOMINGOS DO CARIRI/PB
            { "2513984", "248903" }, // SÃO FRANCISCO/PB
            { "2514008", "251600" }, // SÃO JOÃO DO CARIRI/PB
            { "2500700", "023100" }, // SÃO JOÃO DO RIO DO PEIXE/PB
            { "2514107", "251700" }, // SÃO JOÃO DO TIGRE/PB
            { "2514206", "248400" }, // SÃO JOSÉ DA LAGOA TAPADA/PB
            { "2514305", "243800" }, // SÃO JOSÉ DE CAIANA/PB
            { "2514404", "248500" }, // SÃO JOSÉ DE ESPINHARAS/PB
            { "2514503", "243900" }, // SÃO JOSÉ DE PIRANHAS/PB
            { "2514552", "191900" }, // SÃO JOSÉ DE PRINCESA/PB
            { "2514602", "248600" }, // SÃO JOSÉ DO BONFIM/PB
            { "2514651", "210400" }, // SÃO JOSÉ DO BREJO DO CRUZ/PB
            { "2514701", "248700" }, // SÃO JOSÉ DO SABUGI/PB
            { "2514800", "251800" }, // SÃO JOSÉ DOS CORDEIROS/PB
            { "2514453", "175000" }, // SÃO JOSÉ DOS RAMOS/PB
            { "2514909", "248800" }, // SÃO MAMEDE/PB
            { "2515005", "263300" }, // SÃO MIGUEL DE TAIPU/PB
            { "2515104", "261600" }, // SÃO SEBASTIÃO DE LAGOA DE ROÇA/PB
            { "2515203", "251900" }, // SÃO SEBASTIÃO DO UMBUZEIRO/PB
            { "2515401", "" }, // SÃO VICENTE DO SERIDÓ/PB
            { "2515302", "263400" }, // SAPÉ/PB
            { "2515500", "252000" }, // SERRA BRANCA/PB
            { "2515609", "257800" }, // SERRA DA RAIZ/PB
            { "2515708", "244000" }, // SERRA GRANDE/PB
            { "2515807", "257900" }, // SERRA REDONDA/PB
            { "2515906", "261700" }, // SERRARIA/PB
            { "2515930", "256901" }, // SERTÃOZINHO/PB
            { "2515971", "263401" }, // SOBRADO/PB
            { "2516003", "259900" }, // SOLÂNEA/PB
            { "2516102", "252100" }, // SOLEDADE/PB
            { "2516151", "241402" }, // SOSSEGO/PB
            { "2516201", "248900" }, // SOUSA/PB
            { "2516300", "252200" }, // SUMÉ/PB
            { "2516508", "252300" }, // TAPEROÁ/PB
            { "2516607", "255600" }, // TAVARES/PB
            { "2516706", "255700" }, // TEIXEIRA/PB
            { "2516755", "240001" }, // TENÓRIO/PB
            { "2516805", "244100" }, // TRIUNFO/PB
            { "2516904", "244200" }, // UIRAÚNA/PB
            { "2517001", "252400" }, // UMBUZEIRO/PB
            { "2517100", "249000" }, // VÁRZEA/PB
            { "2517209", "248904" }, // VIEIRÓPOLIS/PB
            { "2505501", "246300" }, // VISTA SERRANA/PB
            { "2517407", "251901" }, // ZABELÊ/PB
            { "2600054", "305300" }, // ABREU E LIMA/PE
            { "2600104", "274400" }, // AFOGADOS DA INGAZEIRA/PE
            { "2600203", "271700" }, // AFRÂNIO/PE
            { "2600302", "290200" }, // AGRESTINA/PE
            { "2600401", "299900" }, // ÁGUA PRETA/PE
            { "2600500", "279900" }, // ÁGUAS BELAS/PE
            { "2600609", "285200" }, // ALAGOINHA/PE
            { "2600708", "296200" }, // ALIANÇA/PE
            { "2600807", "290300" }, // ALTINHO/PE
            { "2600906", "300000" }, // AMARAJI/PE
            { "2601003", "290400" }, // ANGELIM/PE
            { "2601052", "176900" }, // ARAÇOIABA/PE
            { "2601102", "267000" }, // ARARIPINA/PE
            { "2601201", "280000" }, // ARCOVERDE/PE
            { "2601300", "290500" }, // BARRA DE GUABIRABA/PE
            { "2601409", "300100" }, // BARREIROS/PE
            { "2601508", "300200" }, // BELÉM DE MARIA/PE
            { "2601607", "" }, // BELÉM DO SÃO FRANCISCO/PE
            { "2601706", "285300" }, // BELO JARDIM/PE
            { "2601805", "278000" }, // BETÂNIA/PE
            { "2601904", "285400" }, // BEZERROS/PE
            { "2602001", "267100" }, // BODOCÓ/PE
            { "2602100", "290600" }, // BOM CONSELHO/PE
            { "2602209", "281500" }, // BOM JARDIM/PE
            { "2602308", "290700" }, // BONITO/PE
            { "2602407", "290800" }, // BREJÃO/PE
            { "2602506", "274500" }, // BREJINHO/PE
            { "2602605", "285500" }, // BREJO DA MADRE DE DEUS/PE
            { "2602704", "296300" }, // BUENOS AIRES/PE
            { "2602803", "280100" }, // BUÍQUE/PE
            { "2602902", "304500" }, // CABO DE SANTO AGOSTINHO/PE
            { "2603009", "271900" }, // CABROBÓ/PE
            { "2603108", "285600" }, // CACHOEIRINHA/PE
            { "2603207", "290900" }, // CAETÉS/PE
            { "2603306", "291000" }, // CALÇADO/PE
            { "2603405", "274600" }, // CALUMBI/PE
            { "2603454", "304600" }, // CAMARAGIBE/PE
            { "2603504", "291100" }, // CAMOCIM DE SÃO FÉLIX/PE
            { "2603603", "296400" }, // CAMUTANGA/PE
            { "2603702", "291200" }, // CANHOTINHO/PE
            { "2603801", "285700" }, // CAPOEIRAS/PE
            { "2603900", "274700" }, // CARNAÍBA/PE
            { "2603926", "115100" }, // CARNAUBEIRA DA PENHA/PE
            { "2604007", "296500" }, // CARPINA/PE
            { "2604106", "285800" }, // CARUARU/PE
            { "2604155", "177000" }, // CASINHAS/PE
            { "2604205", "300300" }, // CATENDE/PE
            { "2604304", "269700" }, // CEDRO/PE
            { "2604403", "296600" }, // CHÃ DE ALEGRIA/PE
            { "2604502", "281600" }, // CHÃ GRANDE/PE
            { "2604601", "296700" }, // CONDADO/PE
            { "2604700", "291300" }, // CORRENTES/PE
            { "2604809", "300400" }, // CORTÊS/PE
            { "2604908", "281700" }, // CUMARU/PE
            { "2605004", "291400" }, // CUPIRA/PE
            { "2605103", "278100" }, // CUSTÓDIA/PE
            { "2605152", "068900" }, // DORMENTES/PE
            { "2605202", "300500" }, // ESCADA/PE
            { "2605301", "267200" }, // EXU/PE
            { "2605400", "281800" }, // FEIRA NOVA/PE
            { "2605459", "118600" }, // FERNANDO DE NORONHA/PE
            { "2605509", "296800" }, // FERREIROS/PE
            { "2605608", "274800" }, // FLORES/PE
            { "2605707", "272000" }, // FLORESTA/PE
            { "2605806", "281900" }, // FREI MIGUELINHO/PE
            { "2605905", "300600" }, // GAMELEIRA/PE
            { "2606002", "291500" }, // GARANHUNS/PE
            { "2606101", "282000" }, // GLÓRIA DO GOITÁ/PE
            { "2606200", "296900" }, // GOIANA/PE
            { "2606309", "267300" }, // GRANITO/PE
            { "2606408", "285900" }, // GRAVATÁ/PE
            { "2606507", "291600" }, // IATI/PE
            { "2606606", "278200" }, // IBIMIRIM/PE
            { "2606705", "291700" }, // IBIRAJUBA/PE
            { "2606804", "297000" }, // IGARASSU/PE
            { "2606903", "274900" }, // IGUARACI/PE
            { "2607604", "" }, // ILHA DE ITAMARACÁ/PE
            { "2607000", "278300" }, // INAJÁ/PE
            { "2607109", "275000" }, // INGAZEIRA/PE
            { "2607208", "300700" }, // IPOJUCA/PE
            { "2607307", "267400" }, // IPUBI/PE
            { "2607406", "272100" }, // ITACURUBA/PE
            { "2607505", "280200" }, // ITAÍBA/PE
            { "2607653", "297200" }, // ITAMBÉ/PE
            { "2607703", "275100" }, // ITAPETIM/PE
            { "2607752", "297300" }, // ITAPISSUMA/PE
            { "2607802", "297400" }, // ITAQUITINGA/PE
            { "2607901", "304700" }, // JABOATÃO DOS GUARARAPES/PE
            { "2607950", "177100" }, // JAQUEIRA/PE
            { "2608008", "286000" }, // JATAÚBA/PE
            { "2608057", "192000" }, // JATOBÁ/PE
            { "2608107", "282100" }, // JOÃO ALFREDO/PE
            { "2608206", "300800" }, // JOAQUIM NABUCO/PE
            { "2608255", "068700" }, // JUCATI/PE
            { "2608305", "291800" }, // JUPI/PE
            { "2608404", "291900" }, // JUREMA/PE
            { "2608453", "068400" }, // LAGOA DO CARRO/PE
            { "2608503", "297500" }, // LAGOA DO ITAENGA/PE
            { "2608602", "292000" }, // LAGOA DO OURO/PE
            { "2608701", "292100" }, // LAGOA DOS GATOS/PE
            { "2608750", "192100" }, // LAGOA GRANDE/PE
            { "2608800", "292200" }, // LAJEDO/PE
            { "2608909", "282200" }, // LIMOEIRO/PE
            { "2609006", "297600" }, // MACAPARANA/PE
            { "2609105", "282300" }, // MACHADOS/PE
            { "2609154", "177200" }, // MANARI/PE
            { "2609204", "300900" }, // MARAIAL/PE
            { "2609303", "269800" }, // MIRANDIBA/PE
            { "2614303", "267600" }, // MOREILÂNDIA/PE
            { "2609402", "304800" }, // MORENO/PE
            { "2609501", "297700" }, // NAZARÉ DA MATA/PE
            { "2609600", "304900" }, // OLINDA/PE
            { "2609709", "282400" }, // OROBÓ/PE
            { "2609808", "272200" }, // OROCÓ/PE
            { "2609907", "267500" }, // OURICURI/PE
            { "2610004", "301000" }, // PALMARES/PE
            { "2610103", "292300" }, // PALMEIRINA/PE
            { "2610202", "292400" }, // PANELAS/PE
            { "2610301", "292500" }, // PARANATAMA/PE
            { "2610400", "269900" }, // PARNAMIRIM/PE
            { "2610509", "282500" }, // PASSIRA/PE
            { "2610608", "297800" }, // PAUDALHO/PE
            { "2610707", "305000" }, // PAULISTA/PE
            { "2610806", "280300" }, // PEDRA/PE
            { "2610905", "286100" }, // PESQUEIRA/PE
            { "2611002", "272300" }, // PETROLÂNDIA/PE
            { "2611101", "272400" }, // PETROLINA/PE
            { "2611200", "286200" }, // POÇÃO/PE
            { "2611309", "282600" }, // POMBOS/PE
            { "2611408", "301100" }, // PRIMAVERA/PE
            { "2611507", "301200" }, // QUIPAPÁ/PE
            { "2611533", "069000" }, // QUIXABÁ/PE
            { "2611606", "305100" }, // RECIFE/PE
            { "2611705", "286300" }, // RIACHO DAS ALMAS/PE
            { "2611804", "301300" }, // RIBEIRÃO/PE
            { "2611903", "301400" }, // RIO FORMOSO/PE
            { "2612000", "292600" }, // SAIRÉ/PE
            { "2612109", "282700" }, // SALGADINHO/PE
            { "2612208", "270000" }, // SALGUEIRO/PE
            { "2612307", "292700" }, // SALOÁ/PE
            { "2612406", "286400" }, // SANHARÓ/PE
            { "2612455", "068800" }, // SANTA CRUZ/PE
            { "2612471", "069100" }, // SANTA CRUZ DA BAIXA VERDE/PE
            { "2612505", "286500" }, // SANTA CRUZ DO CAPIBARIBE/PE
            { "2612554", "177300" }, // SANTA FILOMENA/PE
            { "2612604", "272500" }, // SANTA MARIA DA BOA VISTA/PE
            { "2612703", "282800" }, // SANTA MARIA DO CAMBUCÁ/PE
            { "2612802", "275200" }, // SANTA TEREZINHA/PE
            { "2612901", "301500" }, // SÃO BENEDITO DO SUL/PE
            { "2613008", "286600" }, // SÃO BENTO DO UNA/PE
            { "2613107", "286700" }, // SÃO CAITANO/PE
            { "2613206", "292800" }, // SÃO JOÃO/PE
            { "2613305", "292900" }, // SÃO JOAQUIM DO MONTE/PE
            { "2613404", "301600" }, // SÃO JOSÉ DA COROA GRANDE/PE
            { "2613503", "270100" }, // SÃO JOSÉ DO BELMONTE/PE
            { "2613602", "275300" }, // SÃO JOSÉ DO EGITO/PE
            { "2613701", "305200" }, // SÃO LOURENÇO DA MATA/PE
            { "2613800", "282900" }, // SÃO VICENTE FERRER/PE
            { "2613909", "275400" }, // SERRA TALHADA/PE
            { "2614006", "270200" }, // SERRITA/PE
            { "2614105", "278400" }, // SERTÂNIA/PE
            { "2614204", "301700" }, // SIRINHAÉM/PE
            { "2614402", "275500" }, // SOLIDÃO/PE
            { "2614501", "283000" }, // SURUBIM/PE
            { "2614600", "275600" }, // TABIRA/PE
            { "2614709", "286800" }, // TACAIMBÓ/PE
            { "2614808", "278500" }, // TACARATU/PE
            { "2614857", "177400" }, // TAMANDARÉ/PE
            { "2615003", "283100" }, // TAQUARITINGA DO NORTE/PE
            { "2615102", "293000" }, // TEREZINHA/PE
            { "2615201", "270300" }, // TERRA NOVA/PE
            { "2615300", "297900" }, // TIMBAÚBA/PE
            { "2615409", "283200" }, // TORITAMA/PE
            { "2615508", "298000" }, // TRACUNHAÉM/PE
            { "2615607", "267700" }, // TRINDADE/PE
            { "2615706", "275700" }, // TRIUNFO/PE
            { "2615805", "280400" }, // TUPANATINGA/PE
            { "2615904", "275800" }, // TUPARETAMA/PE
            { "2616001", "280500" }, // VENTUROSA/PE
            { "2616100", "270400" }, // VERDEJANTE/PE
            { "2616183", "068500" }, // VERTENTE DO LÉRIO/PE
            { "2616209", "283300" }, // VERTENTES/PE
            { "2616308", "298100" }, // VICÊNCIA/PE
            { "2616407", "301800" }, // VITÓRIA DE SANTO ANTÃO/PE
            { "2616506", "068600" }, // XEXÉU/PE
            { "2200053", "192200" }, // ACAUÃ/PI
            { "2200103", "143600" }, // AGRICOLÂNDIA/PI
            { "2200202", "143700" }, // ÁGUA BRANCA/PI
            { "2200251", "018100" }, // ALAGOINHA DO PIAUÍ/PI
            { "2200277", "111900" }, // ALEGRETE DO PIAUÍ/PI
            { "2200301", "141900" }, // ALTO LONGÁ/PI
            { "2200400", "148600" }, // ALTOS/PI
            { "2200459", "210500" }, // ALVORADA DO GURGUÉIA/PI
            { "2200509", "143800" }, // AMARANTE/PI
            { "2200608", "143900" }, // ANGICAL DO PIAUÍ/PI
            { "2200707", "153000" }, // ANÍSIO DE ABREU/PI
            { "2200806", "149700" }, // ANTÔNIO ALMEIDA/PI
            { "2200905", "145200" }, // AROAZES/PI
            { "2201002", "144000" }, // ARRAIAL/PI
            { "2201051", "192300" }, // ASSUNÇÃO DO PIAUÍ/PI
            { "2201101", "154700" }, // AVELINO LOPES/PI
            { "2201150", "112000" }, // BAIXA GRANDE DO RIBEIRO/PI
            { "2201176", "192400" }, // BARRA D'ALCÂNTARA/PI
            { "2201200", "142000" }, // BARRAS/PI
            { "2201309", "154800" }, // BARREIRAS DO PIAUÍ/PI
            { "2201408", "144100" }, // BARRO DURO/PI
            { "2201507", "142100" }, // BATALHA/PI
            { "2201556", "193200" }, // BELA VISTA DO PIAUÍ/PI
            { "2201572", "193300" }, // BELÉM DO PIAUÍ/PI
            { "2201606", "148700" }, // BENEDITINOS/PI
            { "2201705", "149800" }, // BERTOLÍNIA/PI
            { "2201739", "194000" }, // BETÂNIA DO PIAUÍ/PI
            { "2201770", "193400" }, // BOA HORA/PI
            { "2201804", "146500" }, // BOCAINA/PI
            { "2201903", "152200" }, // BOM JESUS/PI
            { "2201919", "112100" }, // BOM PRINCÍPIO DO PIAUÍ/PI
            { "2201929", "112200" }, // BONFIM DO PIAUÍ/PI
            { "2201945", "193500" }, // BOQUEIRÃO DO PIAUÍ/PI
            { "2201960", "112300" }, // BRASILEIRA/PI
            { "2201988", "193600" }, // BREJO DO PIAUÍ/PI
            { "2202000", "140700" }, // BURITI DOS LOPES/PI
            { "2202026", "112400" }, // BURITI DOS MONTES/PI
            { "2202059", "112500" }, // CABECEIRAS DO PIAUÍ/PI
            { "2202075", "211500" }, // CAJAZEIRAS DO PIAUÍ/PI
            { "2202083", "193700" }, // CAJUEIRO DA PRAIA/PI
            { "2202091", "112600" }, // CALDEIRÃO GRANDE DO PIAUÍ/PI
            { "2202109", "153100" }, // CAMPINAS DO PIAUÍ/PI
            { "2202117", "211600" }, // CAMPO ALEGRE DO FIDALGO/PI
            { "2202133", "193800" }, // CAMPO GRANDE DO PIAUÍ/PI
            { "2202174", "193900" }, // CAMPO LARGO DO PIAUÍ/PI
            { "2202208", "142200" }, // CAMPO MAIOR/PI
            { "2202251", "068300" }, // CANAVIEIRA/PI
            { "2202307", "153200" }, // CANTO DO BURITI/PI
            { "2202406", "142300" }, // CAPITÃO DE CAMPOS/PI
            { "2202455", "211700" }, // CAPITÃO GERVÁSIO OLIVEIRA/PI
            { "2202505", "153300" }, // CARACOL/PI
            { "2202539", "194100" }, // CARAÚBAS DO PIAUÍ/PI
            { "2202554", "194200" }, // CARIDADE DO PIAUÍ/PI
            { "2202604", "142400" }, // CASTELO DO PIAUÍ/PI
            { "2202653", "194300" }, // CAXINGÓ/PI
            { "2202703", "142500" }, // COCAL/PI
            { "2202711", "194400" }, // COCAL DE TELHA/PI
            { "2202729", "194500" }, // COCAL DOS ALVES/PI
            { "2202737", "112700" }, // COIVARAS/PI
            { "2202752", "112800" }, // COLÔNIA DO GURGUÉIA/PI
            { "2202778", "112900" }, // COLÔNIA DO PIAUÍ/PI
            { "2202802", "153400" }, // CONCEIÇÃO DO CANINDÉ/PI
            { "2202851", "113000" }, // CORONEL JOSÉ DIAS/PI
            { "2202901", "154900" }, // CORRENTE/PI
            { "2203008", "155000" }, // CRISTALÂNDIA DO PIAUÍ/PI
            { "2203107", "152300" }, // CRISTINO CASTRO/PI
            { "2203206", "155100" }, // CURIMATÁ/PI
            { "2203230", "194600" }, // CURRAIS/PI
            { "2203271", "194700" }, // CURRAL NOVO DO PIAUÍ/PI
            { "2203255", "194800" }, // CURRALINHOS/PI
            { "2203305", "148800" }, // DEMERVAL LOBÃO/PI
            { "2203354", "153500" }, // DIRCEU ARCOVERDE/PI
            { "2203404", "146600" }, // DOM EXPEDITO LOPES/PI
            { "2203453", "018200" }, // DOM INOCÊNCIO/PI
            { "2203420", "142600" }, // DOMINGOS MOURÃO/PI
            { "2203503", "145300" }, // ELESBÃO VELOSO/PI
            { "2203602", "149900" }, // ELISEU MARTINS/PI
            { "2203701", "140800" }, // ESPERANTINA/PI
            { "2203750", "113100" }, // FARTURA DO PIAUÍ/PI
            { "2203800", "150000" }, // FLORES DO PIAUÍ/PI
            { "2203859", "194900" }, // FLORESTA DO PIAUÍ/PI
            { "2203909", "150100" }, // FLORIANO/PI
            { "2204006", "145400" }, // FRANCINÓPOLIS/PI
            { "2204105", "144200" }, // FRANCISCO AYRES/PI
            { "2204154", "175200" }, // FRANCISCO MACEDO/PI
            { "2204204", "146700" }, // FRANCISCO SANTOS/PI
            { "2204303", "146800" }, // FRONTEIRAS/PI
            { "2204352", "195000" }, // GEMINIANO/PI
            { "2204402", "155200" }, // GILBUÉS/PI
            { "2204501", "150200" }, // GUADALUPE/PI
            { "2204550", "195100" }, // GUARIBAS/PI
            { "2204600", "144300" }, // HUGO NAPOLEÃO/PI
            { "2204659", "195200" }, // ILHA GRANDE/PI
            { "2204709", "145500" }, // INHUMA/PI
            { "2204808", "146900" }, // IPIRANGA DO PIAUÍ/PI
            { "2204907", "153600" }, // ISAÍAS COELHO/PI
            { "2205003", "147000" }, // ITAINÓPOLIS/PI
            { "2205102", "150300" }, // ITAUEIRA/PI
            { "2205151", "113200" }, // JACOBINA DO PIAUÍ/PI
            { "2205201", "147100" }, // JAICÓS/PI
            { "2205250", "113300" }, // JARDIM DO MULATO/PI
            { "2205276", "195300" }, // JATOBÁ DO PIAUÍ/PI
            { "2205300", "150400" }, // JERUMENHA/PI
            { "2205359", "195400" }, // JOÃO COSTA/PI
            { "2205409", "140900" }, // JOAQUIM PIRES/PI
            { "2205458", "195500" }, // JOCA MARQUES/PI
            { "2205508", "148900" }, // JOSÉ DE FREITAS/PI
            { "2205516", "195600" }, // JUAZEIRO DO PIAUÍ/PI
            { "2205524", "195700" }, // JÚLIO BORGES/PI
            { "2205532", "195800" }, // JUREMA/PI
            { "2205557", "113400" }, // LAGOA ALEGRE/PI
            { "2205573", "211800" }, // LAGOA DE SÃO FRANCISCO/PI
            { "2205565", "113500" }, // LAGOA DO BARRO DO PIAUÍ/PI
            { "2205581", "211900" }, // LAGOA DO PIAUÍ/PI
            { "2205599", "196500" }, // LAGOA DO SÍTIO/PI
            { "2205540", "196600" }, // LAGOINHA DO PIAUÍ/PI
            { "2205607", "150500" }, // LANDRI SALES/PI
            { "2205706", "141000" }, // LUÍS CORREIA/PI
            { "2205805", "141100" }, // LUZILÂNDIA/PI
            { "2205854", "196700" }, // MADEIRO/PI
            { "2205904", "150600" }, // MANOEL EMÍDIO/PI
            { "2205953", "113600" }, // MARCOLÂNDIA/PI
            { "2206001", "150700" }, // MARCOS PARENTE/PI
            { "2206050", "196800" }, // MASSAPÊ DO PIAUÍ/PI
            { "2206100", "141200" }, // MATIAS OLÍMPIO/PI
            { "2206209", "149000" }, // MIGUEL ALVES/PI
            { "2206308", "144400" }, // MIGUEL LEÃO/PI
            { "2206357", "196900" }, // MILTON BRANDÃO/PI
            { "2206407", "149100" }, // MONSENHOR GIL/PI
            { "2206506", "147200" }, // MONSENHOR HIPÓLITO/PI
            { "2206605", "155300" }, // MONTE ALEGRE DO PIAUÍ/PI
            { "2206654", "197000" }, // MORRO CABEÇA NO TEMPO/PI
            { "2206670", "197100" }, // MORRO DO CHAPÉU DO PIAUÍ/PI
            { "2206696", "212000" }, // MURICI DOS PORTELAS/PI
            { "2206704", "150800" }, // NAZARÉ DO PIAUÍ/PI
            { "2206753", "197200" }, // NOSSA SENHORA DE NAZARÉ/PI
            { "2206803", "141300" }, // NOSSA SENHORA DOS REMÉDIOS/PI
            { "2206902", "145600" }, // NOVO ORIENTE DO PIAUÍ/PI
            { "2206951", "197300" }, // NOVO SANTO ANTÔNIO/PI
            { "2207009", "147300" }, // OEIRAS/PI
            { "2207108", "212100" }, // OLHO D'ÁGUA DO PIAUÍ/PI
            { "2207207", "147400" }, // PADRE MARCOS/PI
            { "2207306", "153700" }, // PAES LANDIM/PI
            { "2207355", "197400" }, // PAJEÚ DO PIAUÍ/PI
            { "2207405", "152400" }, // PALMEIRA DO PIAUÍ/PI
            { "2207504", "144500" }, // PALMEIRAIS/PI
            { "2207553", "197500" }, // PAQUETÁ/PI
            { "2207603", "155400" }, // PARNAGUÁ/PI
            { "2207702", "141400" }, // PARNAÍBA/PI
            { "2207751", "113700" }, // PASSAGEM FRANCA DO PIAUÍ/PI
            { "2207777", "113800" }, // PATOS DO PIAUÍ/PI
            { "2207793", "230800" }, // PAU D'ARCO DO PIAUÍ/PI
            { "2207801", "153800" }, // PAULISTANA/PI
            { "2207850", "197600" }, // PAVUSSU/PI
            { "2207900", "142700" }, // PEDRO II/PI
            { "2207934", "197700" }, // PEDRO LAURENTINO/PI
            { "2208007", "147500" }, // PICOS/PI
            { "2208106", "145700" }, // PIMENTEIRAS/PI
            { "2208205", "147600" }, // PIO IX/PI
            { "2208304", "142800" }, // PIRACURUCA/PI
            { "2208403", "142900" }, // PIRIPIRI/PI
            { "2208502", "141500" }, // PORTO/PI
            { "2208551", "212200" }, // PORTO ALEGRE DO PIAUÍ/PI
            { "2208601", "145800" }, // PRATA DO PIAUÍ/PI
            { "2208650", "113900" }, // QUEIMADA NOVA/PI
            { "2208700", "152500" }, // REDENÇÃO DO GURGUÉIA/PI
            { "2208809", "144600" }, // REGENERAÇÃO/PI
            { "2208858", "197800" }, // RIACHO FRIO/PI
            { "2208874", "212300" }, // RIBEIRA DO PIAUÍ/PI
            { "2208908", "151600" }, // RIBEIRO GONÇALVES/PI
            { "2209005", "150900" }, // RIO GRANDE DO PIAUÍ/PI
            { "2209104", "147700" }, // SANTA CRUZ DO PIAUÍ/PI
            { "2209153", "114000" }, // SANTA CRUZ DOS MILAGRES/PI
            { "2209203", "151700" }, // SANTA FILOMENA/PI
            { "2209302", "152600" }, // SANTA LUZ/PI
            { "2209377", "114200" }, // SANTA ROSA DO PIAUÍ/PI
            { "2209351", "114100" }, // SANTANA DO PIAUÍ/PI
            { "2209401", "147800" }, // SANTO ANTÔNIO DE LISBOA/PI
            { "2209450", "197900" }, // SANTO ANTÔNIO DOS MILAGRES/PI
            { "2209500", "147900" }, // SANTO INÁCIO DO PIAUÍ/PI
            { "2209559", "114300" }, // SÃO BRAZ DO PIAUÍ/PI
            { "2209609", "145900" }, // SÃO FÉLIX DO PIAUÍ/PI
            { "2209658", "198000" }, // SÃO FRANCISCO DE ASSIS DO PIAUÍ/PI
            { "2209708", "151000" }, // SÃO FRANCISCO DO PIAUÍ/PI
            { "2209757", "198100" }, // SÃO GONÇALO DO GURGUÉIA/PI
            { "2209807", "144700" }, // SÃO GONÇALO DO PIAUÍ/PI
            { "2209856", "018000" }, // SÃO JOÃO DA CANABRAVA/PI
            { "2209872", "198200" }, // SÃO JOÃO DA FRONTEIRA/PI
            { "2209906", "143000" }, // SÃO JOÃO DA SERRA/PI
            { "2209955", "198300" }, // SÃO JOÃO DA VARJOTA/PI
            { "2209971", "198400" }, // SÃO JOÃO DO ARRAIAL/PI
            { "2210003", "153900" }, // SÃO JOÃO DO PIAUÍ/PI
            { "2210052", "114500" }, // SÃO JOSÉ DO DIVINO/PI
            { "2210102", "151100" }, // SÃO JOSÉ DO PEIXE/PI
            { "2210201", "148000" }, // SÃO JOSÉ DO PIAUÍ/PI
            { "2210300", "148100" }, // SÃO JULIÃO/PI
            { "2210359", "114600" }, // SÃO LOURENÇO DO PIAUÍ/PI
            { "2210375", "198500" }, // SÃO LUÍS DO PIAUÍ/PI
            { "2210383", "198900" }, // SÃO MIGUEL DA BAIXA GRANDE/PI
            { "2210391", "212400" }, // SÃO MIGUEL DO FIDALGO/PI
            { "2210409", "143100" }, // SÃO MIGUEL DO TAPUIO/PI
            { "2210508", "144800" }, // SÃO PEDRO DO PIAUÍ/PI
            { "2210607", "154000" }, // SÃO RAIMUNDO NONATO/PI
            { "2210623", "199000" }, // SEBASTIÃO BARROS/PI
            { "2210631", "212500" }, // SEBASTIÃO LEAL/PI
            { "2210656", "114700" }, // SIGEFREDO PACHECO/PI
            { "2210706", "148200" }, // SIMÕES/PI
            { "2210805", "154100" }, // SIMPLÍCIO MENDES/PI
            { "2210904", "154200" }, // SOCORRO DO PIAUÍ/PI
            { "2210938", "199100" }, // SUSSUAPARA/PI
            { "2210953", "199200" }, // TAMBORIL DO PIAUÍ/PI
            { "2210979", "212600" }, // TANQUE DO PIAUÍ/PI
            { "2211001", "149200" }, // TERESINA/PI
            { "2211100", "149300" }, // UNIÃO/PI
            { "2211209", "151800" }, // URUÇUÍ/PI
            { "2211308", "146000" }, // VALENÇA DO PIAUÍ/PI
            { "2211357", "114800" }, // VÁRZEA BRANCA/PI
            { "2211407", "146100" }, // VÁRZEA GRANDE/PI
            { "2211506", "199300" }, // VERA MENDES/PI
            { "2211605", "199400" }, // VILA NOVA DO PIAUÍ/PI
            { "2211704", "199500" }, // WALL FERRAZ/PI
            { "4120655", "" }, // 4º CENTENÁRIO/PR
            { "4100103", "777100" }, // ABATIÁ/PR
            { "4100202", "732700" }, // ADRIANÓPOLIS/PR
            { "4100301", "733400" }, // AGUDOS DO SUL/PR
            { "4100400", "726900" }, // ALMIRANTE TAMANDARÉ/PR
            { "4100459", "754700" }, // ALTAMIRA DO PARANÁ/PR
            { "4128625", "" }, // ALTO PARAÍSO/PR
            { "4100608", "790300" }, // ALTO PARANÁ/PR
            { "4100707", "799800" }, // ALTO PIQUIRI/PR
            { "4100509", "799700" }, // ALTÔNIA/PR
            { "4100806", "782000" }, // ALVORADA DO SUL/PR
            { "4100905", "790400" }, // AMAPORÃ/PR
            { "4101002", "764800" }, // AMPÉRE/PR
            { "4101051", "082800" }, // ANAHY/PR
            { "4101101", "777200" }, // ANDIRÁ/PR
            { "4101150", "212700" }, // ÂNGULO/PR
            { "4101200", "731100" }, // ANTONINA/PR
            { "4101309", "738400" }, // ANTÔNIO OLINTO/PR
            { "4101408", "795300" }, // APUCARANA/PR
            { "4101507", "782100" }, // ARAPONGAS/PR
            { "4101606", "737400" }, // ARAPOTI/PR
            { "4101655", "177500" }, // ARAPUÃ/PR
            { "4101705", "749500" }, // ARARUNA/PR
            { "4101804", "727000" }, // ARAUCÁRIA/PR
            { "4101853", "199600" }, // ARIRANHA DO IVAÍ/PR
            { "4101903", "780400" }, // ASSAÍ/PR
            { "4102000", "756200" }, // ASSIS CHATEAUBRIAND/PR
            { "4102109", "782200" }, // ASTORGA/PR
            { "4102208", "787500" }, // ATALAIA/PR
            { "4102307", "727100" }, // BALSA NOVA/PR
            { "4102406", "777300" }, // BANDEIRANTES/PR
            { "4102505", "749600" }, // BARBOSA FERRAZ/PR
            { "4102703", "777400" }, // BARRA DO JACARÉ/PR
            { "4102604", "764900" }, // BARRACÃO/PR
            { "4102752", "" }, // BELA VISTA DA CAROBA/PR
            { "4102802", "782300" }, // BELA VISTA DO PARAÍSO/PR
            { "4102901", "745000" }, // BITURUNA/PR
            { "4103008", "749700" }, // BOA ESPERANÇA/PR
            { "4103024", "083500" }, // BOA ESPERANÇA DO IGUAÇU/PR
            { "4103040", "200200" }, // BOA VENTURA DE SÃO ROQUE/PR
            { "4103057", "756300" }, // BOA VISTA DA APARECIDA/PR
            { "4103107", "727200" }, // BOCAIÚVA DO SUL/PR
            { "4103156", "200300" }, // BOM JESUS DO SUL/PR
            { "4103206", "773419" }, // BOM SUCESSO/PR
            { "4103222", "053700" }, // BOM SUCESSO DO SUL/PR
            { "4103305", "795500" }, // BORRAZÓPOLIS/PR
            { "4103354", "756400" }, // BRAGANEY/PR
            { "4103370", "135000" }, // BRASILÂNDIA DO SUL/PR
            { "4103404", "782400" }, // CAFEARA/PR
            { "4103453", "759000" }, // CAFELÂNDIA/PR
            { "4103479", "135100" }, // CAFEZAL DO SUL/PR
            { "4103503", "795600" }, // CALIFÓRNIA/PR
            { "4103602", "777500" }, // CAMBARÁ/PR
            { "4103701", "782500" }, // CAMBÉ/PR
            { "4103800", "795700" }, // CAMBIRA/PR
            { "4103909", "749800" }, // CAMPINA DA LAGOA/PR
            { "4103958", "177600" }, // CAMPINA DO SIMÃO/PR
            { "4104006", "727300" }, // CAMPINA GRANDE DO SUL/PR
            { "4104055", "757303" }, // CAMPO BONITO/PR
            { "4104105", "734400" }, // CAMPO DO TENENTE/PR
            { "4104204", "727400" }, // CAMPO LARGO/PR
            { "4104253", "178200" }, // CAMPO MAGRO/PR
            { "4104303", "749900" }, // CAMPO MOURÃO/PR
            { "4104402", "747800" }, // CÂNDIDO DE ABREU/PR
            { "4104428", "084300" }, // CANDÓI/PR
            { "4104451", "773300" }, // CANTAGALO/PR
            { "4104501", "765000" }, // CAPANEMA/PR
            { "4104600", "756500" }, // CAPITÃO LEÔNIDAS MARQUES/PR
            { "4104659", "178300" }, // CARAMBEÍ/PR
            { "4104709", "741700" }, // CARLÓPOLIS/PR
            { "4104808", "756600" }, // CASCAVEL/PR
            { "4104907", "735500" }, // CASTRO/PR
            { "4105003", "735513" }, // CATANDUVAS/PR
            { "4105102", "782600" }, // CENTENÁRIO DO SUL/PR
            { "4105201", "732800" }, // CERRO AZUL/PR
            { "4105300", "756800" }, // CÉU AZUL/PR
            { "4105409", "765100" }, // CHOPINZINHO/PR
            { "4105508", "799900" }, // CIANORTE/PR
            { "4105607", "800000" }, // CIDADE GAÚCHA/PR
            { "4105706", "745100" }, // CLEVELÂNDIA/PR
            { "4105805", "727500" }, // COLOMBO/PR
            { "4105904", "782700" }, // COLORADO/PR
            { "4106001", "777600" }, // CONGONHINHAS/PR
            { "4106100", "741800" }, // CONSELHEIRO MAIRINCK/PR
            { "4106209", "727600" }, // CONTENDA/PR
            { "4106308", "756900" }, // CORBÉLIA/PR
            { "4106407", "777700" }, // CORNÉLIO PROCÓPIO/PR
            { "4106456", "178400" }, // CORONEL DOMINGOS SOARES/PR
            { "4106506", "765200" }, // CORONEL VIVIDA/PR
            { "4106555", "751400" }, // CORUMBATAÍ DO SUL/PR
            { "4106803", "745200" }, // CRUZ MACHADO/PR
            { "4106571", "083600" }, // CRUZEIRO DO IGUAÇU/PR
            { "4106605", "800100" }, // CRUZEIRO DO OESTE/PR
            { "4106704", "790500" }, // CRUZEIRO DO SUL/PR
            { "4106852", "178500" }, // CRUZMALTINA/PR
            { "4106902", "727700" }, // CURITIBA/PR
            { "4107009", "741900" }, // CURIÚVA/PR
            { "4107108", "790600" }, // DIAMANTE DO NORTE/PR
            { "4107124", "135200" }, // DIAMANTE DO SUL/PR
            { "4107157", "759100" }, // DIAMANTE D'OESTE/PR
            { "4107207", "765300" }, // DOIS VIZINHOS/PR
            { "4107256", "800200" }, // DOURADINA/PR
            { "4107306", "787600" }, // DOUTOR CAMARGO/PR
            { "4128633", "212800" }, // DOUTOR ULYSSES/PR
            { "4107405", "765400" }, // ENÉAS MARQUES/PR
            { "4107504", "750000" }, // ENGENHEIRO BELTRÃO/PR
            { "4107538", "135300" }, // ENTRE RIOS DO OESTE/PR
            { "4107520", "200400" }, // ESPERANÇA NOVA/PR
            { "4107546", "200500" }, // ESPIGÃO ALTO DO IGUAÇU/PR
            { "4107553", "084600" }, // FAROL/PR
            { "4107603", "795800" }, // FAXINAL/PR
            { "4107652", "053500" }, // FAZENDA RIO GRANDE/PR
            { "4107702", "750100" }, // FÊNIX/PR
            { "4107736", "178600" }, // FERNANDES PINHEIRO/PR
            { "4107751", "742000" }, // FIGUEIRA/PR
            { "4107850", "215900" }, // FLOR DA SERRA DO SUL/PR
            { "4107801", "787700" }, // FLORAÍ/PR
            { "4107900", "787800" }, // FLORESTA/PR
            { "4108007", "782800" }, // FLORESTÓPOLIS/PR
            { "4108106", "782900" }, // FLÓRIDA/PR
            { "4108205", "757000" }, // FORMOSA DO OESTE/PR
            { "4108304", "757100" }, // FOZ DO IGUAÇU/PR
            { "4108452", "200600" }, // FOZ DO JORDÃO/PR
            { "4108320", "800300" }, // FRANCISCO ALVES/PR
            { "4108403", "765500" }, // FRANCISCO BELTRÃO/PR
            { "4108502", "745300" }, // GENERAL CARNEIRO/PR
            { "4108551", "796802" }, // GODOY MOREIRA/PR
            { "4108601", "750200" }, // GOIOERÊ/PR
            { "4108650", "178700" }, // GOIOXIM/PR
            { "4108700", "795900" }, // GRANDES RIOS/PR
            { "4108809", "757200" }, // GUAÍRA/PR
            { "4108908", "783613" }, // GUAIRAÇÁ/PR
            { "4108957", "178800" }, // GUAMIRANGA/PR
            { "4109005", "742100" }, // GUAPIRAMA/PR
            { "4109104", "800400" }, // GUAPOREMA/PR
            { "4109203", "783000" }, // GUARACI/PR
            { "4109302", "757300" }, // GUARANIAÇU/PR
            { "4109401", "773400" }, // GUARAPUAVA/PR
            { "4109500", "731200" }, // GUARAQUEÇABA/PR
            { "4109609", "731300" }, // GUARATUBA/PR
            { "4109658", "136200" }, // HONÓRIO SERPA/PR
            { "4109708", "742200" }, // IBAITI/PR
            { "4109757", "756701" }, // IBEMA/PR
            { "4109807", "783100" }, // IBIPORÃ/PR
            { "4109906", "800500" }, // ICARAÍMA/PR
            { "4110003", "783200" }, // IGUARAÇU/PR
            { "4110052", "083300" }, // IGUATU/PR
            { "4110078", "178900" }, // IMBAÚ/PR
            { "4110102", "739400" }, // IMBITUVA/PR
            { "4110201", "773500" }, // INÁCIO MARTINS/PR
            { "4110300", "790800" }, // INAJÁ/PR
            { "4110409", "800600" }, // INDIANÓPOLIS/PR
            { "4110508", "727006" }, // IPIRANGA/PR
            { "4110607", "800700" }, // IPORÃ/PR
            { "4110656", "137000" }, // IRACEMA DO OESTE/PR
            { "4110706", "739500" }, // IRATI/PR
            { "4110805", "750300" }, // IRETAMA/PR
            { "4110904", "783300" }, // ITAGUAJÉ/PR
            { "4110953", "137100" }, // ITAIPULÂNDIA/PR
            { "4111001", "777800" }, // ITAMBARACÁ/PR
            { "4111100", "727411" }, // ITAMBÉ/PR
            { "4111209", "765600" }, // ITAPEJARA D'OESTE/PR
            { "4111258", "137200" }, // ITAPERUÇU/PR
            { "4111308", "790900" }, // ITAÚNA DO SUL/PR
            { "4111407", "748000" }, // IVAÍ/PR
            { "4111506", "796000" }, // IVAIPORÃ/PR
            { "4111555", "136400" }, // IVATÉ/PR
            { "4111605", "788000" }, // IVATUBA/PR
            { "4111704", "742300" }, // JABOTI/PR
            { "4111803", "777900" }, // JACAREZINHO/PR
            { "4111902", "783400" }, // JAGUAPITÃ/PR
            { "4112009", "737500" }, // JAGUARIAÍVA/PR
            { "4112108", "796100" }, // JANDAIA DO SUL/PR
            { "4112207", "750400" }, // JANIÓPOLIS/PR
            { "4112306", "742400" }, // JAPIRA/PR
            { "4112405", "800800" }, // JAPURÁ/PR
            { "4112504", "796200" }, // JARDIM ALEGRE/PR
            { "4112603", "791000" }, // JARDIM OLINDA/PR
            { "4112702", "780500" }, // JATAIZINHO/PR
            { "4112751", "757400" }, // JESUÍTAS/PR
            { "4112801", "742500" }, // JOAQUIM TÁVORA/PR
            { "4112900", "778000" }, // JUNDIAÍ DO SUL/PR
            { "4112959", "750500" }, // JURANDA/PR
            { "4113007", "800900" }, // JUSSARA/PR
            { "4113106", "796300" }, // KALORÉ/PR
            { "4113205", "734500" }, // LAPA/PR
            { "4113254", "084400" }, // LARANJAL/PR
            { "4113304", "773600" }, // LARANJEIRAS DO SUL/PR
            { "4113403", "778100" }, // LEÓPOLIS/PR
            { "4113429", "084500" }, // LIDIANÓPOLIS/PR
            { "4113452", "756604" }, // LINDOESTE/PR
            { "4113502", "791100" }, // LOANDA/PR
            { "4113601", "783500" }, // LOBATO/PR
            { "4113700", "783600" }, // LONDRINA/PR
            { "4113734", "751500" }, // LUIZIANA/PR
            { "4113759", "796400" }, // LUNARDELLI/PR
            { "4113809", "783700" }, // LUPIONÓPOLIS/PR
            { "4113908", "739600" }, // MALLET/PR
            { "4114005", "750600" }, // MAMBORÊ/PR
            { "4114104", "788100" }, // MANDAGUAÇU/PR
            { "4114203", "788200" }, // MANDAGUARI/PR
            { "4114302", "727800" }, // MANDIRITUBA/PR
            { "4114351", "200700" }, // MANFRINÓPOLIS/PR
            { "4114401", "745400" }, // MANGUEIRINHA/PR
            { "4114500", "754800" }, // MANOEL RIBAS/PR
            { "4114609", "757500" }, // MARECHAL CÂNDIDO RONDON/PR
            { "4114708", "801000" }, // MARIA HELENA/PR
            { "4114807", "788300" }, // MARIALVA/PR
            { "4114906", "796500" }, // MARILÂNDIA DO SUL/PR
            { "4115002", "791200" }, // MARILENA/PR
            { "4115101", "750700" }, // MARILUZ/PR
            { "4115200", "788400" }, // MARINGÁ/PR
            { "4115309", "765700" }, // MARIÓPOLIS/PR
            { "4115358", "138000" }, // MARIPÁ/PR
            { "4115408", "765800" }, // MARMELEIRO/PR
            { "4115457", "179000" }, // MARQUINHO/PR
            { "4115507", "796600" }, // MARUMBI/PR
            { "4115606", "757600" }, // MATELÂNDIA/PR
            { "4115705", "731400" }, // MATINHOS/PR
            { "4115739", "138100" }, // MATO RICO/PR
            { "4115754", "138200" }, // MAUÁ DA SERRA/PR
            { "4115804", "757700" }, // MEDIANEIRA/PR
            { "4115853", "138300" }, // MERCEDES/PR
            { "4115903", "791300" }, // MIRADOR/PR
            { "4116000", "783800" }, // MIRASELVA/PR
            { "4116059", "757800" }, // MISSAL/PR
            { "4116109", "750800" }, // MOREIRA SALES/PR
            { "4116208", "731500" }, // MORRETES/PR
            { "4116307", "783900" }, // MUNHOZ DE MELO/PR
            { "4116406", "784000" }, // NOSSA SENHORA DAS GRAÇAS/PR
            { "4116505", "791400" }, // NOVA ALIANÇA DO IVAÍ/PR
            { "4116604", "778200" }, // NOVA AMÉRICA DA COLINA/PR
            { "4116703", "757900" }, // NOVA AURORA/PR
            { "4116802", "750900" }, // NOVA CANTU/PR
            { "4116901", "791500" }, // NOVA ESPERANÇA/PR
            { "4116950", "212900" }, // NOVA ESPERANÇA DO SUDOESTE/PR
            { "4117008", "778300" }, // NOVA FÁTIMA/PR
            { "4117057", "139000" }, // NOVA LARANJEIRAS/PR
            { "4117107", "791600" }, // NOVA LONDRINA/PR
            { "4117206", "801100" }, // NOVA OLÍMPIA/PR
            { "4117255", "765900" }, // NOVA PRATA DO IGUAÇU/PR
            { "4117214", "139100" }, // NOVA SANTA BÁRBARA/PR
            { "4117222", "758000" }, // NOVA SANTA ROSA/PR
            { "4117271", "043500" }, // NOVA TEBAS/PR
            { "4117297", "213000" }, // NOVO ITACOLOMI/PR
            { "4117305", "748100" }, // ORTIGUEIRA/PR
            { "4117404", "788500" }, // OURIZONA/PR
            { "4117453", "043800" }, // OURO VERDE DO OESTE/PR
            { "4117503", "788600" }, // PAIÇANDU/PR
            { "4117602", "745500" }, // PALMAS/PR
            { "4117701", "734600" }, // PALMEIRA/PR
            { "4117800", "735532" }, // PALMITAL/PR
            { "4117909", "758100" }, // PALOTINA/PR
            { "4118006", "791700" }, // PARAÍSO DO NORTE/PR
            { "4118105", "791800" }, // PARANACITY/PR
            { "4118204", "731600" }, // PARANAGUÁ/PR
            { "4118303", "791900" }, // PARANAPOEMA/PR
            { "4118402", "792000" }, // PARANAVAÍ/PR
            { "4118451", "086100" }, // PATO BRAGADO/PR
            { "4118501", "766000" }, // PATO BRANCO/PR
            { "4118600", "745600" }, // PAULA FREITAS/PR
            { "4118709", "745700" }, // PAULO FRONTIN/PR
            { "4118808", "751000" }, // PEABIRU/PR
            { "4118857", "179100" }, // PEROBAL/PR
            { "4118907", "801200" }, // PÉROLA/PR
            { "4119004", "766100" }, // PÉROLA D'OESTE/PR
            { "4119103", "733500" }, // PIÊN/PR
            { "4119152", "082500" }, // PINHAIS/PR
            { "4119251", "136900" }, // PINHAL DE SÃO BENTO/PR
            { "4119202", "742600" }, // PINHALÃO/PR
            { "4119301", "773700" }, // PINHÃO/PR
            { "4119400", "735600" }, // PIRAÍ DO SUL/PR
            { "4119509", "727900" }, // PIRAQUARA/PR
            { "4119608", "755000" }, // PITANGA/PR
            { "4119657", "082700" }, // PITANGUEIRAS/PR
            { "4119707", "792100" }, // PLANALTINA DO PARANÁ/PR
            { "4119806", "766200" }, // PLANALTO/PR
            { "4119905", "735700" }, // PONTA GROSSA/PR
            { "4119954", "201400" }, // PONTAL DO PARANÁ/PR
            { "4120002", "784100" }, // PORECATU/PR
            { "4120101", "734700" }, // PORTO AMAZONAS/PR
            { "4120150", "201500" }, // PORTO BARREIRO/PR
            { "4120200", "792200" }, // PORTO RICO/PR
            { "4120309", "745800" }, // PORTO VITÓRIA/PR
            { "4120333", "179200" }, // PRADO FERREIRA/PR
            { "4120358", "766300" }, // PRANCHITA/PR
            { "4120408", "792300" }, // PRESIDENTE CASTELO BRANCO/PR
            { "4120507", "784200" }, // PRIMEIRO DE MAIO/PR
            { "4120606", "739700" }, // PRUDENTÓPOLIS/PR
            { "4120705", "742700" }, // QUATIGUÁ/PR
            { "4120804", "728000" }, // QUATRO BARRAS/PR
            { "4120853", "088400" }, // QUATRO PONTES/PR
            { "4120903", "773800" }, // QUEDAS DO IGUAÇU/PR
            { "4121000", "792400" }, // QUERÊNCIA DO NORTE/PR
            { "4121109", "751100" }, // QUINTA DO SOL/PR
            { "4121208", "733600" }, // QUITANDINHA/PR
            { "4121257", "086000" }, // RAMILÂNDIA/PR
            { "4121307", "780600" }, // RANCHO ALEGRE/PR
            { "4121356", "084700" }, // RANCHO ALEGRE D'OESTE/PR
            { "4121406", "766400" }, // REALEZA/PR
            { "4121505", "739800" }, // REBOUÇAS/PR
            { "4121604", "766500" }, // RENASCENÇA/PR
            { "4121703", "748200" }, // RESERVA/PR
            { "4121752", "201600" }, // RESERVA DO IGUAÇU/PR
            { "4121802", "778400" }, // RIBEIRÃO CLARO/PR
            { "4121901", "778500" }, // RIBEIRÃO DO PINHAL/PR
            { "4122008", "739900" }, // RIO AZUL/PR
            { "4122107", "796700" }, // RIO BOM/PR
            { "4122156", "148300" }, // RIO BONITO DO IGUAÇU/PR
            { "4122172", "213600" }, // RIO BRANCO DO IVAÍ/PR
            { "4122206", "728100" }, // RIO BRANCO DO SUL/PR
            { "4122305", "734800" }, // RIO NEGRO/PR
            { "4122404", "784300" }, // ROLÂNDIA/PR
            { "4122503", "751200" }, // RONCADOR/PR
            { "4122602", "801300" }, // RONDON/PR
            { "4122651", "043600" }, // ROSÁRIO DO IVAÍ/PR
            { "4122701", "784400" }, // SABÁUDIA/PR
            { "4122800", "766600" }, // SALGADO FILHO/PR
            { "4122909", "742800" }, // SALTO DO ITARARÉ/PR
            { "4123006", "766700" }, // SALTO DO LONTRA/PR
            { "4123105", "778600" }, // SANTA AMÉLIA/PR
            { "4123204", "780700" }, // SANTA CECÍLIA DO PAVÃO/PR
            { "4123303", "" }, // SANTA CRUZ DE MONTE CASTELO/PR
            { "4123402", "784500" }, // SANTA FÉ/PR
            { "4123501", "758200" }, // SANTA HELENA/PR
            { "4123600", "784600" }, // SANTA INÊS/PR
            { "4123709", "792600" }, // SANTA ISABEL DO IVAÍ/PR
            { "4123808", "766800" }, // SANTA IZABEL DO OESTE/PR
            { "4123824", "083400" }, // SANTA LÚCIA/PR
            { "4123857", "140000" }, // SANTA MARIA DO OESTE/PR
            { "4123907", "778700" }, // SANTA MARIANA/PR
            { "4123956", "084900" }, // SANTA MÔNICA/PR
            { "4124020", "043700" }, // SANTA TEREZA DO OESTE/PR
            { "4124053", "758300" }, // SANTA TEREZINHA DE ITAIPU/PR
            { "4124004", "742900" }, // SANTANA DO ITARARÉ/PR
            { "4124103", "778800" }, // SANTO ANTÔNIO DA PLATINA/PR
            { "4124202", "792700" }, // SANTO ANTÔNIO DO CAIUÁ/PR
            { "4124301", "778900" }, // SANTO ANTÔNIO DO PARAÍSO/PR
            { "4124400", "766900" }, // SANTO ANTÔNIO DO SUDOESTE/PR
            { "4124509", "784700" }, // SANTO INÁCIO/PR
            { "4124608", "788700" }, // SÃO CARLOS DO IVAÍ/PR
            { "4124707", "780800" }, // SÃO JERÔNIMO DA SERRA/PR
            { "4124806", "749908" }, // SÃO JOÃO/PR
            { "4124905", "792800" }, // SÃO JOÃO DO CAIUÁ/PR
            { "4125001", "796800" }, // SÃO JOÃO DO IVAÍ/PR
            { "4125100", "738500" }, // SÃO JOÃO DO TRIUNFO/PR
            { "4125308", "788800" }, // SÃO JORGE DO IVAÍ/PR
            { "4125357", "801400" }, // SÃO JORGE DO PATROCÍNIO/PR
            { "4125209", "767100" }, // SÃO JORGE D'OESTE/PR
            { "4125407", "743000" }, // SÃO JOSÉ DA BOA VISTA/PR
            { "4125456", "043900" }, // SÃO JOSÉ DAS PALMEIRAS/PR
            { "4125506", "728200" }, // SÃO JOSÉ DOS PINHAIS/PR
            { "4125555", "084800" }, // SÃO MANOEL DO PARANÁ/PR
            { "4125605", "738600" }, // SÃO MATEUS DO SUL/PR
            { "4125704", "758400" }, // SÃO MIGUEL DO IGUAÇU/PR
            { "4125753", "140100" }, // SÃO PEDRO DO IGUAÇU/PR
            { "4125803", "796900" }, // SÃO PEDRO DO IVAÍ/PR
            { "4125902", "792900" }, // SÃO PEDRO DO PARANÁ/PR
            { "4126009", "780900" }, // SÃO SEBASTIÃO DA AMOREIRA/PR
            { "4126108", "801500" }, // SÃO TOMÉ/PR
            { "4126207", "743100" }, // SAPOPEMA/PR
            { "4126256", "766804" }, // SARANDI/PR
            { "4126272", "140200" }, // SAUDADE DO IGUAÇU/PR
            { "4126306", "737600" }, // SENGÉS/PR
            { "4126355", "201700" }, // SERRANÓPOLIS DO IGUAÇU/PR
            { "4126405", "779000" }, // SERTANEJA/PR
            { "4126504", "784800" }, // SERTANÓPOLIS/PR
            { "4126603", "743200" }, // SIQUEIRA CAMPOS/PR
            { "4126652", "044000" }, // SULINA/PR
            { "4126678", "179400" }, // TAMARANA/PR
            { "4126702", "793000" }, // TAMBOARA/PR
            { "4126801", "801600" }, // TAPEJARA/PR
            { "4126900", "801700" }, // TAPIRA/PR
            { "4127007", "740000" }, // TEIXEIRA SOARES/PR
            { "4127106", "735800" }, // TELÊMACO BORBA/PR
            { "4127205", "801800" }, // TERRA BOA/PR
            { "4127304", "793100" }, // TERRA RICA/PR
            { "4127403", "758500" }, // TERRA ROXA/PR
            { "4127502", "735900" }, // TIBAGI/PR
            { "4127601", "733700" }, // TIJUCAS DO SUL/PR
            { "4127700", "758600" }, // TOLEDO/PR
            { "4127809", "743300" }, // TOMAZINA/PR
            { "4127858", "758700" }, // TRÊS BARRAS DO PARANÁ/PR
            { "4127882", "082600" }, // TUNAS DO PARANÁ/PR
            { "4127908", "801900" }, // TUNEIRAS DO OESTE/PR
            { "4127957", "758800" }, // TUPÃSSI/PR
            { "4127965", "773900" }, // TURVO/PR
            { "4128005", "751300" }, // UBIRATÃ/PR
            { "4128104", "802000" }, // UMUARAMA/PR
            { "4128203", "745900" }, // UNIÃO DA VITÓRIA/PR
            { "4128302", "789000" }, // UNIFLOR/PR
            { "4128401", "781000" }, // URAÍ/PR
            { "4128534", "083800" }, // VENTANIA/PR
            { "4128559", "758900" }, // VERA CRUZ DO OESTE/PR
            { "4128609", "767200" }, // VERÊ/PR
            { "4128658", "083700" }, // VIRMOND/PR
            { "4128708", "767300" }, // VITORINO/PR
            { "4128500", "743400" }, // WENCESLAU BRAZ/PR
            { "4128807", "802100" }, // XAMBRÊ/PR
            { "3300100", "609000" }, // ANGRA DOS REIS/RJ
            { "3300159", "073600" }, // APERIBÉ/RJ
            { "3300209", "603300" }, // ARARUAMA/RJ
            { "3300225", "074000" }, // AREAL/RJ
            { "3300233", "" }, // ARMAÇÃO DOS BÚZIOS/RJ
            { "3300258", "603700" }, // ARRAIAL DO CABO/RJ
            { "3300308", "610200" }, // BARRA DO PIRAÍ/RJ
            { "3300407", "610300" }, // BARRA MANSA/RJ
            { "3300456", "073100" }, // BELFORD ROXO/RJ
            { "3300506", "597900" }, // BOM JARDIM/RJ
            { "3300605", "587800" }, // BOM JESUS DO ITABAPOANA/RJ
            { "3300704", "603400" }, // CABO FRIO/RJ
            { "3300803", "601800" }, // CACHOEIRAS DE MACACU/RJ
            { "3300902", "589900" }, // CAMBUCI/RJ
            { "3301009", "043400" }, // CAMPOS DOS GOYTACAZES/RJ
            { "3301108", "596000" }, // CANTAGALO/RJ
            { "3300936", "179600" }, // CARAPEBUS/RJ
            { "3301157", "073400" }, // CARDOSO MOREIRA/RJ
            { "3301207", "596100" }, // CARMO/RJ
            { "3301306", "601900" }, // CASIMIRO DE ABREU/RJ
            { "3300951", "074100" }, // COMENDADOR LEVY GASPARIAN/RJ
            { "3301405", "592000" }, // CONCEIÇÃO DE MACABU/RJ
            { "3301504", "598000" }, // CORDEIRO/RJ
            { "3301603", "596200" }, // DUAS BARRAS/RJ
            { "3301702", "613600" }, // DUQUE DE CAXIAS/RJ
            { "3301801", "606500" }, // ENGENHEIRO PAULO DE FRONTIN/RJ
            { "3301850", "073000" }, // GUAPIMIRIM/RJ
            { "3301876", "179700" }, // IGUABA GRANDE/RJ
            { "3301900", "613700" }, // ITABORAÍ/RJ
            { "3302007", "613800" }, // ITAGUAÍ/RJ
            { "3302056", "592400" }, // ITALVA/RJ
            { "3302106", "596300" }, // ITAOCARA/RJ
            { "3302205", "587900" }, // ITAPERUNA/RJ
            { "3302254", "610800" }, // ITATIAIA/RJ
            { "3302270", "073300" }, // JAPERI/RJ
            { "3302304", "588000" }, // LAJE DO MURIAÉ/RJ
            { "3302403", "592100" }, // MACAÉ/RJ
            { "3302452", "179800" }, // MACUCO/RJ
            { "3302502", "613900" }, // MAGÉ/RJ
            { "3302601", "614000" }, // MANGARATIBA/RJ
            { "3302700", "614100" }, // MARICÁ/RJ
            { "3302809", "606600" }, // MENDES/RJ
            { "3302908", "606700" }, // MIGUEL PEREIRA/RJ
            { "3303005", "590000" }, // MIRACEMA/RJ
            { "3303104", "588100" }, // NATIVIDADE/RJ
            { "3303203", "614200" }, // NILÓPOLIS/RJ
            { "3303302", "614300" }, // NITERÓI/RJ
            { "3303401", "600100" }, // NOVA FRIBURGO/RJ
            { "3303500", "614400" }, // NOVA IGUAÇU/RJ
            { "3303609", "614500" }, // PARACAMBI/RJ
            { "3303708", "604800" }, // PARAÍBA DO SUL/RJ
            { "3303807", "609100" }, // PARATI/RJ
            { "3303856", "607100" }, // PATY DO ALFERES/RJ
            { "3303906", "600200" }, // PETRÓPOLIS/RJ
            { "3303955", "179900" }, // PINHEIRAL/RJ
            { "3304003", "606800" }, // PIRAÍ/RJ
            { "3304102", "588200" }, // PORCIÚNCULA/RJ
            { "3304110", "180000" }, // PORTO REAL/RJ
            { "3304128", "073800" }, // QUATIS/RJ
            { "3304144", "073200" }, // QUEIMADOS/RJ
            { "3304151", "235600" }, // QUISSAMÃ/RJ
            { "3304201", "610400" }, // RESENDE/RJ
            { "3304300", "602000" }, // RIO BONITO/RJ
            { "3304409", "606900" }, // RIO CLARO/RJ
            { "3304508", "610500" }, // RIO DAS FLORES/RJ
            { "3304524", "073700" }, // RIO DAS OSTRAS/RJ
            { "3304557", "618500" }, // RIO DE JANEIRO/RJ
            { "3304607", "598100" }, // SANTA MARIA MADALENA/RJ
            { "3304706", "590100" }, // SANTO ANTÔNIO DE PÁDUA/RJ
            { "3304805", "592200" }, // SÃO FIDÉLIS/RJ
            { "3304755", "201800" }, // SÃO FRANCISCO DE ITABAPOANA/RJ
            { "3304904", "614600" }, // SÃO GONÇALO/RJ
            { "3305000", "592300" }, // SÃO JOÃO DA BARRA/RJ
            { "3305109", "614700" }, // SÃO JOÃO DE MERITI/RJ
            { "3305133", "180100" }, // SÃO JOSÉ DE UBÁ/RJ
            { "3305158", "043300" }, // SÃO JOSÉ DO VALE DO RIO PRETO/RJ
            { "3305208", "603500" }, // SÃO PEDRO DA ALDEIA/RJ
            { "3305307", "598200" }, // SÃO SEBASTIÃO DO ALTO/RJ
            { "3305406", "604900" }, // SAPUCAIA/RJ
            { "3305505", "603600" }, // SAQUAREMA/RJ
            { "3305554", "180200" }, // SEROPÉDICA/RJ
            { "3305604", "602100" }, // SILVA JARDIM/RJ
            { "3305703", "596400" }, // SUMIDOURO/RJ
            { "3305752", "180300" }, // TANGUÁ/RJ
            { "3305802", "600300" }, // TERESÓPOLIS/RJ
            { "3305901", "598300" }, // TRAJANO DE MORAIS/RJ
            { "3306008", "605000" }, // TRÊS RIOS/RJ
            { "3306107", "610600" }, // VALENÇA/RJ
            { "3306156", "073500" }, // VARRE-SAI/RJ
            { "3306206", "607000" }, // VASSOURAS/RJ
            { "3306305", "610700" }, // VOLTA REDONDA/RJ
            { "2400109", "224800" }, // ACARI/RN
            { "2400208", "218700" }, // AÇU/RN
            { "2400307", "228100" }, // AFONSO BEZERRA/RN
            { "2400406", "220500" }, // ÁGUA NOVA/RN
            { "2400505", "220600" }, // ALEXANDRIA/RN
            { "2400604", "220700" }, // ALMINO AFONSO/RN
            { "2400703", "217300" }, // ALTO DO RODRIGUES/RN
            { "2400802", "228200" }, // ANGICOS/RN
            { "2400901", "220800" }, // ANTÔNIO MARTINS/RN
            { "2401008", "218800" }, // APODI/RN
            { "2401107", "217400" }, // AREIA BRANCA/RN
            { "2401206", "" }, // AREZ/RN
            { "2401404", "236100" }, // BAÍA FORMOSA/RN
            { "2401453", "217500" }, // BARAÚNA/RN
            { "2401503", "231000" }, // BARCELONA/RN
            { "2401602", "229300" }, // BENTO FERNANDES/RN
            { "2405306", "" }, // BOA SAÚDE/RN
            { "2401651", "180400" }, // BODÓ/RN
            { "2401701", "233200" }, // BOM JESUS/RN
            { "2401800", "233300" }, // BREJINHO/RN
            { "2401859", "201900" }, // CAIÇARA DO NORTE/RN
            { "2401909", "229400" }, // CAIÇARA DO RIO DO VENTO/RN
            { "2402006", "224900" }, // CAICÓ/RN
            { "2401305", "" }, // CAMPO GRANDE/RN
            { "2402105", "231100" }, // CAMPO REDONDO/RN
            { "2402204", "236200" }, // CANGUARETAMA/RN
            { "2402303", "219000" }, // CARAÚBAS/RN
            { "2402402", "225000" }, // CARNAÚBA DOS DANTAS/RN
            { "2402501", "217600" }, // CARNAUBAIS/RN
            { "2402600", "236300" }, // CEARÁ-MIRIM/RN
            { "2402709", "225100" }, // CERRO CORÁ/RN
            { "2402808", "231200" }, // CORONEL EZEQUIEL/RN
            { "2402907", "220900" }, // CORONEL JOÃO PESSOA/RN
            { "2403004", "225200" }, // CRUZETA/RN
            { "2403103", "225300" }, // CURRAIS NOVOS/RN
            { "2403202", "221000" }, // DOUTOR SEVERIANO/RN
            { "2403301", "221100" }, // ENCANTO/RN
            { "2403400", "225400" }, // EQUADOR/RN
            { "2403509", "219600" }, // ESPÍRITO SANTO/RN
            { "2403608", "236600" }, // EXTREMOZ/RN
            { "2403707", "219100" }, // FELIPE GUERRA/RN
            { "2403756", "180500" }, // FERNANDO PEDROZA/RN
            { "2403806", "225500" }, // FLORÂNIA/RN
            { "2403905", "221200" }, // FRANCISCO DANTAS/RN
            { "2404002", "221300" }, // FRUTUOSO GOMES/RN
            { "2404101", "227300" }, // GALINHOS/RN
            { "2404200", "236700" }, // GOIANINHA/RN
            { "2404309", "219200" }, // GOVERNADOR DIX-SEPT ROSADO/RN
            { "2404408", "217700" }, // GROSSOS/RN
            { "2404507", "217800" }, // GUAMARÉ/RN
            { "2404606", "233400" }, // IELMO MARINHO/RN
            { "2404705", "219300" }, // IPANGUAÇU/RN
            { "2404804", "225600" }, // IPUEIRA/RN
            { "2404853", "202000" }, // ITAJÁ/RN
            { "2404903", "219400" }, // ITAÚ/RN
            { "2405009", "231300" }, // JAÇANÃ/RN
            { "2405108", "229500" }, // JANDAÍRA/RN
            { "2405207", "219500" }, // JANDUÍS/RN
            { "2405405", "231400" }, // JAPI/RN
            { "2405504", "229600" }, // JARDIM DE ANGICOS/RN
            { "2405603", "225700" }, // JARDIM DE PIRANHAS/RN
            { "2405702", "225800" }, // JARDIM DO SERIDÓ/RN
            { "2405801", "229700" }, // JOÃO CÂMARA/RN
            { "2405900", "221400" }, // JOÃO DIAS/RN
            { "2406007", "221500" }, // JOSÉ DA PENHA/RN
            { "2406106", "225900" }, // JUCURUTU/RN
            { "2406205", "233600" }, // LAGOA D'ANTA/RN
            { "2406304", "233700" }, // LAGOA DE PEDRAS/RN
            { "2406403", "231500" }, // LAGOA DE VELHOS/RN
            { "2406502", "226000" }, // LAGOA NOVA/RN
            { "2406601", "233800" }, // LAGOA SALGADA/RN
            { "2406700", "229800" }, // LAJES/RN
            { "2406809", "231600" }, // LAJES PINTADAS/RN
            { "2406908", "221600" }, // LUCRÉCIA/RN
            { "2407005", "221700" }, // LUÍS GOMES/RN
            { "2407104", "236800" }, // MACAÍBA/RN
            { "2407203", "217900" }, // MACAU/RN
            { "2407252", "181300" }, // MAJOR SALES/RN
            { "2407302", "221800" }, // MARCELINO VIEIRA/RN
            { "2407401", "221900" }, // MARTINS/RN
            { "2407500", "236900" }, // MAXARANGUAPE/RN
            { "2407609", "222000" }, // MESSIAS TARGINO/RN
            { "2407708", "233900" }, // MONTANHAS/RN
            { "2407807", "234000" }, // MONTE ALEGRE/RN
            { "2407906", "231700" }, // MONTE DAS GAMELEIRAS/RN
            { "2408003", "218000" }, // MOSSORÓ/RN
            { "2408102", "237000" }, // NATAL/RN
            { "2408201", "237100" }, // NÍSIA FLORESTA/RN
            { "2408300", "234100" }, // NOVA CRUZ/RN
            { "2408409", "222100" }, // OLHO-D'ÁGUA DO BORGES/RN
            { "2408508", "226100" }, // OURO BRANCO/RN
            { "2408607", "222200" }, // PARANÁ/RN
            { "2408706", "235800" }, // PARAÚ/RN
            { "2408805", "229900" }, // PARAZINHO/RN
            { "2408904", "226200" }, // PARELHAS/RN
            { "2403251", "237800" }, // PARNAMIRIM/RN
            { "2409100", "234200" }, // PASSA E FICA/RN
            { "2409209", "234300" }, // PASSAGEM/RN
            { "2409308", "222300" }, // PATU/RN
            { "2409407", "222400" }, // PAU DOS FERROS/RN
            { "2409506", "227400" }, // PEDRA GRANDE/RN
            { "2409605", "230000" }, // PEDRA PRETA/RN
            { "2409704", "228300" }, // PEDRO AVELINO/RN
            { "2409803", "237200" }, // PEDRO VELHO/RN
            { "2409902", "218100" }, // PENDÊNCIAS/RN
            { "2410009", "222500" }, // PILÕES/RN
            { "2410108", "230100" }, // POÇO BRANCO/RN
            { "2410207", "222600" }, // PORTALEGRE/RN
            { "2410256", "181400" }, // PORTO DO MANGUE/RN
            { "2410405", "230200" }, // PUREZA/RN
            { "2410504", "222700" }, // RAFAEL FERNANDES/RN
            { "2410603", "222800" }, // RAFAEL GODEIRO/RN
            { "2410702", "222900" }, // RIACHO DA CRUZ/RN
            { "2410801", "223000" }, // RIACHO DE SANTANA/RN
            { "2410900", "234500" }, // RIACHUELO/RN
            { "2408953", "202100" }, // RIO DO FOGO/RN
            { "2411007", "223100" }, // RODOLFO FERNANDES/RN
            { "2411106", "231800" }, // RUY BARBOSA/RN
            { "2411205", "231900" }, // SANTA CRUZ/RN
            { "2409332", "181500" }, // SANTA MARIA/RN
            { "2411403", "228400" }, // SANTANA DO MATOS/RN
            { "2411429", "226300" }, // SANTANA DO SERIDÓ/RN
            { "2411502", "234600" }, // SANTO ANTÔNIO/RN
            { "2411601", "227500" }, // SÃO BENTO DO NORTE/RN
            { "2411700", "232000" }, // SÃO BENTO DO TRAIRI/RN
            { "2411809", "226400" }, // SÃO FERNANDO/RN
            { "2411908", "223200" }, // SÃO FRANCISCO DO OESTE/RN
            { "2412005", "237300" }, // SÃO GONÇALO DO AMARANTE/RN
            { "2412104", "226500" }, // SÃO JOÃO DO SABUGI/RN
            { "2412203", "237400" }, // SÃO JOSÉ DE MIPIBU/RN
            { "2412302", "232100" }, // SÃO JOSÉ DO CAMPESTRE/RN
            { "2412401", "226600" }, // SÃO JOSÉ DO SERIDÓ/RN
            { "2412500", "223300" }, // SÃO MIGUEL/RN
            { "2412559", "202200" }, // SÃO MIGUEL DE TOUROS/RN
            { "2412609", "234700" }, // SÃO PAULO DO POTENGI/RN
            { "2412708", "234800" }, // SÃO PEDRO/RN
            { "2412807", "219700" }, // SÃO RAFAEL/RN
            { "2412906", "232200" }, // SÃO TOMÉ/RN
            { "2413003", "226700" }, // SÃO VICENTE/RN
            { "2413102", "234900" }, // SENADOR ELÓI DE SOUZA/RN
            { "2413201", "237500" }, // SENADOR GEORGINO AVELINO/RN
            { "2410306", "234400" }, // SERRA CAIADA/RN
            { "2413300", "232300" }, // SERRA DE SÃO BENTO/RN
            { "2413359", "023000" }, // SERRA DO MEL/RN
            { "2413409", "226800" }, // SERRA NEGRA DO NORTE/RN
            { "2413508", "235000" }, // SERRINHA/RN
            { "2413557", "202300" }, // SERRINHA DOS PINTOS/RN
            { "2413607", "219800" }, // SEVERIANO MELO/RN
            { "2413706", "232400" }, // SÍTIO NOVO/RN
            { "2413805", "223400" }, // TABOLEIRO GRANDE/RN
            { "2413904", "230300" }, // TAIPU/RN
            { "2414001", "232500" }, // TANGARÁ/RN
            { "2414100", "223500" }, // TENENTE ANANIAS/RN
            { "2414159", "202400" }, // TENENTE LAURENTINO CRUZ/RN
            { "2411056", "181600" }, // TIBAU/RN
            { "2414209", "237600" }, // TIBAU DO SUL/RN
            { "2414308", "226900" }, // TIMBAÚBA DOS BATISTAS/RN
            { "2414407", "227600" }, // TOUROS/RN
            { "2414456", "202500" }, // TRIUNFO POTIGUAR/RN
            { "2414506", "223600" }, // UMARIZAL/RN
            { "2414605", "219900" }, // UPANEMA/RN
            { "2414704", "235100" }, // VÁRZEA/RN
            { "2414753", "202600" }, // VENHA-VER/RN
            { "2414803", "235200" }, // VERA CRUZ/RN
            { "2414902", "223700" }, // VIÇOSA/RN
            { "2415008", "237700" }, // VILA FLOR/RN
            { "1100015", "" }, // ALTA FLORESTA D'OESTE/RO
            { "1100379", "213700" }, // ALTO ALEGRE DOS PARECIS/RO
            { "1100403", "103600" }, // ALTO PARAÍSO/RO
            { "1100346", "054500" }, // ALVORADA D'OESTE/RO
            { "1100023", "075100" }, // ARIQUEMES/RO
            { "1100452", "213800" }, // BURITIS/RO
            { "1100031", "054800" }, // CABIXI/RO
            { "1100601", "104000" }, // CACAULÂNDIA/RO
            { "1100049", "075200" }, // CACOAL/RO
            { "1100700", "104100" }, // CAMPO NOVO DE RONDÔNIA/RO
            { "1100809", "104200" }, // CANDEIAS DO JAMARI/RO
            { "1100908", "104300" }, // CASTANHEIRAS/RO
            { "1100056", "076500" }, // CEREJEIRAS/RO
            { "1100924", "213900" }, // CHUPINGUAIA/RO
            { "1100064", "075300" }, // COLORADO DO OESTE/RO
            { "1100072", "103400" }, // CORUMBIARA/RO
            { "1100080", "075400" }, // COSTA MARQUES/RO
            { "1100940", "202700" }, // CUJUBIM/RO
            { "1100098", "" }, // ESPIGÃO DO OESTE/RO
            { "1101005", "105100" }, // GOVERNADOR JORGE TEIXEIRA/RO
            { "1100106", "075600" }, // GUAJARÁ-MIRIM/RO
            { "1101104", "105200" }, // ITAPUÃ DO OESTE/RO
            { "1100114", "075700" }, // JARU/RO
            { "1100122", "075800" }, // JI-PARANÁ/RO
            { "1100130", "054200" }, // MACHADINHO D'OESTE/RO
            { "1101203", "105300" }, // MINISTRO ANDREAZZA/RO
            { "1101302", "105400" }, // MIRANTE DA SERRA/RO
            { "1101401", "105500" }, // MONTE NEGRO/RO
            { "1100148", "054300" }, // NOVA BRASILÂNDIA D'OESTE/RO
            { "1100338", "054100" }, // NOVA MAMORÉ/RO
            { "1101435", "203300" }, // NOVA UNIÃO/RO
            { "1100502", "214000" }, // NOVO HORIZONTE DO OESTE/RO
            { "1100155", "075900" }, // OURO PRETO DO OESTE/RO
            { "1101450", "203400" }, // PARECIS/RO
            { "1100189", "076000" }, // PIMENTA BUENO/RO
            { "1101468", "203500" }, // PIMENTEIRAS DO OESTE/RO
            { "1100205", "076100" }, // PORTO VELHO/RO
            { "1100254", "076200" }, // PRESIDENTE MÉDICI/RO
            { "1101476", "203600" }, // PRIMAVERA DE RONDÔNIA/RO
            { "1100262", "103500" }, // RIO CRESPO/RO
            { "1100288", "076400" }, // ROLIM DE MOURA/RO
            { "1100296", "054700" }, // SANTA LUZIA D'OESTE/RO
            { "1101484", "203700" }, // SÃO FELIPE D'OESTE/RO
            { "1101492", "203800" }, // SÃO FRANCISCO DO GUAPORÉ/RO
            { "1100320", "054400" }, // SÃO MIGUEL DO GUAPORÉ/RO
            { "1101500", "105600" }, // SERINGUEIRAS/RO
            { "1101559", "203900" }, // TEIXEIRÓPOLIS/RO
            { "1101609", "105700" }, // THEOBROMA/RO
            { "1101708", "107000" }, // URUPÁ/RO
            { "1101757", "204000" }, // VALE DO ANARI/RO
            { "1101807", "107100" }, // VALE DO PARAÍSO/RO
            { "1100304", "076300" }, // VILHENA/RO
            { "1400050", "088900" }, // ALTO ALEGRE/RR
            { "1400027", "204100" }, // AMAJARI/RR
            { "1400100", "089000" }, // BOA VISTA/RR
            { "1400159", "089100" }, // BONFIM/RR
            { "1400175", "214100" }, // CANTÁ/RR
            { "1400209", "089200" }, // CARACARAÍ/RR
            { "1400233", "204200" }, // CAROEBE/RR
            { "1400282", "204300" }, // IRACEMA/RR
            { "1400308", "089300" }, // MUCAJAÍ/RR
            { "1400407", "089400" }, // NORMANDIA/RR
            { "1400456", "204400" }, // PACARAIMA/RR
            { "1400472", "204500" }, // RORAINÓPOLIS/RR
            { "1400506", "089500" }, // SÃO JOÃO DA BALIZA/RR
            { "1400605", "089600" }, // SÃO LUIZ/RR
            { "1400704", "214200" }, // UIRAMUTÃ/RR
            { "4300034", "218300" }, // ACEGUÁ/RS
            { "4300059", "905300" }, // ÁGUA SANTA/RS
            { "4300109", "860000" }, // AGUDO/RS
            { "4300208", "911000" }, // AJURICABA/RS
            { "4300307", "889600" }, // ALECRIM/RS
            { "4300406", "927600" }, // ALEGRETE/RS
            { "4300455", "891700" }, // ALEGRIA/RS
            { "4300471", "216300" }, // ALMIRANTE TAMANDARÉ DO SUL/RS
            { "4300505", "896600" }, // ALPESTRE/RS
            { "4300554", "048900" }, // ALTO ALEGRE/RS
            { "4300570", "118300" }, // ALTO FELIZ/RS
            { "4300604", "849300" }, // ALVORADA/RS
            { "4300638", "877000" }, // AMARAL FERRADOR/RS
            { "4300646", "118400" }, // AMETISTA DO SUL/RS
            { "4300661", "048700" }, // ANDRÉ DA ROCHA/RS
            { "4300703", "882300" }, // ANTA GORDA/RS
            { "4300802", "878500" }, // ANTÔNIO PRADO/RS
            { "4300851", "096600" }, // ARAMBARÉ/RS
            { "4300877", "181700" }, // ARARICÁ/RS
            { "4300901", "902700" }, // ARATIBA/RS
            { "4301008", "857100" }, // ARROIO DO MEIO/RS
            { "4301073", "218400" }, // ARROIO DO PADRE/RS
            { "4301057", "048300" }, // ARROIO DO SAL/RS
            { "4301206", "860100" }, // ARROIO DO TIGRE/RS
            { "4301107", "866300" }, // ARROIO DOS RATOS/RS
            { "4301305", "875100" }, // ARROIO GRANDE/RS
            { "4301404", "882400" }, // ARVOREZINHA/RS
            { "4301503", "911100" }, // AUGUSTO PESTANA/RS
            { "4301552", "905400" }, // ÁUREA/RS
            { "4301602", "927700" }, // BAGÉ/RS
            { "4301636", "204600" }, // BALNEÁRIO PINHAL/RS
            { "4301651", "854700" }, // BARÃO/RS
            { "4301701", "902800" }, // BARÃO DE COTEGIPE/RS
            { "4301750", "095400" }, // BARÃO DO TRIUNFO/RS
            { "4301859", "102000" }, // BARRA DO GUARITA/RS
            { "4301875", "181800" }, // BARRA DO QUARAÍ/RS
            { "4301909", "849400" }, // BARRA DO RIBEIRO/RS
            { "4301925", "098700" }, // BARRA DO RIO AZUL/RS
            { "4301958", "098300" }, // BARRA FUNDA/RS
            { "4301800", "902900" }, // BARRACÃO/RS
            { "4302006", "918800" }, // BARROS CASSAL/RS
            { "4302055", "204700" }, // BENJAMIN CONSTANT DO SUL/RS
            { "4302105", "878600" }, // BENTO GONÇALVES/RS
            { "4302154", "118500" }, // BOA VISTA DAS MISSÕES/RS
            { "4302204", "889700" }, // BOA VISTA DO BURICÁ/RS
            { "4302220", "224100" }, // BOA VISTA DO CADEADO/RS
            { "4302238", "224200" }, // BOA VISTA DO INCRA/RS
            { "4302253", "214300" }, // BOA VISTA DO SUL/RS
            { "4302303", "920800" }, // BOM JESUS/RS
            { "4302352", "853200" }, // BOM PRINCÍPIO/RS
            { "4302378", "102100" }, // BOM PROGRESSO/RS
            { "4302402", "857200" }, // BOM RETIRO DO SUL/RS
            { "4302451", "858100" }, // BOQUEIRÃO DO LEÃO/RS
            { "4302501", "885900" }, // BOSSOROCA/RS
            { "4302584", "224300" }, // BOZANO/RS
            { "4302600", "896700" }, // BRAGA/RS
            { "4302659", "047800" }, // BROCHIER/RS
            { "4302709", "866400" }, // BUTIÁ/RS
            { "4302808", "876300" }, // CAÇAPAVA DO SUL/RS
            { "4302907", "927800" }, // CACEQUI/RS
            { "4303004", "866500" }, // CACHOEIRA DO SUL/RS
            { "4303103", "849500" }, // CACHOEIRINHA/RS
            { "4303202", "903000" }, // CACIQUE DOBLE/RS
            { "4303301", "886000" }, // CAIBATÉ/RS
            { "4303400", "896800" }, // CAIÇARA/RS
            { "4303509", "870200" }, // CAMAQUÃ/RS
            { "4303558", "905500" }, // CAMARGO/RS
            { "4303608", "920900" }, // CAMBARÁ DO SUL/RS
            { "4303673", "118700" }, // CAMPESTRE DA SERRA/RS
            { "4303707", "889800" }, // CAMPINA DAS MISSÕES/RS
            { "4303806", "903100" }, // CAMPINAS DO SUL/RS
            { "4303905", "849600" }, // CAMPO BOM/RS
            { "4304002", "882702" }, // CAMPO NOVO/RS
            { "4304101", "919200" }, // CAMPOS BORGES/RS
            { "4304200", "889701" }, // CANDELÁRIA/RS
            { "4304309", "889900" }, // CÂNDIDO GODÓI/RS
            { "4304358", "102400" }, // CANDIOTA/RS
            { "4304408", "853300" }, // CANELA/RS
            { "4304507", "870300" }, // CANGUÇU/RS
            { "4304606", "849700" }, // CANOAS/RS
            { "4304614", "224400" }, // CANUDOS DO VALE/RS
            { "4304622", "224500" }, // CAPÃO BONITO DO SUL/RS
            { "4304630", "863900" }, // CAPÃO DA CANOA/RS
            { "4304655", "224600" }, // CAPÃO DO CIPÓ/RS
            { "4304663", "870400" }, // CAPÃO DO LEÃO/RS
            { "4304689", "854800" }, // CAPELA DE SANTANA/RS
            { "4304697", "100600" }, // CAPITÃO/RS
            { "4304671", "204800" }, // CAPIVARI DO SUL/RS
            { "4304713", "181900" }, // CARAÁ/RS
            { "4304705", "913000" }, // CARAZINHO/RS
            { "4304804", "878700" }, // CARLOS BARBOSA/RS
            { "4304853", "098800" }, // CARLOS GOMES/RS
            { "4304903", "882500" }, // CASCA/RS
            { "4304952", "921500" }, // CASEIROS/RS
            { "4305009", "886100" }, // CATUÍPE/RS
            { "4305108", "878800" }, // CAXIAS DO SUL/RS
            { "4305116", "098900" }, // CENTENÁRIO/RS
            { "4305124", "204900" }, // CERRITO/RS
            { "4305132", "047400" }, // CERRO BRANCO/RS
            { "4305157", "048800" }, // CERRO GRANDE/RS
            { "4305173", "047700" }, // CERRO GRANDE DO SUL/RS
            { "4305207", "890000" }, // CERRO LARGO/RS
            { "4305306", "913100" }, // CHAPADA/RS
            { "4305355", "866600" }, // CHARQUEADAS/RS
            { "4305371", "099000" }, // CHARRUA/RS
            { "4305405", "" }, // CHIAPETTA/RS
            { "4305439", "182000" }, // CHUÍ/RS
            { "4305447", "205000" }, // CHUVISCA/RS
            { "4305454", "046500" }, // CIDREIRA/RS
            { "4305504", "903200" }, // CIRÍACO/RS
            { "4305587", "118800" }, // COLINAS/RS
            { "4305603", "917500" }, // COLORADO/RS
            { "4305702", "911300" }, // CONDOR/RS
            { "4305801", "897000" }, // CONSTANTINA/RS
            { "4305835", "218500" }, // COQUEIRO BAIXO/RS
            { "4305850", "118900" }, // COQUEIROS DO SUL/RS
            { "4305871", "101500" }, // CORONEL BARROS/RS
            { "4305900", "913200" }, // CORONEL BICACO/RS
            { "4305934", "218600" }, // CORONEL PILAR/RS
            { "4305959", "878900" }, // COTIPORÃ/RS
            { "4305975", "097100" }, // COXILHA/RS
            { "4306007", "890100" }, // CRISSIUMAL/RS
            { "4306056", "871000" }, // CRISTAL/RS
            { "4306072", "205100" }, // CRISTAL DO SUL/RS
            { "4306106", "931700" }, // CRUZ ALTA/RS
            { "4306130", "220000" }, // CRUZALTENSE/RS
            { "4306205", "857300" }, // CRUZEIRO DO SUL/RS
            { "4306304", "882600" }, // DAVID CANABARRO/RS
            { "4306320", "102200" }, // DERRUBADAS/RS
            { "4306353", "886600" }, // DEZESSEIS DE NOVEMBRO/RS
            { "4306379", "205200" }, // DILERMANDO DE AGUIAR/RS
            { "4306403", "897103" }, // DOIS IRMÃOS/RS
            { "4306429", "119500" }, // DOIS IRMÃOS DAS MISSÕES/RS
            { "4306452", "883600" }, // DOIS LAJEADOS/RS
            { "4306502", "870500" }, // DOM FELICIANO/RS
            { "4306601", "927900" }, // DOM PEDRITO/RS
            { "4306551", "205300" }, // DOM PEDRO DE ALCÂNTARA/RS
            { "4306700", "860300" }, // DONA FRANCISCA/RS
            { "4306734", "891800" }, // DOUTOR MAURÍCIO CARDOSO/RS
            { "4306759", "214400" }, // DOUTOR RICARDO/RS
            { "4306767", "048400" }, // ELDORADO DO SUL/RS
            { "4306809", "857400" }, // ENCANTADO/RS
            { "4306908", "876400" }, // ENCRUZILHADA DO SUL/RS
            { "4306924", "098400" }, // ENGENHO VELHO/RS
            { "4306957", "046800" }, // ENTRE RIOS DO SUL/RS
            { "4306932", "" }, // ENTRE-IJUÍS/RS
            { "4306973", "905600" }, // EREBANGO/RS
            { "4307005", "903300" }, // ERECHIM/RS
            { "4307054", "913600" }, // ERNESTINA/RS
            { "4307203", "903400" }, // ERVAL GRANDE/RS
            { "4307302", "897100" }, // ERVAL SECO/RS
            { "4307401", "921000" }, // ESMERALDA/RS
            { "4307450", "214500" }, // ESPERANÇA DO SUL/RS
            { "4307500", "918900" }, // ESPUMOSO/RS
            { "4307559", "046900" }, // ESTAÇÃO/RS
            { "4307609", "849800" }, // ESTÂNCIA VELHA/RS
            { "4307708", "849900" }, // ESTEIO/RS
            { "4307807", "921403" }, // ESTRELA/RS
            { "4307815", "182100" }, // ESTRELA VELHA/RS
            { "4307831", "886800" }, // EUGÊNIO DE CASTRO/RS
            { "4307864", "879500" }, // FAGUNDES VARELA/RS
            { "4307906", "879000" }, // FARROUPILHA/RS
            { "4308003", "860400" }, // FAXINAL DO SOTURNO/RS
            { "4308052", "905700" }, // FAXINALZINHO/RS
            { "4308078", "205400" }, // FAZENDA VILANOVA/RS
            { "4308102", "853500" }, // FELIZ/RS
            { "4308201", "879100" }, // FLORES DA CUNHA/RS
            { "4308250", "182200" }, // FLORIANO PEIXOTO/RS
            { "4308300", "882700" }, // FONTOURA XAVIER/RS
            { "4308409", "925000" }, // FORMIGUEIRO/RS
            { "4308433", "224700" }, // FORQUETINHA/RS
            { "4308458", "931800" }, // FORTALEZA DOS VALOS/RS
            { "4308508", "897200" }, // FREDERICO WESTPHALEN/RS
            { "4308607", "879200" }, // GARIBALDI/RS
            { "4308656", "102300" }, // GARRUCHOS/RS
            { "4308706", "903500" }, // GAURAMA/RS
            { "4308805", "866700" }, // GENERAL CÂMARA/RS
            { "4308854", "119600" }, // GENTIL/RS
            { "4308904", "903600" }, // GETÚLIO VARGAS/RS
            { "4309001", "886200" }, // GIRUÁ/RS
            { "4309050", "850900" }, // GLORINHA/RS
            { "4309100", "853600" }, // GRAMADO/RS
            { "4309126", "098500" }, // GRAMADO DOS LOUREIROS/RS
            { "4309159", "095100" }, // GRAMADO XAVIER/RS
            { "4309209", "850000" }, // GRAVATAÍ/RS
            { "4309258", "883700" }, // GUABIJU/RS
            { "4309308", "850100" }, // GUAÍBA/RS
            { "4309407", "882800" }, // GUAPORÉ/RS
            { "4309506", "890200" }, // GUARANI DAS MISSÕES/RS
            { "4309555", "854900" }, // HARMONIA/RS
            { "4307104", "875200" }, // HERVAL/RS
            { "4309571", "182300" }, // HERVEIRAS/RS
            { "4309605", "890300" }, // HORIZONTINA/RS
            { "4309654", "102500" }, // HULHA NEGRA/RS
            { "4309704", "890400" }, // HUMAITÁ/RS
            { "4309753", "861000" }, // IBARAMA/RS
            { "4309803", "903700" }, // IBIAÇÁ/RS
            { "4309902", "921100" }, // IBIRAIARAS/RS
            { "4309951", "919300" }, // IBIRAPUITÃ/RS
            { "4310009", "931900" }, // IBIRUBÁ/RS
            { "4310108", "913003" }, // IGREJINHA/RS
            { "4310207", "911400" }, // IJUÍ/RS
            { "4310306", "882900" }, // ILÓPOLIS/RS
            { "4310330", "046600" }, // IMBÉ/RS
            { "4310363", "046400" }, // IMIGRANTE/RS
            { "4310405", "890500" }, // INDEPENDÊNCIA/RS
            { "4310413", "101400" }, // INHACORÁ/RS
            { "4310439", "921600" }, // IPÊ/RS
            { "4310462", "049000" }, // IPIRANGA DO SUL/RS
            { "4310504", "897300" }, // IRAÍ/RS
            { "4310538", "182400" }, // ITAARA/RS
            { "4310553", "047300" }, // ITACURUBI/RS
            { "4310579", "100700" }, // ITAPUCÁ/RS
            { "4310603", "928000" }, // ITAQUI/RS
            { "4310652", "864002" }, // ITATI/RS
            { "4310702", "903800" }, // ITATIBA DO SUL/RS
            { "4310751", "932600" }, // IVORÁ/RS
            { "4310801", "853800" }, // IVOTI/RS
            { "4310850", "913700" }, // JABOTICABA/RS
            { "4310876", "220100" }, // JACUIZINHO/RS
            { "4310900", "903900" }, // JACUTINGA/RS
            { "4311007", "875300" }, // JAGUARÃO/RS
            { "4311106", "925100" }, // JAGUARI/RS
            { "4311122", "921700" }, // JAQUIRANA/RS
            { "4311130", "182500" }, // JARI/RS
            { "4311155", "932000" }, // JÓIA/RS
            { "4311205", "932100" }, // JÚLIO DE CASTILHOS/RS
            { "4311239", "227000" }, // LAGOA BONITA DO SUL/RS
            { "4311270", "119700" }, // LAGOA DOS TRÊS CANTOS/RS
            { "4311304", "921200" }, // LAGOA VERMELHA/RS
            { "4311254", "919400" }, // LAGOÃO/RS
            { "4311403", "857600" }, // LAJEADO/RS
            { "4311429", "119800" }, // LAJEADO DO BUGRE/RS
            { "4311502", "876500" }, // LAVRAS DO SUL/RS
            { "4311601", "897400" }, // LIBERATO SALZANO/RS
            { "4311627", "119900" }, // LINDOLFO COLLOR/RS
            { "4311643", "120000" }, // LINHA NOVA/RS
            { "4311718", "205500" }, // MAÇAMBARÁ/RS
            { "4311700", "904000" }, // MACHADINHO/RS
            { "4311734", "205600" }, // MAMPITUBA/RS
            { "4311759", "120100" }, // MANOEL VIANA/RS
            { "4311775", "096700" }, // MAQUINÉ/RS
            { "4311791", "101200" }, // MARATÁ/RS
            { "4311809", "904100" }, // MARAU/RS
            { "4311908", "904200" }, // MARCELINO RAMOS/RS
            { "4311981", "094500" }, // MARIANA PIMENTEL/RS
            { "4312005", "904300" }, // MARIANO MORO/RS
            { "4312054", "182600" }, // MARQUES DE SOUZA/RS
            { "4312104", "925200" }, // MATA/RS
            { "4312138", "122400" }, // MATO CASTELHANO/RS
            { "4312153", "100800" }, // MATO LEITÃO/RS
            { "4312179", "220200" }, // MATO QUEIMADO/RS
            { "4312203", "904400" }, // MAXIMILIANO DE ALMEIDA/RS
            { "4312252", "095500" }, // MINAS DO LEÃO/RS
            { "4312302", "897500" }, // MIRAGUAÍ/RS
            { "4312351", "883800" }, // MONTAURI/RS
            { "4312377", "205700" }, // MONTE ALEGRE DOS CAMPOS/RS
            { "4312385", "122500" }, // MONTE BELO DO SUL/RS
            { "4312401", "853900" }, // MONTENEGRO/RS
            { "4312427", "097200" }, // MORMAÇO/RS
            { "4312443", "122600" }, // MORRINHOS DO SUL/RS
            { "4312450", "871100" }, // MORRO REDONDO/RS
            { "4312476", "099400" }, // MORRO REUTER/RS
            { "4312500", "873600" }, // MOSTARDAS/RS
            { "4312609", "857700" }, // MUÇUM/RS
            { "4312617", "182700" }, // MUITOS CAPÕES/RS
            { "4312625", "097300" }, // MULITERNO/RS
            { "4312658", "917600" }, // NÃO-ME-TOQUE/RS
            { "4312674", "097400" }, // NICOLAU VERGUEIRO/RS
            { "4312708", "897600" }, // NONOAI/RS
            { "4312757", "883900" }, // NOVA ALVORADA/RS
            { "4312807", "883000" }, // NOVA ARAÇÁ/RS
            { "4312906", "883100" }, // NOVA BASSANO/RS
            { "4312955", "127700" }, // NOVA BOA VISTA/RS
            { "4313003", "857800" }, // NOVA BRÉSCIA/RS
            { "4313011", "205800" }, // NOVA CANDELÁRIA/RS
            { "4313037", "047100" }, // NOVA ESPERANÇA DO SUL/RS
            { "4313060", "047000" }, // NOVA HARTZ/RS
            { "4313086", "099300" }, // NOVA PÁDUA/RS
            { "4313102", "860500" }, // NOVA PALMA/RS
            { "4313201", "854000" }, // NOVA PETRÓPOLIS/RS
            { "4313300", "883200" }, // NOVA PRATA/RS
            { "4313334", "206500" }, // NOVA RAMADA/RS
            { "4313359", "048500" }, // NOVA ROMA DO SUL/RS
            { "4313375", "128200" }, // NOVA SANTA RITA/RS
            { "4313490", "128500" }, // NOVO BARREIRO/RS
            { "4313391", "206600" }, // NOVO CABRAIS/RS
            { "4313409", "850200" }, // NOVO HAMBURGO/RS
            { "4313425", "128300" }, // NOVO MACHADO/RS
            { "4313441", "128400" }, // NOVO TIRADENTES/RS
            { "4313466", "227100" }, // NOVO XINGU/RS
            { "4313508", "864000" }, // OSÓRIO/RS
            { "4313607", "904500" }, // PAIM FILHO/RS
            { "4313656", "864400" }, // PALMARES DO SUL/RS
            { "4313706", "913300" }, // PALMEIRA DAS MISSÕES/RS
            { "4313805", "897700" }, // PALMITINHO/RS
            { "4313904", "911500" }, // PANAMBI/RS
            { "4313953", "047200" }, // PÂNTANO GRANDE/RS
            { "4314001", "883300" }, // PARAÍ/RS
            { "4314027", "867200" }, // PARAÍSO DO SUL/RS
            { "4314035", "101300" }, // PARECI NOVO/RS
            { "4314050", "854100" }, // PAROBÉ/RS
            { "4314068", "182800" }, // PASSA SETE/RS
            { "4314076", "095200" }, // PASSO DO SOBRADO/RS
            { "4314100", "913400" }, // PASSO FUNDO/RS
            { "4314134", "220300" }, // PAULO BENTO/RS
            { "4314159", "867300" }, // PAVERAMA/RS
            { "4314175", "220400" }, // PEDRAS ALTAS/RS
            { "4314209", "870600" }, // PEDRO OSÓRIO/RS
            { "4314308", "911600" }, // PEJUÇARA/RS
            { "4314407", "870700" }, // PELOTAS/RS
            { "4314423", "099500" }, // PICADA CAFÉ/RS
            { "4314456", "095301" }, // PINHAL/RS
            { "4314464", "223800" }, // PINHAL DA SERRA/RS
            { "4314472", "096800" }, // PINHAL GRANDE/RS
            { "4314498", "128600" }, // PINHEIRINHO DO VALE/RS
            { "4314506", "876600" }, // PINHEIRO MACHADO/RS
            { "4314555", "046700" }, // PIRAPÓ/RS
            { "4314605", "876700" }, // PIRATINI/RS
            { "4314704", "897800" }, // PLANALTO/RS
            { "4314753", "855000" }, // POÇO DAS ANTAS/RS
            { "4314779", "097500" }, // PONTÃO/RS
            { "4314787", "099100" }, // PONTE PRETA/RS
            { "4314803", "850300" }, // PORTÃO/RS
            { "4314902", "850400" }, // PORTO ALEGRE/RS
            { "4315008", "890600" }, // PORTO LUCENA/RS
            { "4315057", "101900" }, // PORTO MAUÁ/RS
            { "4315073", "129400" }, // PORTO VERA CRUZ/RS
            { "4315107", "890700" }, // PORTO XAVIER/RS
            { "4315131", "858200" }, // POUSO NOVO/RS
            { "4315149", "129500" }, // PRESIDENTE LUCENA/RS
            { "4315156", "047500" }, // PROGRESSO/RS
            { "4315172", "884000" }, // PROTÁSIO ALVES/RS
            { "4315206", "883400" }, // PUTINGA/RS
            { "4315305", "928100" }, // QUARAÍ/RS
            { "4315313", "238000" }, // QUATRO IRMÃOS/RS
            { "4315321", "096900" }, // QUEVEDOS/RS
            { "4315354", "932700" }, // QUINZE DE NOVEMBRO/RS
            { "4315404", "897900" }, // REDENTORA/RS
            { "4315453", "858300" }, // RELVADO/RS
            { "4315503", "886308" }, // RESTINGA SECA/RS
            { "4315552", "098600" }, // RIO DOS ÍNDIOS/RS
            { "4315602", "873700" }, // RIO GRANDE/RS
            { "4315701", "866800" }, // RIO PARDO/RS
            { "4315750", "855100" }, // RIOZINHO/RS
            { "4315800", "857900" }, // ROCA SALES/RS
            { "4315909", "898000" }, // RODEIO BONITO/RS
            { "4315958", "224000" }, // ROLADOR/RS
            { "4316006", "854200" }, // ROLANTE/RS
            { "4316105", "898100" }, // RONDA ALTA/RS
            { "4316204", "898200" }, // RONDINHA/RS
            { "4316303", "890800" }, // ROQUE GONZALES/RS
            { "4316402", "928200" }, // ROSÁRIO DO SUL/RS
            { "4316428", "099200" }, // SAGRADA FAMÍLIA/RS
            { "4316436", "932800" }, // SALDANHA MARINHO/RS
            { "4316451", "919000" }, // SALTO DO JACUÍ/RS
            { "4316477", "101700" }, // SALVADOR DAS MISSÕES/RS
            { "4316501", "854300" }, // SALVADOR DO SUL/RS
            { "4316600", "904600" }, // SANANDUVA/RS
            { "4316709", "932200" }, // SANTA BÁRBARA DO SUL/RS
            { "4316733", "227200" }, // SANTA CECÍLIA DO SUL/RS
            { "4316758", "100900" }, // SANTA CLARA DO SUL/RS
            { "4316808", "860600" }, // SANTA CRUZ DO SUL/RS
            { "4316972", "227700" }, // SANTA MARGARIDA DO SUL/RS
            { "4316907", "925400" }, // SANTA MARIA/RS
            { "4316956", "855200" }, // SANTA MARIA DO HERVAL/RS
            { "4317202", "864407" }, // SANTA ROSA/RS
            { "4317251", "238100" }, // SANTA TEREZA/RS
            { "4317301", "875400" }, // SANTA VITÓRIA DO PALMAR/RS
            { "4317004", "876800" }, // SANTANA DA BOA VISTA/RS
            { "4317103", "928300" }, // SANTANA DO LIVRAMENTO/RS
            { "4317400", "932300" }, // SANTIAGO/RS
            { "4317509", "886300" }, // SANTO ÂNGELO/RS
            { "4317608", "864100" }, // SANTO ANTÔNIO DA PATRULHA/RS
            { "4317707", "928400" }, // SANTO ANTÔNIO DAS MISSÕES/RS
            { "4317558", "129600" }, // SANTO ANTÔNIO DO PALMA/RS
            { "4317756", "129700" }, // SANTO ANTÔNIO DO PLANALTO/RS
            { "4317806", "913500" }, // SANTO AUGUSTO/RS
            { "4317905", "891000" }, // SANTO CRISTO/RS
            { "4317954", "129800" }, // SANTO EXPEDITO DO SUL/RS
            { "4318002", "928500" }, // SÃO BORJA/RS
            { "4318051", "884100" }, // SÃO DOMINGOS DO SUL/RS
            { "4318101", "932400" }, // SÃO FRANCISCO DE ASSIS/RS
            { "4318200", "921300" }, // SÃO FRANCISCO DE PAULA/RS
            { "4318309", "897801" }, // SÃO GABRIEL/RS
            { "4318408", "866900" }, // SÃO JERÔNIMO/RS
            { "4318424", "905800" }, // SÃO JOÃO DA URTIGA/RS
            { "4318432", "097000" }, // SÃO JOÃO DO POLESINE/RS
            { "4318440", "884200" }, // SÃO JORGE/RS
            { "4318457", "130900" }, // SÃO JOSÉ DAS MISSÕES/RS
            { "4318465", "884300" }, // SÃO JOSÉ DO HERVAL/RS
            { "4318481", "855300" }, // SÃO JOSÉ DO HORTÊNCIO/RS
            { "4318499", "101600" }, // SÃO JOSÉ DO INHACORÁ/RS
            { "4318507", "873800" }, // SÃO JOSÉ DO NORTE/RS
            { "4318606", "904700" }, // SÃO JOSÉ DO OURO/RS
            { "4318614", "227800" }, // SÃO JOSÉ DO SUL/RS
            { "4318622", "131000" }, // SÃO JOSÉ DOS AUSENTES/RS
            { "4318705", "850500" }, // SÃO LEOPOLDO/RS
            { "4318804", "870800" }, // SÃO LOURENÇO DO SUL/RS
            { "4318903", "886400" }, // SÃO LUIZ GONZAGA/RS
            { "4319000", "879300" }, // SÃO MARCOS/RS
            { "4319109", "898300" }, // SÃO MARTINHO/RS
            { "4319125", "131100" }, // SÃO MARTINHO DA SERRA/RS
            { "4319158", "052300" }, // SÃO MIGUEL DAS MISSÕES/RS
            { "4319208", "886500" }, // SÃO NICOLAU/RS
            { "4319307", "891100" }, // SÃO PAULO DAS MISSÕES/RS
            { "4319356", "131200" }, // SÃO PEDRO DA SERRA/RS
            { "4319364", "227900" }, // SÃO PEDRO DAS MISSÕES/RS
            { "4319372", "101800" }, // SÃO PEDRO DO BUTIÁ/RS
            { "4319406", "925500" }, // SÃO PEDRO DO SUL/RS
            { "4319505", "854400" }, // SÃO SEBASTIÃO DO CAÍ/RS
            { "4319604", "876900" }, // SÃO SEPÉ/RS
            { "4319703", "890503" }, // SÃO VALENTIM/RS
            { "4319711", "131300" }, // SÃO VALENTIM DO SUL/RS
            { "4319737", "131400" }, // SÃO VALÉRIO DO SUL/RS
            { "4319752", "855400" }, // SÃO VENDELINO/RS
            { "4319802", "925600" }, // SÃO VICENTE DO SUL/RS
            { "4319901", "850600" }, // SAPIRANGA/RS
            { "4320008", "850700" }, // SAPUCAIA DO SUL/RS
            { "4320107", "898400" }, // SARANDI/RS
            { "4320206", "898500" }, // SEBERI/RS
            { "4320230", "891900" }, // SEDE NOVA/RS
            { "4320263", "047600" }, // SEGREDO/RS
            { "4320305", "917700" }, // SELBACH/RS
            { "4320321", "206700" }, // SENADOR SALGADO FILHO/RS
            { "4320354", "131500" }, // SENTINELA DO SUL/RS
            { "4320404", "883500" }, // SERAFINA CORRÊA/RS
            { "4320453", "101000" }, // SÉRIO/RS
            { "4320503", "904900" }, // SERTÃO/RS
            { "4320552", "133000" }, // SERTÃO SANTANA/RS
            { "4320578", "890901" }, // SETE DE SETEMBRO/RS
            { "4320602", "905000" }, // SEVERIANO DE ALMEIDA/RS
            { "4320651", "925700" }, // SILVEIRA MARTINS/RS
            { "4320677", "095300" }, // SINIMBU/RS
            { "4320701", "860700" }, // SOBRADINHO/RS
            { "4320800", "919100" }, // SOLEDADE/RS
            { "4320859", "183000" }, // TABAÍ/RS
            { "4320909", "905100" }, // TAPEJARA/RS
            { "4321006", "873903" }, // TAPERA/RS
            { "4321105", "870900" }, // TAPES/RS
            { "4321204", "854500" }, // TAQUARA/RS
            { "4321303", "867000" }, // TAQUARI/RS
            { "4321329", "049100" }, // TAQUARUÇU DO SUL/RS
            { "4321352", "873900" }, // TAVARES/RS
            { "4321402", "891200" }, // TENENTE PORTELA/RS
            { "4321436", "864500" }, // TERRA DE AREIA/RS
            { "4321451", "858000" }, // TEUTÔNIA/RS
            { "4321469", "228000" }, // TIO HUGO/RS
            { "4321477", "133100" }, // TIRADENTES DO SUL/RS
            { "4321493", "183100" }, // TOROPI/RS
            { "4321501", "864200" }, // TORRES/RS
            { "4321600", "864300" }, // TRAMANDAÍ/RS
            { "4321626", "101100" }, // TRAVESSEIRO/RS
            { "4321634", "905900" }, // TRÊS ARROIOS/RS
            { "4321667", "864600" }, // TRÊS CACHOEIRAS/RS
            { "4321709", "854600" }, // TRÊS COROAS/RS
            { "4321808", "891300" }, // TRÊS DE MAIO/RS
            { "4321832", "096500" }, // TRÊS FORQUILHAS/RS
            { "4321857", "898700" }, // TRÊS PALMEIRAS/RS
            { "4321907", "891400" }, // TRÊS PASSOS/RS
            { "4321956", "052100" }, // TRINDADE DO SUL/RS
            { "4322004", "867100" }, // TRIUNFO/RS
            { "4322103", "891500" }, // TUCUNDUVA/RS
            { "4322152", "919500" }, // TUNAS/RS
            { "4322186", "133200" }, // TUPANCI DO SUL/RS
            { "4322202", "932500" }, // TUPANCIRETÃ/RS
            { "4322251", "855500" }, // TUPANDI/RS
            { "4322301", "891600" }, // TUPARENDI/RS
            { "4322327", "206800" }, // TURUÇU/RS
            { "4322343", "183200" }, // UBIRETAMA/RS
            { "4322350", "133300" }, // UNIÃO DA SERRA/RS
            { "4322376", "183300" }, // UNISTALDA/RS
            { "4322400", "928700" }, // URUGUAIANA/RS
            { "4322509", "921400" }, // VACARIA/RS
            { "4322533", "134600" }, // VALE DO SOL/RS
            { "4322541", "134700" }, // VALE REAL/RS
            { "4322525", "214600" }, // VALE VERDE/RS
            { "4322558", "884400" }, // VANINI/RS
            { "4322608", "860800" }, // VENÂNCIO AIRES/RS
            { "4322707", "860900" }, // VERA CRUZ/RS
            { "4322806", "879400" }, // VERANÓPOLIS/RS
            { "4322855", "183400" }, // VESPASIANO CORREA/RS
            { "4322905", "905200" }, // VIADUTOS/RS
            { "4323002", "850800" }, // VIAMÃO/RS
            { "4323101", "898600" }, // VICENTE DUTRA/RS
            { "4323200", "917900" }, // VICTOR GRAEFF/RS
            { "4323309", "879600" }, // VILA FLORES/RS
            { "4323358", "206900" }, // VILA LANGARO/RS
            { "4323408", "054000" }, // VILA MARIA/RS
            { "4323457", "134800" }, // VILA NOVA DO SUL/RS
            { "4323507", "052200" }, // VISTA ALEGRE/RS
            { "4323606", "048600" }, // VISTA ALEGRE DO PRATA/RS
            { "4323705", "892000" }, // VISTA GAÚCHA/RS
            { "4323754", "127500" }, // VITÓRIA DAS MISSÕES/RS
            { "4323770", "228500" }, // WESTFÁLIA/RS
            { "4323804", "127600" }, // XANGRI-LÁ/RS
            { "4200051", "045000" }, // ABDON BATISTA/SC
            { "4200101", "837900" }, // ABELARDO LUZ/SC
            { "4200200", "811700" }, // AGROLÂNDIA/SC
            { "4200309", "811800" }, // AGRONÔMICA/SC
            { "4200408", "831700" }, // ÁGUA DOCE/SC
            { "4200507", "838000" }, // ÁGUAS DE CHAPECÓ/SC
            { "4200556", "091700" }, // ÁGUAS FRIAS/SC
            { "4200606", "817100" }, // ÁGUAS MORNAS/SC
            { "4200705", "817200" }, // ALFREDO WAGNER/SC
            { "4200754", "207000" }, // ALTO BELA VISTA/SC
            { "4200804", "838100" }, // ANCHIETA/SC
            { "4200903", "817300" }, // ANGELINA/SC
            { "4201000", "828900" }, // ANITA GARIBALDI/SC
            { "4201109", "817400" }, // ANITÁPOLIS/SC
            { "4201208", "817500" }, // ANTÔNIO CARLOS/SC
            { "4201257", "809800" }, // APIÚNA/SC
            { "4201273", "092700" }, // ARABUTÃ/SC
            { "4201307", "805300" }, // ARAQUARI/SC
            { "4201406", "824100" }, // ARARANGUÁ/SC
            { "4201505", "820800" }, // ARMAZÉM/SC
            { "4201604", "831800" }, // ARROIO TRINTA/SC
            { "4201653", "092800" }, // ARVOREDO/SC
            { "4201703", "808200" }, // ASCURRA/SC
            { "4201802", "811900" }, // ATALANTA/SC
            { "4201901", "812000" }, // AURORA/SC
            { "4201950", "207100" }, // BALNEÁRIO ARROIO DO SILVA/SC
            { "4202057", "228700" }, // BALNEÁRIO BARRA DO SUL/SC
            { "4202008", "807100" }, // BALNEÁRIO CAMBORIÚ/SC
            { "4202073", "207200" }, // BALNEÁRIO GAIVOTA/SC
            { "4212809", "807800" }, // BALNEÁRIO PIÇARRAS/SC
            { "4202081", "183500" }, // BANDEIRANTE/SC
            { "4202099", "183600" }, // BARRA BONITA/SC
            { "4202107", "805400" }, // BARRA VELHA/SC
            { "4202131", "183700" }, // BELA VISTA DO TOLDO/SC
            { "4202156", "093600" }, // BELMONTE/SC
            { "4202206", "808300" }, // BENEDITO NOVO/SC
            { "4202305", "814300" }, // BIGUAÇU/SC
            { "4202404", "808400" }, // BLUMENAU/SC
            { "4202438", "183800" }, // BOCAÍNA DO SUL/SC
            { "4202503", "826800" }, // BOM JARDIM DA SERRA/SC
            { "4202537", "183900" }, // BOM JESUS/SC
            { "4202578", "184600" }, // BOM JESUS DO OESTE/SC
            { "4202602", "826900" }, // BOM RETIRO/SC
            { "4202453", "088500" }, // BOMBINHAS/SC
            { "4202701", "808500" }, // BOTUVERÁ/SC
            { "4202800", "820900" }, // BRAÇO DO NORTE/SC
            { "4202859", "089900" }, // BRAÇO DO TROMBUDO/SC
            { "4202875", "207300" }, // BRUNÓPOLIS/SC
            { "4202909", "808600" }, // BRUSQUE/SC
            { "4203006", "831900" }, // CAÇADOR/SC
            { "4203105", "838200" }, // CAIBI/SC
            { "4203154", "089700" }, // CALMON/SC
            { "4203204", "807200" }, // CAMBORIÚ/SC
            { "4203303", "846400" }, // CAMPO ALEGRE/SC
            { "4203402", "829000" }, // CAMPO BELO DO SUL/SC
            { "4203501", "838300" }, // CAMPO ERÊ/SC
            { "4203600", "829100" }, // CAMPOS NOVOS/SC
            { "4203709", "817600" }, // CANELINHA/SC
            { "4203808", "846500" }, // CANOINHAS/SC
            { "4203253", "184700" }, // CAPÃO ALTO/SC
            { "4203907", "832000" }, // CAPINZAL/SC
            { "4203956", "117700" }, // CAPIVARI DE BAIXO/SC
            { "4204004", "832100" }, // CATANDUVAS/SC
            { "4204103", "838400" }, // CAXAMBU DO SUL/SC
            { "4204152", "046300" }, // CELSO RAMOS/SC
            { "4204178", "091200" }, // CERRO NEGRO/SC
            { "4204194", "207400" }, // CHAPADÃO DO LAGEADO/SC
            { "4204202", "838500" }, // CHAPECÓ/SC
            { "4204251", "117800" }, // COCAL DO SUL/SC
            { "4204301", "832200" }, // CONCÓRDIA/SC
            { "4204350", "091800" }, // CORDILHEIRA ALTA/SC
            { "4204400", "838600" }, // CORONEL FREITAS/SC
            { "4204459", "093200" }, // CORONEL MARTINS/SC
            { "4204558", "827000" }, // CORREIA PINTO/SC
            { "4204509", "805500" }, // CORUPÁ/SC
            { "4204608", "821000" }, // CRICIÚMA/SC
            { "4204707", "838700" }, // CUNHA PORÃ/SC
            { "4204756", "207500" }, // CUNHATAÍ/SC
            { "4204806", "829200" }, // CURITIBANOS/SC
            { "4204905", "838800" }, // DESCANSO/SC
            { "4205001", "838900" }, // DIONÍSIO CERQUEIRA/SC
            { "4205100", "810600" }, // DONA EMMA/SC
            { "4205159", "809900" }, // DOUTOR PEDRINHO/SC
            { "4205175", "184800" }, // ENTRE RIOS/SC
            { "4205191", "184900" }, // ERMO/SC
            { "4205209", "832300" }, // ERVAL VELHO/SC
            { "4205308", "839000" }, // FAXINAL DOS GUEDES/SC
            { "4205357", "185000" }, // FLOR DO SERTÃO/SC
            { "4205407", "814400" }, // FLORIANÓPOLIS/SC
            { "4205431", "117900" }, // FORMOSA DO SUL/SC
            { "4205456", "045100" }, // FORQUILHINHA/SC
            { "4205506", "832400" }, // FRAIBURGO/SC
            { "4205555", "185100" }, // FREI ROGÉRIO/SC
            { "4205605", "839100" }, // GALVÃO/SC
            { "4205704", "814500" }, // GAROPABA/SC
            { "4205803", "805600" }, // GARUVA/SC
            { "4205902", "808700" }, // GASPAR/SC
            { "4206009", "814600" }, // GOVERNADOR CELSO RAMOS/SC
            { "4206108", "821100" }, // GRÃO PARÁ/SC
            { "4206207", "821200" }, // GRAVATAL/SC
            { "4206306", "808800" }, // GUABIRUBA/SC
            { "4206405", "839200" }, // GUARACIABA/SC
            { "4206504", "805700" }, // GUARAMIRIM/SC
            { "4206603", "839300" }, // GUARUJÁ DO SUL/SC
            { "4206652", "091900" }, // GUATAMBÚ/SC
            { "4206702", "832500" }, // HERVAL D'OESTE/SC
            { "4206751", "185200" }, // IBIAM/SC
            { "4206801", "832600" }, // IBICARÉ/SC
            { "4206900", "810700" }, // IBIRAMA/SC
            { "4207007", "824200" }, // IÇARA/SC
            { "4207106", "807300" }, // ILHOTA/SC
            { "4207205", "819700" }, // IMARUÍ/SC
            { "4207304", "819800" }, // IMBITUBA/SC
            { "4207403", "812100" }, // IMBUIA/SC
            { "4207502", "808900" }, // INDAIAL/SC
            { "4207577", "185300" }, // IOMERÊ/SC
            { "4207601", "832700" }, // IPIRA/SC
            { "4207650", "046000" }, // IPORÃ DO OESTE/SC
            { "4207684", "093300" }, // IPUAÇU/SC
            { "4207700", "832800" }, // IPUMIRIM/SC
            { "4207759", "044800" }, // IRACEMINHA/SC
            { "4207809", "832900" }, // IRANI/SC
            { "4207858", "092200" }, // IRATI/SC
            { "4207908", "846600" }, // IRINEÓPOLIS/SC
            { "4208005", "833000" }, // ITÁ/SC
            { "4208104", "846700" }, // ITAIÓPOLIS/SC
            { "4208203", "807400" }, // ITAJAÍ/SC
            { "4208302", "807500" }, // ITAPEMA/SC
            { "4208401", "839400" }, // ITAPIRANGA/SC
            { "4208450", "805601" }, // ITAPOÁ/SC
            { "4208500", "812200" }, // ITUPORANGA/SC
            { "4208609", "833100" }, // JABORÁ/SC
            { "4208708", "825500" }, // JACINTO MACHADO/SC
            { "4208807", "824300" }, // JAGUARUNA/SC
            { "4208906", "805800" }, // JARAGUÁ DO SUL/SC
            { "4208955", "092300" }, // JARDINÓPOLIS/SC
            { "4209003", "833200" }, // JOAÇABA/SC
            { "4209102", "805900" }, // JOINVILLE/SC
            { "4209151", "045200" }, // JOSÉ BOITEUX/SC
            { "4209177", "185400" }, // JUPIÁ/SC
            { "4209201", "833300" }, // LACERDÓPOLIS/SC
            { "4209300", "827100" }, // LAGES/SC
            { "4209409", "819900" }, // LAGUNA/SC
            { "4209458", "" }, // LAJEADO GRANDE/SC
            { "4209508", "812300" }, // LAURENTINO/SC
            { "4209607", "821300" }, // LAURO MÜLLER/SC
            { "4209706", "829300" }, // LEBON RÉGIS/SC
            { "4209805", "817700" }, // LEOBERTO LEAL/SC
            { "4209854", "045500" }, // LINDÓIA DO SUL/SC
            { "4209904", "812400" }, // LONTRAS/SC
            { "4210001", "809000" }, // LUIZ ALVES/SC
            { "4210035", "185500" }, // LUZERNA/SC
            { "4210050", "091600" }, // MACIEIRA/SC
            { "4210100", "846800" }, // MAFRA/SC
            { "4210209", "817800" }, // MAJOR GERCINO/SC
            { "4210308", "846900" }, // MAJOR VIEIRA/SC
            { "4210407", "824400" }, // MARACAJÁ/SC
            { "4210506", "839500" }, // MARAVILHA/SC
            { "4210555", "841300" }, // MAREMA/SC
            { "4210605", "809100" }, // MASSARANDUBA/SC
            { "4210704", "847000" }, // MATOS COSTA/SC
            { "4210803", "825600" }, // MELEIRO/SC
            { "4210852", "090000" }, // MIRIM DOCE/SC
            { "4210902", "839600" }, // MODELO/SC
            { "4211009", "839700" }, // MONDAÍ/SC
            { "4211058", "090900" }, // MONTE CARLO/SC
            { "4211108", "847100" }, // MONTE CASTELO/SC
            { "4211207", "821400" }, // MORRO DA FUMAÇA/SC
            { "4211256", "088600" }, // MORRO GRANDE/SC
            { "4211306", "807600" }, // NAVEGANTES/SC
            { "4211405", "839800" }, // NOVA ERECHIM/SC
            { "4211454", "118000" }, // NOVA ITABERABA/SC
            { "4211504", "817900" }, // NOVA TRENTO/SC
            { "4211603", "825700" }, // NOVA VENEZA/SC
            { "4211652", "092400" }, // NOVO HORIZONTE/SC
            { "4211702", "821500" }, // ORLEANS/SC
            { "4211751", "827200" }, // OTACÍLIO COSTA/SC
            { "4211801", "833400" }, // OURO/SC
            { "4211850", "093500" }, // OURO VERDE/SC
            { "4211876", "185600" }, // PAIAL/SC
            { "4211892", "185700" }, // PAINEL/SC
            { "4211900", "814700" }, // PALHOÇA/SC
            { "4212007", "839900" }, // PALMA SOLA/SC
            { "4212056", "187100" }, // PALMEIRA/SC
            { "4212106", "840000" }, // PALMITOS/SC
            { "4212205", "847200" }, // PAPANDUVA/SC
            { "4212239", "093700" }, // PARAÍSO/SC
            { "4212254", "088700" }, // PASSO DE TORRES/SC
            { "4212270", "118100" }, // PASSOS MAIA/SC
            { "4212304", "814800" }, // PAULO LOPES/SC
            { "4212403", "821600" }, // PEDRAS GRANDES/SC
            { "4212502", "807700" }, // PENHA/SC
            { "4212601", "833500" }, // PERITIBA/SC
            { "4212700", "812500" }, // PETROLÂNDIA/SC
            { "4212908", "840100" }, // PINHALZINHO/SC
            { "4213005", "833600" }, // PINHEIRO PRETO/SC
            { "4213104", "833700" }, // PIRATUBA/SC
            { "4213153", "092500" }, // PLANALTO ALEGRE/SC
            { "4213203", "809200" }, // POMERODE/SC
            { "4213302", "829400" }, // PONTE ALTA/SC
            { "4213351", "091300" }, // PONTE ALTA DO NORTE/SC
            { "4213401", "833800" }, // PONTE SERRADA/SC
            { "4213500", "814900" }, // PORTO BELO/SC
            { "4213609", "847300" }, // PORTO UNIÃO/SC
            { "4213708", "812600" }, // POUSO REDONDO/SC
            { "4213807", "825800" }, // PRAIA GRANDE/SC
            { "4213906", "833900" }, // PRESIDENTE CASTELO BRANCO/SC
            { "4214003", "810800" }, // PRESIDENTE GETÚLIO/SC
            { "4214102", "809300" }, // PRESIDENTE NEREU/SC
            { "4214151", "187200" }, // PRINCESA/SC
            { "4214201", "840200" }, // QUILOMBO/SC
            { "4214300", "818000" }, // RANCHO QUEIMADO/SC
            { "4214409", "834000" }, // RIO DAS ANTAS/SC
            { "4214508", "812700" }, // RIO DO CAMPO/SC
            { "4214607", "812800" }, // RIO DO OESTE/SC
            { "4214805", "812900" }, // RIO DO SUL/SC
            { "4214706", "809400" }, // RIO DOS CEDROS/SC
            { "4214904", "821700" }, // RIO FORTUNA/SC
            { "4215000", "847400" }, // RIO NEGRINHO/SC
            { "4215059", "091400" }, // RIO RUFINO/SC
            { "4215075", "093800" }, // RIQUEZA/SC
            { "4215109", "809500" }, // RODEIO/SC
            { "4215208", "840300" }, // ROMELÂNDIA/SC
            { "4215307", "813000" }, // SALETE/SC
            { "4215356", "207600" }, // SALTINHO/SC
            { "4215406", "834100" }, // SALTO VELOSO/SC
            { "4215455", "088800" }, // SANGÃO/SC
            { "4215505", "829500" }, // SANTA CECÍLIA/SC
            { "4215554", "094200" }, // SANTA HELENA/SC
            { "4215604", "821800" }, // SANTA ROSA DE LIMA/SC
            { "4215653", "046200" }, // SANTA ROSA DO SUL/SC
            { "4215679", "118200" }, // SANTA TEREZINHA/SC
            { "4215687", "207700" }, // SANTA TEREZINHA DO PROGRESSO/SC
            { "4215695", "207800" }, // SANTIAGO DO SUL/SC
            { "4215703", "815000" }, // SANTO AMARO DA IMPERATRIZ/SC
            { "4215802", "847500" }, // SÃO BENTO DO SUL/SC
            { "4215752", "207900" }, // SÃO BERNARDINO/SC
            { "4215901", "818100" }, // SÃO BONIFÁCIO/SC
            { "4216008", "840400" }, // SÃO CARLOS/SC
            { "4216057", "091500" }, // SÃO CRISTÓVÃO DO SUL/SC
            { "4216107", "840500" }, // SÃO DOMINGOS/SC
            { "4216206", "806000" }, // SÃO FRANCISCO DO SUL/SC
            { "4216305", "818200" }, // SÃO JOÃO BATISTA/SC
            { "4216354", "089800" }, // SÃO JOÃO DO ITAPERIÚ/SC
            { "4216255", "094300" }, // SÃO JOÃO DO OESTE/SC
            { "4216404", "824500" }, // SÃO JOÃO DO SUL/SC
            { "4216503", "827300" }, // SÃO JOAQUIM/SC
            { "4216602", "815100" }, // SÃO JOSÉ/SC
            { "4216701", "840600" }, // SÃO JOSÉ DO CEDRO/SC
            { "4216800", "829600" }, // SÃO JOSÉ DO CERRITO/SC
            { "4216909", "840700" }, // SÃO LOURENÇO DO OESTE/SC
            { "4217006", "821900" }, // SÃO LUDGERO/SC
            { "4217105", "822000" }, // SÃO MARTINHO/SC
            { "4217154", "094400" }, // SÃO MIGUEL DA BOA VISTA/SC
            { "4217204", "" }, // SÃO MIGUEL DO OESTE/SC
            { "4217253", "187300" }, // SÃO PEDRO DE ALCÂNTARA/SC
            { "4217303", "840900" }, // SAUDADES/SC
            { "4217402", "806100" }, // SCHROEDER/SC
            { "4217501", "834200" }, // SEARA/SC
            { "4217550", "044900" }, // SERRA ALTA/SC
            { "4217600", "822100" }, // SIDERÓPOLIS/SC
            { "4217709", "824600" }, // SOMBRIO/SC
            { "4217758", "092600" }, // SUL BRASIL/SC
            { "4217808", "813100" }, // TAIÓ/SC
            { "4217907", "834300" }, // TANGARÁ/SC
            { "4217956", "187400" }, // TIGRINHOS/SC
            { "4218004", "815200" }, // TIJUCAS/SC
            { "4218103", "825900" }, // TIMBÉ DO SUL/SC
            { "4218202", "809600" }, // TIMBÓ/SC
            { "4218251", "044700" }, // TIMBÓ GRANDE/SC
            { "4218301", "847600" }, // TRÊS BARRAS/SC
            { "4218350", "187500" }, // TREVISO/SC
            { "4218400", "822200" }, // TREZE DE MAIO/SC
            { "4218509", "834400" }, // TREZE TÍLIAS/SC
            { "4218608", "813200" }, // TROMBUDO CENTRAL/SC
            { "4218707", "822300" }, // TUBARÃO/SC
            { "4218756", "046100" }, // TUNÁPOLIS/SC
            { "4218806", "826000" }, // TURVO/SC
            { "4218855", "045400" }, // UNIÃO DO OESTE/SC
            { "4218905", "827400" }, // URUBICI/SC
            { "4218954", "827500" }, // URUPEMA/SC
            { "4219002", "822400" }, // URUSSANGA/SC
            { "4219101", "841000" }, // VARGEÃO/SC
            { "4219150", "091000" }, // VARGEM/SC
            { "4219176", "091100" }, // VARGEM BONITA/SC
            { "4219200", "809700" }, // VIDAL RAMOS/SC
            { "4219309", "834500" }, // VIDEIRA/SC
            { "4219358", "045300" }, // VÍTOR MEIRELES/SC
            { "4219408", "810900" }, // WITMARSUM/SC
            { "4219507", "841100" }, // XANXERÊ/SC
            { "4219606", "834600" }, // XAVANTINA/SC
            { "4219705", "841200" }, // XAXIM/SC
            { "4219853", "208700" }, // ZORTÉA/SC
            { "2800100", "323000" }, // AMPARO DE SÃO FRANCISCO/SE
            { "2800209", "324200" }, // AQUIDABÃ/SE
            { "2800308", "331300" }, // ARACAJU/SE
            { "2800407", "329300" }, // ARAUÁ/SE
            { "2800506", "328300" }, // AREIA BRANCA/SE
            { "2800605", "331400" }, // BARRA DOS COQUEIROS/SE
            { "2800670", "329400" }, // BOQUIM/SE
            { "2800704", "323100" }, // BREJO GRANDE/SE
            { "2801009", "328400" }, // CAMPO DO BRITO/SE
            { "2801108", "323200" }, // CANHOBA/SE
            { "2801207", "321900" }, // CANINDÉ DE SÃO FRANCISCO/SE
            { "2801306", "326400" }, // CAPELA/SE
            { "2801405", "324300" }, // CARIRA/SE
            { "2801504", "326500" }, // CARMÓPOLIS/SE
            { "2801603", "324400" }, // CEDRO DE SÃO JOÃO/SE
            { "2801702", "330500" }, // CRISTINÁPOLIS/SE
            { "2801900", "324500" }, // CUMBE/SE
            { "2802007", "326600" }, // DIVINA PASTORA/SE
            { "2802106", "331500" }, // ESTÂNCIA/SE
            { "2802205", "324600" }, // FEIRA NOVA/SE
            { "2802304", "324700" }, // FREI PAULO/SE
            { "2802403", "322000" }, // GARARU/SE
            { "2802502", "326700" }, // GENERAL MAYNARD/SE
            { "2802601", "" }, // GRACCHO CARDOSO/SE
            { "2802700", "323300" }, // ILHA DAS FLORES/SE
            { "2802809", "331600" }, // INDIAROBA/SE
            { "2802908", "328500" }, // ITABAIANA/SE
            { "2803005", "329500" }, // ITABAIANINHA/SE
            { "2803104", "324900" }, // ITABI/SE
            { "2803203", "331700" }, // ITAPORANGA D'AJUDA/SE
            { "2803302", "326800" }, // JAPARATUBA/SE
            { "2803401", "325000" }, // JAPOATÃ/SE
            { "2803500", "329600" }, // LAGARTO/SE
            { "2803609", "326900" }, // LARANJEIRAS/SE
            { "2803708", "328600" }, // MACAMBIRA/SE
            { "2803807", "325100" }, // MALHADA DOS BOIS/SE
            { "2803906", "328700" }, // MALHADOR/SE
            { "2804003", "327000" }, // MARUIM/SE
            { "2804102", "328800" }, // MOITA BONITA/SE
            { "2804201", "322100" }, // MONTE ALEGRE DE SERGIPE/SE
            { "2804300", "325200" }, // MURIBECA/SE
            { "2804409", "323400" }, // NEÓPOLIS/SE
            { "2804458", "325300" }, // NOSSA SENHORA APARECIDA/SE
            { "2804508", "322200" }, // NOSSA SENHORA DA GLÓRIA/SE
            { "2804607", "325400" }, // NOSSA SENHORA DAS DORES/SE
            { "2804706", "323500" }, // NOSSA SENHORA DE LOURDES/SE
            { "2804805", "331800" }, // NOSSA SENHORA DO SOCORRO/SE
            { "2804904", "323600" }, // PACATUBA/SE
            { "2805000", "325500" }, // PEDRA MOLE/SE
            { "2805109", "329700" }, // PEDRINHAS/SE
            { "2805208", "325600" }, // PINHÃO/SE
            { "2805307", "327100" }, // PIRAMBU/SE
            { "2805406", "322300" }, // POÇO REDONDO/SE
            { "2805505", "330600" }, // POÇO VERDE/SE
            { "2805604", "322400" }, // PORTO DA FOLHA/SE
            { "2805703", "323700" }, // PROPRIÁ/SE
            { "2805802", "329800" }, // RIACHÃO DO DANTAS/SE
            { "2805901", "327200" }, // RIACHUELO/SE
            { "2806008", "325700" }, // RIBEIRÓPOLIS/SE
            { "2806107", "327300" }, // ROSÁRIO DO CATETE/SE
            { "2806206", "329900" }, // SALGADO/SE
            { "2806305", "331900" }, // SANTA LUZIA DO ITANHY/SE
            { "2806503", "327400" }, // SANTA ROSA DE LIMA/SE
            { "2806404", "115800" }, // SANTANA DO SÃO FRANCISCO/SE
            { "2806602", "327500" }, // SANTO AMARO DAS BROTAS/SE
            { "2806701", "332000" }, // SÃO CRISTÓVÃO/SE
            { "2806800", "328900" }, // SÃO DOMINGOS/SE
            { "2806909", "325800" }, // SÃO FRANCISCO/SE
            { "2807006", "325900" }, // SÃO MIGUEL DO ALEIXO/SE
            { "2807105", "330000" }, // SIMÃO DIAS/SE
            { "2807204", "327600" }, // SIRIRI/SE
            { "2807303", "323800" }, // TELHA/SE
            { "2807402", "330700" }, // TOBIAS BARRETO/SE
            { "2807501", "330800" }, // TOMAR DO GERU/SE
            { "2807600", "332100" }, // UMBAÚBA/SE
            { "3500105", "674000" }, // ADAMANTINA/SP
            { "3500204", "629200" }, // ADOLFO/SP
            { "3500303", "665100" }, // AGUAÍ/SP
            { "3500402", "666600" }, // ÁGUAS DA PRATA/SP
            { "3500501", "668600" }, // ÁGUAS DE LINDÓIA/SP
            { "3500550", "698800" }, // ÁGUAS DE SANTA BÁRBARA/SP
            { "3500600", "650700" }, // ÁGUAS DE SÃO PEDRO/SP
            { "3500709", "680200" }, // AGUDOS/SP
            { "3500758", "078500" }, // ALAMBARI/SP
            { "3500808", "688500" }, // ALFREDO MARCONDES/SP
            { "3500907", "626800" }, // ALTAIR/SP
            { "3501004", "664000" }, // ALTINÓPOLIS/SP
            { "3501103", "677700" }, // ALTO ALEGRE/SP
            { "3501152", "080800" }, // ALUMÍNIO/SP
            { "3501202", "625500" }, // ÁLVARES FLORENCE/SP
            { "3501301", "688600" }, // ÁLVARES MACHADO/SP
            { "3501400", "684900" }, // ÁLVARO DE CARVALHO/SP
            { "3501509", "685000" }, // ALVINLÂNDIA/SP
            { "3501608", "652400" }, // AMERICANA/SP
            { "3501707", "645400" }, // AMÉRICO BRASILIENSE/SP
            { "3501806", "625600" }, // AMÉRICO DE CAMPOS/SP
            { "3501905", "668700" }, // AMPARO/SP
            { "3502002", "650800" }, // ANALÂNDIA/SP
            { "3502101", "671500" }, // ANDRADINA/SP
            { "3502200", "701500" }, // ANGATUBA/SP
            { "3502309", "698900" }, // ANHEMBI/SP
            { "3502408", "688700" }, // ANHUMAS/SP
            { "3502507", "703800" }, // APARECIDA/SP
            { "3502606", "621300" }, // APARECIDA D'OESTE/SP
            { "3502705", "708200" }, // APIAÍ/SP
            { "3502754", "080900" }, // ARAÇARIGUAMA/SP
            { "3502804", "671600" }, // ARAÇATUBA/SP
            { "3502903", "658800" }, // ARAÇOIABA DA SERRA/SP
            { "3503000", "638500" }, // ARAMINA/SP
            { "3503109", "214700" }, // ARANDU/SP
            { "3503158", "078400" }, // ARAPEÍ/SP
            { "3503208", "645500" }, // ARARAQUARA/SP
            { "3503307", "652500" }, // ARARAS/SP
            { "3503356", "187600" }, // ARCO-ÍRIS/SP
            { "3503406", "680300" }, // AREALVA/SP
            { "3503505", "722000" }, // AREIAS/SP
            { "3503604", "699100" }, // AREIÓPOLIS/SP
            { "3503703", "634700" }, // ARIRANHA/SP
            { "3503802", "652600" }, // ARTUR NOGUEIRA/SP
            { "3503901", "711000" }, // ARUJÁ/SP
            { "3503950", "078000" }, // ASPÁSIA/SP
            { "3504008", "693600" }, // ASSIS/SP
            { "3504107", "670100" }, // ATIBAIA/SP
            { "3504206", "628000" }, // AURIFLAMA/SP
            { "3504305", "680400" }, // AVAÍ/SP
            { "3504404", "677800" }, // AVANHANDAVA/SP
            { "3504503", "699200" }, // AVARÉ/SP
            { "3504602", "631700" }, // BADY BASSITT/SP
            { "3504701", "680500" }, // BALBINOS/SP
            { "3504800", "631800" }, // BÁLSAMO/SP
            { "3504909", "722100" }, // BANANAL/SP
            { "3505005", "701600" }, // BARÃO DE ANTONINA/SP
            { "3505104", "677900" }, // BARBOSA/SP
            { "3505203", "648700" }, // BARIRI/SP
            { "3505302", "648800" }, // BARRA BONITA/SP
            { "3505351", "079400" }, // BARRA DO CHAPÉU/SP
            { "3505401", "708300" }, // BARRA DO TURVO/SP
            { "3505500", "637500" }, // BARRETOS/SP
            { "3505609", "643000" }, // BARRINHA/SP
            { "3505708", "711100" }, // BARUERI/SP
            { "3505807", "685100" }, // BASTOS/SP
            { "3505906", "664100" }, // BATATAIS/SP
            { "3506003", "680600" }, // BAURU/SP
            { "3506102", "640100" }, // BEBEDOURO/SP
            { "3506201", "671700" }, // BENTO DE ABREU/SP
            { "3506300", "696100" }, // BERNARDINO DE CAMPOS/SP
            { "3506359", "077000" }, // BERTIOGA/SP
            { "3506409", "678000" }, // BILAC/SP
            { "3506508", "678100" }, // BIRIGÜI/SP
            { "3506607", "711200" }, // BIRITIBA-MIRIM/SP
            { "3506706", "645600" }, // BOA ESPERANÇA DO SUL/SP
            { "3506805", "648900" }, // BOCAINA/SP
            { "3506904", "699300" }, // BOFETE/SP
            { "3507001", "657200" }, // BOITUVA/SP
            { "3507100", "670200" }, // BOM JESUS DOS PERDÕES/SP
            { "3507159", "140600" }, // BOM SUCESSO DE ITARARÉ/SP
            { "3507209", "693700" }, // BORÁ/SP
            { "3507308", "649000" }, // BORACÉIA/SP
            { "3507407", "645700" }, // BORBOREMA/SP
            { "3507456", "141600" }, // BOREBI/SP
            { "3507506", "699400" }, // BOTUCATU/SP
            { "3507605", "670300" }, // BRAGANÇA PAULISTA/SP
            { "3507704", "678200" }, // BRAÚNA/SP
            { "3507753", "187700" }, // BREJO ALEGRE/SP
            { "3507803", "664200" }, // BRODOWSKI/SP
            { "3507902", "650900" }, // BROTAS/SP
            { "3508009", "701700" }, // BURI/SP
            { "3508108", "678300" }, // BURITAMA/SP
            { "3508207", "638600" }, // BURITIZAL/SP
            { "3508306", "680700" }, // CABRÁLIA PAULISTA/SP
            { "3508405", "658900" }, // CABREÚVA/SP
            { "3508504", "703900" }, // CAÇAPAVA/SP
            { "3508603", "704000" }, // CACHOEIRA PAULISTA/SP
            { "3508702", "666700" }, // CACONDE/SP
            { "3508801", "680800" }, // CAFELÂNDIA/SP
            { "3508900", "688800" }, // CAIABU/SP
            { "3509007", "711300" }, // CAIEIRAS/SP
            { "3509106", "688900" }, // CAIUÁ/SP
            { "3509205", "711400" }, // CAJAMAR/SP
            { "3509254", "077100" }, // CAJATI/SP
            { "3509304", "634800" }, // CAJOBI/SP
            { "3509403", "664300" }, // CAJURU/SP
            { "3509452", "079500" }, // CAMPINA DO MONTE ALEGRE/SP
            { "3509502", "652700" }, // CAMPINAS/SP
            { "3509601", "661400" }, // CAMPO LIMPO PAULISTA/SP
            { "3509700", "704100" }, // CAMPOS DO JORDÃO/SP
            { "3509809", "693800" }, // CAMPOS NOVOS PAULISTA/SP
            { "3509908", "709300" }, // CANANÉIA/SP
            { "3509957", "187800" }, // CANAS/SP
            { "3510005", "693900" }, // CÂNDIDO MOTA/SP
            { "3510104", "640200" }, // CÂNDIDO RODRIGUES/SP
            { "3510153", "066900" }, // CANITAR/SP
            { "3510203", "706900" }, // CAPÃO BONITO/SP
            { "3510302", "659000" }, // CAPELA DO ALTO/SP
            { "3510401", "655300" }, // CAPIVARI/SP
            { "3510500", "723800" }, // CARAGUATATUBA/SP
            { "3510609", "711500" }, // CARAPICUÍBA/SP
            { "3510708", "625700" }, // CARDOSO/SP
            { "3510807", "665200" }, // CASA BRANCA/SP
            { "3510906", "664400" }, // CÁSSIA DOS COQUEIROS/SP
            { "3511003", "671800" }, // CASTILHO/SP
            { "3511102", "634900" }, // CATANDUVA/SP
            { "3511201", "635000" }, // CATIGUÁ/SP
            { "3511300", "631900" }, // CEDRAL/SP
            { "3511409", "699500" }, // CERQUEIRA CÉSAR/SP
            { "3511508", "657300" }, // CERQUILHO/SP
            { "3511607", "657400" }, // CESÁRIO LANGE/SP
            { "3511706", "655400" }, // CHARQUEADA/SP
            { "3557204", "697600" }, // CHAVANTES/SP
            { "3511904", "678400" }, // CLEMENTINA/SP
            { "3512001", "637600" }, // COLINA/SP
            { "3512100", "637700" }, // COLÔMBIA/SP
            { "3512209", "652800" }, // CONCHAL/SP
            { "3512308", "699600" }, // CONCHAS/SP
            { "3512407", "652900" }, // CORDEIRÓPOLIS/SP
            { "3512506", "678500" }, // COROADOS/SP
            { "3512605", "699700" }, // CORONEL MACEDO/SP
            { "3512704", "651000" }, // CORUMBATAÍ/SP
            { "3512803", "653000" }, // COSMÓPOLIS/SP
            { "3512902", "625800" }, // COSMORAMA/SP
            { "3513009", "711600" }, // COTIA/SP
            { "3513108", "643100" }, // CRAVINHOS/SP
            { "3513207", "662500" }, // CRISTAIS PAULISTA/SP
            { "3513306", "694000" }, // CRUZÁLIA/SP
            { "3513405", "704200" }, // CRUZEIRO/SP
            { "3513504", "725100" }, // CUBATÃO/SP
            { "3513603", "722200" }, // CUNHA/SP
            { "3513702", "645800" }, // DESCALVADO/SP
            { "3513801", "711700" }, // DIADEMA/SP
            { "3513850", "141700" }, // DIRCE REIS/SP
            { "3513900", "666800" }, // DIVINOLÂNDIA/SP
            { "3514007", "645900" }, // DOBRADA/SP
            { "3514106", "649100" }, // DOIS CÓRREGOS/SP
            { "3514205", "621400" }, // DOLCINÓPOLIS/SP
            { "3514304", "646000" }, // DOURADO/SP
            { "3514403", "674100" }, // DRACENA/SP
            { "3514502", "680900" }, // DUARTINA/SP
            { "3514601", "643200" }, // DUMONT/SP
            { "3514700", "694100" }, // ECHAPORÃ/SP
            { "3514809", "709400" }, // ELDORADO/SP
            { "3514908", "653100" }, // ELIAS FAUSTO/SP
            { "3514924", "067000" }, // ELISIÁRIO/SP
            { "3514957", "141800" }, // EMBAÚBA/SP
            { "3515004", "711800" }, // EMBU/SP
            { "3515103", "711900" }, // EMBU-GUAÇU/SP
            { "3515129", "076800" }, // EMILIANÓPOLIS/SP
            { "3515152", "074800" }, // ENGENHEIRO COELHO/SP
            { "3515186", "666900" }, // ESPÍRITO SANTO DO PINHAL/SP
            { "3515194", "143200" }, // ESPÍRITO SANTO DO TURVO/SP
            { "3557303", "140500" }, // ESTIVA GERBI/SP
            { "3515301", "689000" }, // ESTRELA DO NORTE/SP
            { "3515202", "621500" }, // ESTRELA D'OESTE/SP
            { "3515350", "053600" }, // EUCLIDES DA CUNHA PAULISTA/SP
            { "3515400", "696200" }, // FARTURA/SP
            { "3515608", "640300" }, // FERNANDO PRESTES/SP
            { "3515509", "621600" }, // FERNANDÓPOLIS/SP
            { "3515657", "187900" }, // FERNÃO/SP
            { "3515707", "712000" }, // FERRAZ DE VASCONCELOS/SP
            { "3515806", "674200" }, // FLORA RICA/SP
            { "3515905", "628100" }, // FLOREAL/SP
            { "3516002", "674300" }, // FLÓRIDA PAULISTA/SP
            { "3516101", "" }, // FLORÍNIA/SP
            { "3516200", "662600" }, // FRANCA/SP
            { "3516309", "712100" }, // FRANCISCO MORATO/SP
            { "3516408", "712200" }, // FRANCO DA ROCHA/SP
            { "3516507", "678600" }, // GABRIEL MONTEIRO/SP
            { "3516606", "685200" }, // GÁLIA/SP
            { "3516705", "685300" }, // GARÇA/SP
            { "3516804", "628200" }, // GASTÃO VIDIGAL/SP
            { "3516853", "188000" }, // GAVIÃO PEIXOTO/SP
            { "3516903", "628300" }, // GENERAL SALGADO/SP
            { "3517000", "681000" }, // GETULINA/SP
            { "3517109", "678700" }, // GLICÉRIO/SP
            { "3517208", "681100" }, // GUAIÇARA/SP
            { "3517307", "681200" }, // GUAIMBÊ/SP
            { "3517406", "637800" }, // GUAÍRA/SP
            { "3517505", "632000" }, // GUAPIAÇU/SP
            { "3517604", "707000" }, // GUAPIARA/SP
            { "3517703", "638700" }, // GUARÁ/SP
            { "3517802", "671900" }, // GUARAÇAÍ/SP
            { "3517901", "626900" }, // GUARACI/SP
            { "3518008", "621700" }, // GUARANI D'OESTE/SP
            { "3518107", "681300" }, // GUARANTÃ/SP
            { "3518206", "672000" }, // GUARARAPES/SP
            { "3518305", "712300" }, // GUARAREMA/SP
            { "3518404", "704300" }, // GUARATINGUETÁ/SP
            { "3518503", "701800" }, // GUAREÍ/SP
            { "3518602", "640400" }, // GUARIBA/SP
            { "3518701", "725200" }, // GUARUJÁ/SP
            { "3518800", "712400" }, // GUARULHOS/SP
            { "3518859", "143300" }, // GUATAPARÁ/SP
            { "3518909", "628400" }, // GUZOLÂNDIA/SP
            { "3519006", "685400" }, // HERCULÂNDIA/SP
            { "3519055", "143400" }, // HOLAMBRA/SP
            { "3519071", "074900" }, // HORTOLÂNDIA/SP
            { "3519105", "681400" }, // IACANGA/SP
            { "3519204", "685500" }, // IACRI/SP
            { "3519253", "103300" }, // IARAS/SP
            { "3519303", "646100" }, // IBATÉ/SP
            { "3519402", "632100" }, // IBIRÁ/SP
            { "3519501", "694300" }, // IBIRAREMA/SP
            { "3519600", "646200" }, // IBITINGA/SP
            { "3519709", "707100" }, // IBIÚNA/SP
            { "3519808", "627000" }, // ICÉM/SP
            { "3519907", "689100" }, // IEPÊ/SP
            { "3520004", "649200" }, // IGARAÇU DO TIETÊ/SP
            { "3520103", "638800" }, // IGARAPAVA/SP
            { "3520202", "704400" }, // IGARATÁ/SP
            { "3520301", "709500" }, // IGUAPE/SP
            { "3520426", "143500" }, // ILHA COMPRIDA/SP
            { "3520442", "074700" }, // ILHA SOLTEIRA/SP
            { "3520400", "723900" }, // ILHABELA/SP
            { "3520509", "653200" }, // INDAIATUBA/SP
            { "3520608", "689200" }, // INDIANA/SP
            { "3520707", "621800" }, // INDIAPORÃ/SP
            { "3520806", "674400" }, // INÚBIA PAULISTA/SP
            { "3520905", "" }, // IPAUSSU/SP
            { "3521002", "659100" }, // IPERÓ/SP
            { "3521101", "651100" }, // IPEÚNA/SP
            { "3521150", "188100" }, // IPIGUÁ/SP
            { "3521200", "708400" }, // IPORANGA/SP
            { "3521309", "638900" }, // IPUÃ/SP
            { "3521408", "655500" }, // IRACEMÁPOLIS/SP
            { "3521507", "635100" }, // IRAPUÃ/SP
            { "3521606", "674500" }, // IRAPURU/SP
            { "3521705", "701900" }, // ITABERÁ/SP
            { "3521804", "699800" }, // ITAÍ/SP
            { "3521903", "635200" }, // ITAJOBI/SP
            { "3522000", "649300" }, // ITAJU/SP
            { "3522109", "725300" }, // ITANHAÉM/SP
            { "3522158", "079600" }, // ITAÓCA/SP
            { "3522208", "712500" }, // ITAPECERICA DA SERRA/SP
            { "3522307", "702000" }, // ITAPETININGA/SP
            { "3522406", "702100" }, // ITAPEVA/SP
            { "3522505", "712600" }, // ITAPEVI/SP
            { "3522604", "668800" }, // ITAPIRA/SP
            { "3522653", "144900" }, // ITAPIRAPUÃ PAULISTA/SP
            { "3522703", "646300" }, // ITÁPOLIS/SP
            { "3522802", "702200" }, // ITAPORANGA/SP
            { "3522901", "649400" }, // ITAPUÍ/SP
            { "3523008", "672100" }, // ITAPURA/SP
            { "3523107", "712700" }, // ITAQUAQUECETUBA/SP
            { "3523206", "702300" }, // ITARARÉ/SP
            { "3523305", "725400" }, // ITARIRI/SP
            { "3523404", "661500" }, // ITATIBA/SP
            { "3523503", "699900" }, // ITATINGA/SP
            { "3523602", "651200" }, // ITIRAPINA/SP
            { "3523701", "662700" }, // ITIRAPUÃ/SP
            { "3523800", "667000" }, // ITOBI/SP
            { "3523909", "659200" }, // ITU/SP
            { "3524006", "661600" }, // ITUPEVA/SP
            { "3524105", "639000" }, // ITUVERAVA/SP
            { "3524204", "637900" }, // JABORANDI/SP
            { "3524303", "640500" }, // JABOTICABAL/SP
            { "3524402", "704500" }, // JACAREÍ/SP
            { "3524501", "632200" }, // JACI/SP
            { "3524600", "709600" }, // JACUPIRANGA/SP
            { "3524709", "653300" }, // JAGUARIÚNA/SP
            { "3524808", "621900" }, // JALES/SP
            { "3524907", "722300" }, // JAMBEIRO/SP
            { "3525003", "712800" }, // JANDIRA/SP
            { "3525102", "643300" }, // JARDINÓPOLIS/SP
            { "3525201", "661700" }, // JARINU/SP
            { "3525300", "649500" }, // JAÚ/SP
            { "3525409", "662800" }, // JERIQUARA/SP
            { "3525508", "670400" }, // JOANÓPOLIS/SP
            { "3525607", "689300" }, // JOÃO RAMALHO/SP
            { "3525706", "629300" }, // JOSÉ BONIFÁCIO/SP
            { "3525805", "681500" }, // JÚLIO MESQUITA/SP
            { "3525854", "188200" }, // JUMIRIM/SP
            { "3525904", "661800" }, // JUNDIAÍ/SP
            { "3526001", "674600" }, // JUNQUEIRÓPOLIS/SP
            { "3526100", "709700" }, // JUQUIÁ/SP
            { "3526209", "712900" }, // JUQUITIBA/SP
            { "3526308", "722400" }, // LAGOINHA/SP
            { "3526407", "657500" }, // LARANJAL PAULISTA/SP
            { "3526506", "672200" }, // LAVÍNIA/SP
            { "3526605", "704600" }, // LAVRINHAS/SP
            { "3526704", "665300" }, // LEME/SP
            { "3526803", "681600" }, // LENÇÓIS PAULISTA/SP
            { "3526902", "653400" }, // LIMEIRA/SP
            { "3527009", "668900" }, // LINDÓIA/SP
            { "3527108", "681700" }, // LINS/SP
            { "3527207", "704700" }, // LORENA/SP
            { "3527256", "074400" }, // LOURDES/SP
            { "3527306", "661900" }, // LOUVEIRA/SP
            { "3527405", "674700" }, // LUCÉLIA/SP
            { "3527504", "681800" }, // LUCIANÓPOLIS/SP
            { "3527603", "643400" }, // LUÍS ANTÔNIO/SP
            { "3527702", "678800" }, // LUIZIÂNIA/SP
            { "3527801", "685600" }, // LUPÉRCIO/SP
            { "3527900", "694400" }, // LUTÉCIA/SP
            { "3528007", "649600" }, // MACATUBA/SP
            { "3528106", "629400" }, // MACAUBAL/SP
            { "3528205", "622000" }, // MACEDÔNIA/SP
            { "3528304", "628500" }, // MÁGDA/SP
            { "3528403", "659300" }, // MAIRINQUE/SP
            { "3528502", "713000" }, // MAIRIPORÃ/SP
            { "3528601", "696400" }, // MANDURI/SP
            { "3528700", "689400" }, // MARABÁ PAULISTA/SP
            { "3528809", "694500" }, // MARACAÍ/SP
            { "3528858", "077400" }, // MARAPOAMA/SP
            { "3528908", "674800" }, // MARIÁPOLIS/SP
            { "3529005", "685700" }, // MARÍLIA/SP
            { "3529104", "622100" }, // MARINÓPOLIS/SP
            { "3529203", "689500" }, // MARTINÓPOLIS/SP
            { "3529302", "646400" }, // MATÃO/SP
            { "3529401", "713100" }, // MAUÁ/SP
            { "3529500", "629500" }, // MENDONÇA/SP
            { "3529609", "622200" }, // MERIDIANO/SP
            { "3529658", "078100" }, // MESÓPOLIS/SP
            { "3529708", "639100" }, // MIGUELÓPOLIS/SP
            { "3529807", "649700" }, // MINEIROS DO TIETÊ/SP
            { "3530003", "622300" }, // MIRA ESTRELA/SP
            { "3529906", "709800" }, // MIRACATU/SP
            { "3530102", "672300" }, // MIRANDÓPOLIS/SP
            { "3530201", "689600" }, // MIRANTE DO PARANAPANEMA/SP
            { "3530300", "632300" }, // MIRASSOL/SP
            { "3530409", "632400" }, // MIRASSOLÂNDIA/SP
            { "3530508", "667100" }, // MOCOCA/SP
            { "3530607", "713200" }, // MOGI DAS CRUZES/SP
            { "3530706", "665400" }, // MOGI GUAÇU/SP
            { "3530805", "665500" }, // MOGI MIRIM/SP
            { "3530904", "655600" }, // MOMBUCA/SP
            { "3531001", "629600" }, // MONÇÕES/SP
            { "3531100", "725500" }, // MONGAGUÁ/SP
            { "3531209", "669000" }, // MONTE ALEGRE DO SUL/SP
            { "3531308", "640600" }, // MONTE ALTO/SP
            { "3531407", "629700" }, // MONTE APRAZÍVEL/SP
            { "3531506", "640700" }, // MONTE AZUL PAULISTA/SP
            { "3531605", "674900" }, // MONTE CASTELO/SP
            { "3531803", "653500" }, // MONTE MOR/SP
            { "3531704", "704800" }, // MONTEIRO LOBATO/SP
            { "3531902", "639200" }, // MORRO AGUDO/SP
            { "3532009", "662000" }, // MORUNGABA/SP
            { "3532058", "145000" }, // MOTUCA/SP
            { "3532108", "672400" }, // MURUTINGA DO SUL/SP
            { "3532157", "188300" }, // NANTES/SP
            { "3532207", "689700" }, // NARANDIBA/SP
            { "3532306", "722500" }, // NATIVIDADE DA SERRA/SP
            { "3532405", "670500" }, // NAZARÉ PAULISTA/SP
            { "3532504", "629800" }, // NEVES PAULISTA/SP
            { "3532603", "629900" }, // NHANDEARA/SP
            { "3532702", "630000" }, // NIPOÃ/SP
            { "3532801", "632500" }, // NOVA ALIANÇA/SP
            { "3532827", "145100" }, // NOVA CAMPINA/SP
            { "3532843", "146200" }, // NOVA CANAÃ PAULISTA/SP
            { "3532868", "188400" }, // NOVA CASTILHO/SP
            { "3532900", "646500" }, // NOVA EUROPA/SP
            { "3533007", "632600" }, // NOVA GRANADA/SP
            { "3533106", "675000" }, // NOVA GUATAPORANGA/SP
            { "3533205", "672500" }, // NOVA INDEPENDÊNCIA/SP
            { "3533304", "628600" }, // NOVA LUZITÂNIA/SP
            { "3533403", "653600" }, // NOVA ODESSA/SP
            { "3533254", "077500" }, // NOVAIS/SP
            { "3533502", "635300" }, // NOVO HORIZONTE/SP
            { "3533601", "664500" }, // NUPORANGA/SP
            { "3533700", "685800" }, // OCAUÇU/SP
            { "3533809", "696500" }, // ÓLEO/SP
            { "3533908", "627100" }, // OLÍMPIA/SP
            { "3534005", "632700" }, // ONDA VERDE/SP
            { "3534104", "685900" }, // ORIENTE/SP
            { "3534203", "627200" }, // ORINDIÚVA/SP
            { "3534302", "639300" }, // ORLÂNDIA/SP
            { "3534401", "713300" }, // OSASCO/SP
            { "3534500", "694600" }, // OSCAR BRESSANE/SP
            { "3534609", "675100" }, // OSVALDO CRUZ/SP
            { "3534708", "696600" }, // OURINHOS/SP
            { "3534807", "675200" }, // OURO VERDE/SP
            { "3534757", "208800" }, // OUROESTE/SP
            { "3534906", "675300" }, // PACAEMBU/SP
            { "3535002", "632800" }, // PALESTINA/SP
            { "3535101", "635400" }, // PALMARES PAULISTA/SP
            { "3535200", "622400" }, // PALMEIRA D'OESTE/SP
            { "3535309", "694700" }, // PALMITAL/SP
            { "3535408", "675400" }, // PANORAMA/SP
            { "3535507", "694800" }, // PARAGUAÇU PAULISTA/SP
            { "3535606", "722600" }, // PARAIBUNA/SP
            { "3535705", "635500" }, // PARAÍSO/SP
            { "3535804", "700000" }, // PARANAPANEMA/SP
            { "3535903", "622500" }, // PARANAPUÃ/SP
            { "3536000", "675500" }, // PARAPUÃ/SP
            { "3536109", "700100" }, // PARDINHO/SP
            { "3536208", "709900" }, // PARIQUERA-AÇU/SP
            { "3536257", "078300" }, // PARISI/SP
            { "3536307", "662900" }, // PATROCÍNIO PAULISTA/SP
            { "3536406", "675600" }, // PAULICÉIA/SP
            { "3536505", "653700" }, // PAULÍNIA/SP
            { "3536570", "188500" }, // PAULISTÂNIA/SP
            { "3536604", "627300" }, // PAULO DE FARIA/SP
            { "3536703", "649800" }, // PEDERNEIRAS/SP
            { "3536802", "670600" }, // PEDRA BELA/SP
            { "3536901", "622600" }, // PEDRANÓPOLIS/SP
            { "3537008", "663000" }, // PEDREGULHO/SP
            { "3537107", "669100" }, // PEDREIRA/SP
            { "3537156", "" }, // PEDRINHAS PAULISTA/SP
            { "3537206", "725600" }, // PEDRO DE TOLEDO/SP
            { "3537305", "678900" }, // PENÁPOLIS/SP
            { "3537404", "672600" }, // PEREIRA BARRETO/SP
            { "3537503", "657600" }, // PEREIRAS/SP
            { "3537602", "725700" }, // PERUÍBE/SP
            { "3537701", "679000" }, // PIACATU/SP
            { "3537800", "707200" }, // PIEDADE/SP
            { "3537909", "707300" }, // PILAR DO SUL/SP
            { "3538006", "704900" }, // PINDAMONHANGABA/SP
            { "3538105", "635600" }, // PINDORAMA/SP
            { "3538204", "670700" }, // PINHALZINHO/SP
            { "3538303", "689800" }, // PIQUEROBI/SP
            { "3538501", "705000" }, // PIQUETE/SP
            { "3538600", "670800" }, // PIRACAIA/SP
            { "3538709", "655700" }, // PIRACICABA/SP
            { "3538808", "696700" }, // PIRAJU/SP
            { "3538907", "681900" }, // PIRAJUÍ/SP
            { "3539004", "640800" }, // PIRANGI/SP
            { "3539103", "713400" }, // PIRAPORA DO BOM JESUS/SP
            { "3539202", "689900" }, // PIRAPOZINHO/SP
            { "3539301", "665600" }, // PIRASSUNUNGA/SP
            { "3539400", "682000" }, // PIRATININGA/SP
            { "3539509", "640900" }, // PITANGUEIRAS/SP
            { "3539608", "630100" }, // PLANALTO/SP
            { "3539707", "694900" }, // PLATINA/SP
            { "3539806", "713500" }, // POÁ/SP
            { "3539905", "630200" }, // POLONI/SP
            { "3540002", "686000" }, // POMPÉIA/SP
            { "3540101", "682100" }, // PONGAÍ/SP
            { "3540200", "643500" }, // PONTAL/SP
            { "3540259", "078200" }, // PONTALINDA/SP
            { "3540309", "625900" }, // PONTES GESTAL/SP
            { "3540408", "622700" }, // POPULINA/SP
            { "3540507", "657700" }, // PORANGABA/SP
            { "3540606", "659400" }, // PORTO FELIZ/SP
            { "3540705", "665700" }, // PORTO FERREIRA/SP
            { "3540754", "146300" }, // POTIM/SP
            { "3540804", "632900" }, // POTIRENDABA/SP
            { "3540853", "188600" }, // PRACINHA/SP
            { "3540903", "643600" }, // PRADÓPOLIS/SP
            { "3541000", "725800" }, // PRAIA GRANDE/SP
            { "3541059", "188700" }, // PRATÂNIA/SP
            { "3541109", "682200" }, // PRESIDENTE ALVES/SP
            { "3541208", "690000" }, // PRESIDENTE BERNARDES/SP
            { "3541307", "690100" }, // PRESIDENTE EPITÁCIO/SP
            { "3541406", "690200" }, // PRESIDENTE PRUDENTE/SP
            { "3541505", "690300" }, // PRESIDENTE VENCESLAU/SP
            { "3541604", "682300" }, // PROMISSÃO/SP
            { "3541653", "188800" }, // QUADRA/SP
            { "3541703", "695000" }, // QUATÁ/SP
            { "3541802", "686100" }, // QUEIROZ/SP
            { "3541901", "705100" }, // QUELUZ/SP
            { "3542008", "686200" }, // QUINTANA/SP
            { "3542107", "655800" }, // RAFARD/SP
            { "3542206", "690400" }, // RANCHARIA/SP
            { "3542305", "722700" }, // REDENÇÃO DA SERRA/SP
            { "3542404", "690500" }, // REGENTE FEIJÓ/SP
            { "3542503", "682400" }, // REGINÓPOLIS/SP
            { "3542602", "710000" }, // REGISTRO/SP
            { "3542701", "663100" }, // RESTINGA/SP
            { "3542800", "708500" }, // RIBEIRA/SP
            { "3542909", "646600" }, // RIBEIRÃO BONITO/SP
            { "3543006", "707400" }, // RIBEIRÃO BRANCO/SP
            { "3543105", "663200" }, // RIBEIRÃO CORRENTE/SP
            { "3543204", "696800" }, // RIBEIRÃO DO SUL/SP
            { "3543238", "188900" }, // RIBEIRÃO DOS ÍNDIOS/SP
            { "3543253", "079700" }, // RIBEIRÃO GRANDE/SP
            { "3543303", "713600" }, // RIBEIRÃO PIRES/SP
            { "3543402", "643700" }, // RIBEIRÃO PRETO/SP
            { "3543600", "663300" }, // RIFAINA/SP
            { "3543709", "646700" }, // RINCÃO/SP
            { "3543808", "675700" }, // RINÓPOLIS/SP
            { "3543907", "651300" }, // RIO CLARO/SP
            { "3544004", "655900" }, // RIO DAS PEDRAS/SP
            { "3544103", "713700" }, // RIO GRANDE DA SERRA/SP
            { "3544202", "627400" }, // RIOLÂNDIA/SP
            { "3543501", "702400" }, // RIVERSUL/SP
            { "3544251", "146400" }, // ROSANA/SP
            { "3544301", "705200" }, // ROSEIRA/SP
            { "3544400", "672700" }, // RUBIÁCEA/SP
            { "3544509", "622800" }, // RUBINÉIA/SP
            { "3544608", "682500" }, // SABINO/SP
            { "3544707", "675800" }, // SAGRES/SP
            { "3544806", "635700" }, // SALES/SP
            { "3544905", "643800" }, // SALES OLIVEIRA/SP
            { "3545001", "713800" }, // SALESÓPOLIS/SP
            { "3545100", "" }, // SALMOURÃO/SP
            { "3545159", "081700" }, // SALTINHO/SP
            { "3545209", "659500" }, // SALTO/SP
            { "3545308", "659600" }, // SALTO DE PIRAPORA/SP
            { "3545407", "696900" }, // SALTO GRANDE/SP
            { "3545506", "690600" }, // SANDOVALINA/SP
            { "3545605", "635800" }, // SANTA ADÉLIA/SP
            { "3545704", "622900" }, // SANTA ALBERTINA/SP
            { "3545803", "656000" }, // SANTA BÁRBARA D'OESTE/SP
            { "3546009", "705300" }, // SANTA BRANCA/SP
            { "3546108", "623000" }, // SANTA CLARA D'OESTE/SP
            { "3546207", "665800" }, // SANTA CRUZ DA CONCEIÇÃO/SP
            { "3546256", "189000" }, // SANTA CRUZ DA ESPERANÇA/SP
            { "3546306", "665900" }, // SANTA CRUZ DAS PALMEIRAS/SP
            { "3546405", "697000" }, // SANTA CRUZ DO RIO PARDO/SP
            { "3546504", "641000" }, // SANTA ERNESTINA/SP
            { "3546603", "623100" }, // SANTA FÉ DO SUL/SP
            { "3546702", "656100" }, // SANTA GERTRUDES/SP
            { "3546801", "713900" }, // SANTA ISABEL/SP
            { "3546900", "646800" }, // SANTA LÚCIA/SP
            { "3547007", "651400" }, // SANTA MARIA DA SERRA/SP
            { "3547106", "676000" }, // SANTA MERCEDES/SP
            { "3547502", "643900" }, // SANTA RITA DO PASSA QUATRO/SP
            { "3547403", "623300" }, // SANTA RITA D'OESTE/SP
            { "3547601", "644000" }, // SANTA ROSA DE VITERBO/SP
            { "3547650", "189100" }, // SANTA SALETE/SP
            { "3547205", "623200" }, // SANTANA DA PONTE PENSA/SP
            { "3547304", "714000" }, // SANTANA DE PARNAÍBA/SP
            { "3547700", "690700" }, // SANTO ANASTÁCIO/SP
            { "3547809", "714100" }, // SANTO ANDRÉ/SP
            { "3547908", "664600" }, // SANTO ANTÔNIO DA ALEGRIA/SP
            { "3548005", "653800" }, // SANTO ANTÔNIO DE POSSE/SP
            { "3548054", "074500" }, // SANTO ANTÔNIO DO ARACANGUÁ/SP
            { "3548104", "667200" }, // SANTO ANTÔNIO DO JARDIM/SP
            { "3548203", "705400" }, // SANTO ANTÔNIO DO PINHAL/SP
            { "3548302", "690800" }, // SANTO EXPEDITO/SP
            { "3548401", "679100" }, // SANTÓPOLIS DO AGUAPEÍ/SP
            { "3548500", "725900" }, // SANTOS/SP
            { "3548609", "705500" }, // SÃO BENTO DO SAPUCAÍ/SP
            { "3548708", "714200" }, // SÃO BERNARDO DO CAMPO/SP
            { "3548807", "714300" }, // SÃO CAETANO DO SUL/SP
            { "3548906", "646900" }, // SÃO CARLOS/SP
            { "3549003", "623400" }, // SÃO FRANCISCO/SP
            { "3549102", "667300" }, // SÃO JOÃO DA BOA VISTA/SP
            { "3549201", "623500" }, // SÃO JOÃO DAS DUAS PONTES/SP
            { "3549250", "074600" }, // SÃO JOÃO DE IRACEMA/SP
            { "3549300", "676100" }, // SÃO JOÃO DO PAU D'ALHO/SP
            { "3549409", "639400" }, // SÃO JOAQUIM DA BARRA/SP
            { "3549508", "663400" }, // SÃO JOSÉ DA BELA VISTA/SP
            { "3549607", "722800" }, // SÃO JOSÉ DO BARREIRO/SP
            { "3549706", "667400" }, // SÃO JOSÉ DO RIO PARDO/SP
            { "3549805", "633000" }, // SÃO JOSÉ DO RIO PRETO/SP
            { "3549904", "705600" }, // SÃO JOSÉ DOS CAMPOS/SP
            { "3549953", "081800" }, // SÃO LOURENÇO DA SERRA/SP
            { "3550001", "" }, // SÃO LUIZ DO PARAITINGA/SP
            { "3550100", "700200" }, // SÃO MANUEL/SP
            { "3550209", "707500" }, // SÃO MIGUEL ARCANJO/SP
            { "3550308", "714400" }, // SÃO PAULO/SP
            { "3550407", "651500" }, // SÃO PEDRO/SP
            { "3550506", "697100" }, // SÃO PEDRO DO TURVO/SP
            { "3550605", "659700" }, // SÃO ROQUE/SP
            { "3550704", "724000" }, // SÃO SEBASTIÃO/SP
            { "3550803", "667500" }, // SÃO SEBASTIÃO DA GRAMA/SP
            { "3550902", "644100" }, // SÃO SIMÃO/SP
            { "3551009", "726000" }, // SÃO VICENTE/SP
            { "3551108", "659800" }, // SARAPUÍ/SP
            { "3551207", "697200" }, // SARUTAIÁ/SP
            { "3551306", "630300" }, // SEBASTIANÓPOLIS DO SUL/SP
            { "3551405", "644200" }, // SERRA AZUL/SP
            { "3551603", "669200" }, // SERRA NEGRA/SP
            { "3551504", "644300" }, // SERRANA/SP
            { "3551702", "644400" }, // SERTÃOZINHO/SP
            { "3551801", "710100" }, // SETE BARRAS/SP
            { "3551900", "635900" }, // SEVERÍNIA/SP
            { "3552007", "723000" }, // SILVEIRAS/SP
            { "3552106", "669300" }, // SOCORRO/SP
            { "3552205", "659900" }, // SOROCABA/SP
            { "3552304", "672800" }, // SUD MENNUCCI/SP
            { "3552403", "653900" }, // SUMARÉ/SP
            { "3552551", "066800" }, // SUZANÁPOLIS/SP
            { "3552502", "714500" }, // SUZANO/SP
            { "3552601", "636000" }, // TABAPUÃ/SP
            { "3552700", "647000" }, // TABATINGA/SP
            { "3552809", "714600" }, // TABOÃO DA SERRA/SP
            { "3552908", "690900" }, // TACIBA/SP
            { "3553005", "697300" }, // TAGUAÍ/SP
            { "3553104", "641100" }, // TAIAÇU/SP
            { "3553203", "641200" }, // TAIÚVA/SP
            { "3553302", "666000" }, // TAMBAÚ/SP
            { "3553401", "633100" }, // TANABI/SP
            { "3553500", "707600" }, // TAPIRAÍ/SP
            { "3553609", "667600" }, // TAPIRATIBA/SP
            { "3553658", "189200" }, // TAQUARAL/SP
            { "3553708", "641300" }, // TAQUARITINGA/SP
            { "3553807", "700300" }, // TAQUARITUBA/SP
            { "3553856", "080700" }, // TAQUARIVAÍ/SP
            { "3553906", "691000" }, // TARABAI/SP
            { "3553955", "140400" }, // TARUMÃ/SP
            { "3554003", "657800" }, // TATUÍ/SP
            { "3554102", "705700" }, // TAUBATÉ/SP
            { "3554201", "697400" }, // TEJUPÁ/SP
            { "3554300", "691100" }, // TEODORO SAMPAIO/SP
            { "3554409", "641400" }, // TERRA ROXA/SP
            { "3554508", "657900" }, // TIETÊ/SP
            { "3554607", "697500" }, // TIMBURI/SP
            { "3554656", "081000" }, // TORRE DE PEDRA/SP
            { "3554706", "651600" }, // TORRINHA/SP
            { "3554755", "189300" }, // TRABIJU/SP
            { "3554805", "705800" }, // TREMEMBÉ/SP
            { "3554904", "623600" }, // TRÊS FRONTEIRAS/SP
            { "3554953", "075000" }, // TUIUTI/SP
            { "3555000", "686300" }, // TUPÃ/SP
            { "3555109", "676200" }, // TUPI PAULISTA/SP
            { "3555208", "679200" }, // TURIÚBA/SP
            { "3555307", "623700" }, // TURMALINA/SP
            { "3555356", "077200" }, // UBARANA/SP
            { "3555406", "724100" }, // UBATUBA/SP
            { "3555505", "686400" }, // UBIRAJARA/SP
            { "3555604", "633200" }, // UCHOA/SP
            { "3555703", "630400" }, // UNIÃO PAULISTA/SP
            { "3555802", "623800" }, // URÂNIA/SP
            { "3555901", "682600" }, // URU/SP
            { "3556008", "636100" }, // URUPÊS/SP
            { "3556107", "626000" }, // VALENTIM GENTIL/SP
            { "3556206", "654000" }, // VALINHOS/SP
            { "3556305", "672900" }, // VALPARAÍSO/SP
            { "3556354", "076700" }, // VARGEM/SP
            { "3556404", "667700" }, // VARGEM GRANDE DO SUL/SP
            { "3556453", "714700" }, // VARGEM GRANDE PAULISTA/SP
            { "3556503", "662100" }, // VÁRZEA PAULISTA/SP
            { "3556602", "686500" }, // VERA CRUZ/SP
            { "3556701", "654100" }, // VINHEDO/SP
            { "3556800", "641500" }, // VIRADOURO/SP
            { "3556909", "641600" }, // VISTA ALEGRE DO ALTO/SP
            { "3556958", "189400" }, // VITÓRIA BRASIL/SP
            { "3557006", "660000" }, // VOTORANTIM/SP
            { "3557105", "626100" }, // VOTUPORANGA/SP
            { "3557154", "077300" }, // ZACARIAS/SP
            { "1700251", "055000" }, // ABREULÂNDIA/TO
            { "1700301", "208900" }, // AGUIARNÓPOLIS/TO
            { "1700350", "007900" }, // ALIANÇA DO TOCANTINS/TO
            { "1700400", "009100" }, // ALMAS/TO
            { "1700707", "005900" }, // ALVORADA/TO
            { "1701002", "000100" }, // ANANÁS/TO
            { "1701051", "058100" }, // ANGICO/TO
            { "1701101", "004900" }, // APARECIDA DO RIO NEGRO/TO
            { "1701309", "055100" }, // ARAGOMINAS/TO
            { "1701903", "002700" }, // ARAGUACEMA/TO
            { "1702000", "011500" }, // ARAGUAÇU/TO
            { "1702109", "000200" }, // ARAGUAÍNA/TO
            { "1702158", "056000" }, // ARAGUANÃ/TO
            { "1702208", "000300" }, // ARAGUATINS/TO
            { "1702307", "002800" }, // ARAPOEMA/TO
            { "1702406", "009200" }, // ARRAIAS/TO
            { "1702554", "000400" }, // AUGUSTINÓPOLIS/TO
            { "1702703", "009300" }, // AURORA DO TOCANTINS/TO
            { "1702901", "000500" }, // AXIXÁ DO TOCANTINS/TO
            { "1703008", "000600" }, // BABAÇULÂNDIA/TO
            { "1703057", "210000" }, // BANDEIRANTES DO TOCANTINS/TO
            { "1703073", "209000" }, // BARRA DO OURO/TO
            { "1703107", "008000" }, // BARROLÂNDIA/TO
            { "1703206", "003400" }, // BERNARDO SAYÃO/TO
            { "1703305", "056100" }, // BOM JESUS DO TOCANTINS/TO
            { "1703602", "058200" }, // BRASILÂNDIA DO TOCANTINS/TO
            { "1703701", "006000" }, // BREJINHO DE NAZARÉ/TO
            { "1703800", "001800" }, // BURITI DO TOCANTINS/TO
            { "1703826", "056200" }, // CACHOEIRINHA/TO
            { "1703842", "058300" }, // CAMPOS LINDOS/TO
            { "1703867", "056300" }, // CARIRI DO TOCANTINS/TO
            { "1703883", "058400" }, // CARMOLÂNDIA/TO
            { "1703891", "058500" }, // CARRASCO BONITO/TO
            { "1703909", "003600" }, // CASEARA/TO
            { "1704105", "058600" }, // CENTENÁRIO/TO
            { "1705102", "209100" }, // CHAPADA DA NATIVIDADE/TO
            { "1704600", "209200" }, // CHAPADA DE AREIA/TO
            { "1705508", "002900" }, // COLINAS DO TOCANTINS/TO
            { "1716703", "003300" }, // COLMÉIA/TO
            { "1705557", "010500" }, // COMBINADO/TO
            { "1705607", "009500" }, // CONCEIÇÃO DO TOCANTINS/TO
            { "1706001", "003000" }, // COUTO DE MAGALHÃES/TO
            { "1706100", "006100" }, // CRISTALÂNDIA/TO
            { "1706258", "209300" }, // CRIXÁS DO TOCANTINS/TO
            { "1706506", "061800" }, // DARCINÓPOLIS/TO
            { "1707009", "009600" }, // DIANÓPOLIS/TO
            { "1707108", "007700" }, // DIVINÓPOLIS DO TOCANTINS/TO
            { "1707207", "003100" }, // DOIS IRMÃOS DO TOCANTINS/TO
            { "1707306", "006200" }, // DUERÊ/TO
            { "1707405", "061900" }, // ESPERANTINA/TO
            { "1707553", "006300" }, // FÁTIMA/TO
            { "1707652", "006400" }, // FIGUEIRÓPOLIS/TO
            { "1707702", "000700" }, // FILADÉLFIA/TO
            { "1708205", "006500" }, // FORMOSO DO ARAGUAIA/TO
            { "1708254", "063200" }, // FORTALEZA DO TABOCÃO/TO
            { "1708304", "003800" }, // GOIANORTE/TO
            { "1709005", "004100" }, // GOIATINS/TO
            { "1709302", "006600" }, // GUARAÍ/TO
            { "1709500", "006700" }, // GURUPI/TO
            { "1709807", "190700" }, // IPUEIRAS/TO
            { "1710508", "004200" }, // ITACAJÁ/TO
            { "1710706", "000800" }, // ITAGUATINS/TO
            { "1710904", "063300" }, // ITAPIRATINS/TO
            { "1711100", "003200" }, // ITAPORÃ DO TOCANTINS/TO
            { "1711506", "063400" }, // JAÚ DO TOCANTINS/TO
            { "1711803", "063500" }, // JUARINA/TO
            { "1711902", "063700" }, // LAGOA DA CONFUSÃO/TO
            { "1711951", "063800" }, // LAGOA DO TOCANTINS/TO
            { "1712009", "063600" }, // LAJEADO/TO
            { "1712157", "209400" }, // LAVANDEIRA/TO
            { "1712405", "004300" }, // LIZARDA/TO
            { "1712454", "209500" }, // LUZINÓPOLIS/TO
            { "1712504", "003700" }, // MARIANÓPOLIS DO TOCANTINS/TO
            { "1712702", "056400" }, // MATEIROS/TO
            { "1712801", "063900" }, // MAURILÂNDIA DO TOCANTINS/TO
            { "1713205", "006800" }, // MIRACEMA DO TOCANTINS/TO
            { "1713304", "006900" }, // MIRANORTE/TO
            { "1713601", "007000" }, // MONTE DO CARMO/TO
            { "1713700", "209600" }, // MONTE SANTO DO TOCANTINS/TO
            { "1713957", "056500" }, // MURICILÂNDIA/TO
            { "1714203", "009800" }, // NATIVIDADE/TO
            { "1714302", "000900" }, // NAZARÉ/TO
            { "1714880", "001000" }, // NOVA OLINDA/TO
            { "1715002", "008100" }, // NOVA ROSALÂNDIA/TO
            { "1715101", "004400" }, // NOVO ACORDO/TO
            { "1715150", "056600" }, // NOVO ALEGRE/TO
            { "1715259", "064100" }, // NOVO JARDIM/TO
            { "1715507", "209700" }, // OLIVEIRA DE FÁTIMA/TO
            { "1721000", "008200" }, // PALMAS/TO
            { "1715705", "056700" }, // PALMEIRANTE/TO
            { "1715754", "009900" }, // PALMEIRÓPOLIS/TO
            { "1716109", "007100" }, // PARAÍSO DO TOCANTINS/TO
            { "1716208", "010000" }, // PARANÃ/TO
            { "1716307", "056800" }, // PAU D'ARCO/TO
            { "1716505", "004500" }, // PEDRO AFONSO/TO
            { "1716604", "000212" }, // PEIXE/TO
            { "1716653", "003500" }, // PEQUIZEIRO/TO
            { "1717008", "010100" }, // PINDORAMA DO TOCANTINS/TO
            { "1717206", "056900" }, // PIRAQUÊ/TO
            { "1717503", "007300" }, // PIUM/TO
            { "1717800", "010200" }, // PONTE ALTA DO BOM JESUS/TO
            { "1717909", "004600" }, // PONTE ALTA DO TOCANTINS/TO
            { "1718006", "010800" }, // PORTO ALEGRE DO TOCANTINS/TO
            { "1718204", "007400" }, // PORTO NACIONAL/TO
            { "1718303", "001700" }, // PRAIA NORTE/TO
            { "1718402", "007500" }, // PRESIDENTE KENNEDY/TO
            { "1718451", "209800" }, // PUGMIL/TO
            { "1718501", "064900" }, // RECURSOLÂNDIA/TO
            { "1718550", "065000" }, // RIACHINHO/TO
            { "1718659", "057000" }, // RIO DA CONCEIÇÃO/TO
            { "1718709", "065800" }, // RIO DOS BOIS/TO
            { "1718758", "004700" }, // RIO SONO/TO
            { "1718808", "001600" }, // SAMPAIO/TO
            { "1718840", "065300" }, // SANDOLÂNDIA/TO
            { "1718865", "065500" }, // SANTA FÉ DO ARAGUAIA/TO
            { "1718881", "065400" }, // SANTA MARIA DO TOCANTINS/TO
            { "1718899", "214800" }, // SANTA RITA DO TOCANTINS/TO
            { "1718907", "010600" }, // SANTA ROSA DO TOCANTINS/TO
            { "1719004", "005000" }, // SANTA TEREZA DO TOCANTINS/TO
            { "1720002", "209900" }, // SANTA TEREZINHA DO TOCANTINS/TO
            { "1720101", "057100" }, // SÃO BENTO DO TOCANTINS/TO
            { "1720150", "065100" }, // SÃO FÉLIX DO TOCANTINS/TO
            { "1720200", "065200" }, // SÃO MIGUEL DO TOCANTINS/TO
            { "1720259", "057200" }, // SÃO SALVADOR DO TOCANTINS/TO
            { "1720309", "001100" }, // SÃO SEBASTIÃO DO TOCANTINS/TO
            { "1720499", "010700" }, // SÃO VALÉRIO DA NATIVIDADE/TO
            { "1720655", "007600" }, // SILVANÓPOLIS/TO
            { "1720804", "001200" }, // SÍTIO NOVO DO TOCANTINS/TO
            { "1720853", "065600" }, // SUCUPIRA/TO
            { "1720903", "010300" }, // TAGUATINGA/TO
            { "1720937", "057300" }, // TAIPAS DO TOCANTINS/TO
            { "1720978", "175100" }, // TALISMÃ/TO
            { "1721109", "004800" }, // TOCANTÍNIA/TO
            { "1721208", "001300" }, // TOCANTINÓPOLIS/TO
            { "1721257", "190800" }, // TUPIRAMA/TO
            { "1721307", "065700" }, // TUPIRATINS/TO
            { "1722081", "001400" }, // WANDERLÂNDIA/TO
            { "1722107", "001500" }, // XAMBIOÁ/TO
        };

    #endregion Cidades Goiania X Código IBGE
}