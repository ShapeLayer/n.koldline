namespace GamePlay.CallingInteraction
{
  public static class CallingInteractionDefinitions
  {
    // --- 기본 안내 및 국가번호 관련 ---
    public static ResponseNode CALLING_START = MakeNode("CALLING_START", "안녕하세요, 백악관 핫 라인에 전화하셨습니다.");
    public static ResponseNode NATIONALITY_REQ = MakeNode("NATIONALITY_REQ", "전화주신 분의 국적을 국가번호로 입력하고 우물 정자를 눌러주세요. 국가번호에 대해 잘 모르고 계신다면 별표를 눌러주세요.");
    public static ResponseNode NATN_CODE_ENTRY = MakeNode("NATN_CODE_ENTRY", "국가번호는 모든 전화에 대해, 국제적으로 각 전화 가입자의 국가를 식별하기 위해 국제전기연합에서 제정한 표준입니다. " +
                                                                               "귀하께서 북아메리카 또는 카리브에 계신다면 1번, 아프리카 또는 대서양에 계신다면 2번, 유럽에 계신다면 3번, 중앙아메리카 또는 남아메리카에 계신다면 5번, 동남아시아 또는 오세아니아에 계신다면 6번, 러시아에 계신다면 7번, 동아시아 또는 인도차이나에 계신다면 8번, 남아시아, 서아시아, 중앙아시아에 계신다면 9번을 눌러주세요.");

    public static ResponseNode NATN_CODE_P1 = MakeNode("NATN_CODE_P1", "국가번호 1번을 선택하셨습니다. 미국 본토 주 지역은 +1 201부터 989까지, 미국령인 버진 아일랜드는 +1 340, 북마리아나 제도는 +1 670, 괌은 +1 671, 미국령 사모아는 +1 684, 푸에르토리코는 +1 787 또는 939, 캐나다는 +1 204 및 226부터 942까지, 영국령 버뮤다는 +1 441, 앵귈라는 +1 264, 영국령 버진 아일랜드는 +1 284, 케이먼 제도는 +1 345, 터크스 케이커스 제도는 +1 649, 몬트세랫은 +1 664, 바하마는 +1 242, 바베이도스는 +1 246, 앤티가 바부다는 +1 268, 그레나다는 +1 473, 네덜란드령 신트마르턴은 +1 721, 세인트루시아는 +1 758, 도미니카 연방은 +1 767, 세인트빈센트 그레나딘은 +1 784, 트리니다드 토바고는 +1 868, 세인트키츠 네비스는 +1 869, 자메이카는 +1 876, 도미니카 공화국은 +1 809, 829 또는 849입니다. 안내를 다시 들으시려면 9번, 상위 페이지로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode NATN_CODE_P2 = MakeNode("NATN_CODE_P2", "국가번호 2번을 선택하셨습니다. 이집트는 +20, 남수단은 +211, 모로코와 서사하라는 +212, 알제리는 +213, 튀니지는 +216, 리비아는 +218, 감비아는 +220, 세네갈은 +221, 모리타니는 +222, 말리는 +223, 기니는 +224, 코트디부아르는 +225, 부르키나파소는 +226, 니제르는 +227, 토고는 +228, 베냉은 +229, 모리셔스는 +230, 라이베리아는 +231, 시시에라리온은 +232, 가나는 +233, 나이지리아는 +234, 차드는 +235, 중앙아프리카공화국은 +236, 카메룬은 +237, 카보베르데는 +238, 상투메 프린시페는 +239, 적도 기니는 +240, 가봉은 +241, 콩고 공화국은 +242, 콩고민주공화국은 +243, 앙골라는 +244, 기니비사우는 +245, 영국령 인도양 지역은 +246, 어센션 섬은 +247, 세이셸은 +248, 수단은 +249, 르완다는 +250, 에티오피아는 +251, 소말리아와 소말릴란드는 +252, 지부티는 +253, 케냐는 +254, 탄자니아는 +255, 우간다는 +256, 부룬디는 +257, 모잠비크는 +258, 잠비아는 +260, 마다가스카르는 +261, 프랑스령 레위니옹과 마요트는 +262, 짐바브웨는 +263, 나미비아는 +264, 말라위는 +265, 레소토는 +266, 보츠와나는 +267, 에스와티니는 +268, 코모로는 +269, 남아프리카 공화국은 +27, 세인트헬레나와 트리스탄다쿠냐는 +290, 에리트레아는 +291, 네덜란드령 아루바는 +297, 덴마크령 페로 제도는 +298, 그린란드는 +299입니다. 안내를 다시 들으시려면 9번, 상위 페이지로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode NATN_CODE_P3 = MakeNode("NATN_CODE_P3", "국가번호 3번을 선택하셨습니다. 그리스는 +30, 네덜란드는 +31, 벨기에는 +32, 프랑스는 +33, 스페인은 +34, 영국령 지브롤터는 +350, 포르투갈은 +351, 룩셈부르크는 +352, 아일랜드는 +353, 아이슬란드는 +354, 알바니아는 +355, 몰타는 +356, 키프로스는 +357, 핀란드는 +358, 올란드 제도는 +358 18, 불가리아는 +359, 헝가리는 +36, 리투아니아는 +370, 라트비아는 +371, 에스토니아는 +372, 몰도바와 트란스니스트리아는 +373, 아르메니아는 +374, 벨라루스는 +375, 안도라는 +376, 모나코는 +377, 산마리노는 +378, 우크라이나는 +380, 세르비아는 +381, 몬테네그로는 +382, 코소보는 +383, 크로아티아는 +385, 슬로베니아는 +386, 보스니아 헤르체고비나는 +387, 북마케도니아는 +389, 이탈리아는 +39, 바티칸은 +39 06 698입니다. 루마니아는 +40, 스위스는 +41, 체코는 +420, 슬로바키아는 +421, 리히텐슈타인은 +423, 오스트리아는 +43, 영국 본토는 +44, 영국령 건지 섬은 +44 1481, 저지 섬은 +44 1534, 맨 섬은 +44 1624, 덴마크는 +45, 스웨덴은 +46, 노르웨이는 +47, 노르웨이령 스발바르 제도와 얀마옌 섬은 +47 79, 폴란드는 +48, 독일은 +49입니다. 안내를 다시 들으시려면 9번, 상위 페이지로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode NATN_CODE_P5 = MakeNode("NATN_CODE_P5", "국가번호 5번을 선택하셨습니다. 영국령 포클랜드 제도와 사우스조지아 사우스샌드위치 제도는 +500, 벨리즈는 +501, 과테말라는 +502, 엘살바도르는 +503, 온두라스는 +504, 니카라과는 +505, 코스타리카는 +506, 파나마는 +507, 프랑스령 생피에르 미클롱은 +508, 아이티는 +509, 페루는 +51, 멕시코는 +52, 쿠바는 +53, 아르헨티나는 +54, 브라질은 +55, 칠레는 +56, 콜롬비아는 +57, 베네수엘라는 +58, 프랑스령 과들루프와 생마르탱, 생바르텔레미는 +590, 볼리비아는 +591, 가이아나는 +592, 에콰도르는 +593, 프랑스령 기아나는 +594, 파라과이는 +595, 프랑스령 마르티니크는 +596, 수리남은 +597, 우루과이는 +598, 네덜란드령 신트외스타티위스는 +599 3, 사바는 +599 4, 보네르는 +599 7, 퀴라소는 +599 9입니다. 안내를 다시 들으시려면 9번, 상위 페이지로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode NATN_CODE_P6 = MakeNode("NATN_CODE_P6", "국가번호 6번을 선택하셨습니다. 말레이시아는 +60, 호주는 +61, 호주령 코코스 제도는 +61 8 9162, 크리스마스섬은 +61 8 9164, 인도네시아는 +62, 필리핀은 +63, 뉴질랜드와 영국령 핏케언 제도는 +64, 싱가포르는 +65, 태국은 +66, 동티모르는 +670, 호주령 남극 지역은 +672 1, 노퍽섬은 +672 3, 브루나이는 +673, 나우루는 +674, 파푸아뉴기니는 +675, 통가는 +676, 솔로몬 제도는 +677, 바누아투는 +678, 피지는 +679, 팔라우는 +680, 프랑스령 왈리스 푸투나는 +681, 뉴질랜드령 쿡 제도는 +682, 니우에는 +683, 사모아는 +685, 키리바시는 +686, 프랑스령 누벨칼레도니는 +687, 투발루는 +688, 프랑스령 폴리네시아는 +689, 뉴질랜드령 토켈라우는 +690, 미크로네시아 연방은 +691, 마셜 제도는 +692입니다. 안내를 다시 들으시려면 9번, 상위 페이지로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode NATN_CODE_P7 = MakeNode("NATN_CODE_P7", "국가번호 7번을 선택하셨습니다. 러시아는 +7, 카자흐스탄은 +7 6 또는 7 7, 압하지야는 유선전화 +7 840 또는 휴대전화 +7 940, 남오세티야는 유선전화 +7 850 또는 휴대전화 +7 929입니다. 안내를 다시 들으시려면 9번, 상위 페이지로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode NATN_CODE_P8 = MakeNode("NATN_CODE_P8", "국가번호 8번을 선택하셨습니다. 일본은 +81, 대한민국은 +82, 베트남은 +84, 북한은 +850, 홍콩은 +852, 마카오은 +853, 캄보디아는 +855, 라오스는 +856, 중국은 +86, 방글라데시는 +880, 대만은 +886입니다. 안내를 다시 들으시려면 9번, 상위 페이지로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode NATN_CODE_P9 = MakeNode("NATN_CODE_P9", "국가번호 9번을 선택하셨습니다. 튀르키예는 +90, 북키프로스는 +90 533 또는 548, 인도는 +91, 파키스탄은 +92, 아프가니스탄은 +93, 스리랑카는 +94, 미얀마는 +95, 몰디브는 +960, 레바논은 +961, 요르단은 +962, 시리아는 +963, 이라크는 +964, 쿠웨이트는 +965, 사우디아라비아는 +966, 예멘은 +967, 오만은 +968, 팔레스타인은 +970, 아랍에미리트는 +971, 이스라엘은 +972, 바레인은 +973, 카타르는 +974, 부탄은 +975, 몽골은 +976, 네팔은 +977, 이란은 +98, 타지키스탄은 +992, 투르크메니스탄은 +993, 아제르바이잔은 +994, 조지아는 +995, 남오세티야는 +995 34, 키르기스스탄은 +996, 우즈베키스탄은 +998입니다. 안내를 다시 들으시려면 9번, 상위 페이지로 돌아가려면 0번을 눌러주세요.");

    public static ResponseNode NATN_SELECTED = MakeNode("NATN_SELECTED", "{nation}로 확인하였습니다. 감사합니다.");

    // --- 신분 확인 및 연결 설정 ---
    public static ResponseNode IDENTITY_REQ = MakeNode("IDENTITY_REQ", "전화주신 분이 비공식적인 외교를 위한 실무진이라면 1번, 공식적인 외교를 위한 실무진이라면 2번, 국가적인 외교권이 있는 것이 아닌, 각 지역정부 소속이고, 공개적인 외교를 위한 실무진이라면 3번, 외교권이 있는 중앙 정부기관의 고위직 공무원이라면 4번, 중앙 정부기관의 장차관급 공무원이라면 5번, 그 이상은 6번을 눌러주세요.");
    public static ResponseNode IDENTITY_SELECTED = MakeNode("IDENTITY_SELECTED", "{nation}의 {rank_type}유형이시군요?");

    public static ResponseNode TARGET_SETTING = MakeNode("TARGET_SETTING", "긴급한 상황이 있어 대통령과 직접 통화를 원하신다면 1번, 문제가 덜 급하거나 상담원과 연결하시려면 2번, 기타 문의는 3번을 눌러주세요.");

    // --- 직통 회선 인증 및 비상 상황 ---
    public static ResponseNode HOTLINE_AUTH_REQ = MakeNode("HOTLINE_AUTH_REQ", "계속하려면 직통 회선 코드가 필요합니다. 직통 회선 코드 여섯 자리를 입력하신 후 우물 정자를 눌러주세요.");
    public static ResponseNode HOTLINE_AUTH_FAIL = MakeNode("HOTLINE_AUTH_FAIL", "입력하신 코드는 올바른 코드가 아니거나 권한이 부여되지 않았습니다. 직통 회선 코드를 다시 입력하세요. 다시 시도하려면 1번, 상위 메뉴로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode HOTLINE_AUTH_SUCC = MakeNode("HOTLINE_AUTH_SUCC", "반갑습니다. 정부 관계자와 연결해드리는 동안 잠시만 기다려주십시오.");

    public static ResponseNode HOTLINE_ABSENCE = MakeNode("HOTLINE_ABSENCE", "죄송합니다. 지금 연락 가능한 담당자가 부재중에 있습니다. 통화를 예약하려면 1번, 대기가 불가능한 비상상황이 있다면 2번을 눌러주세요.");
    public static ResponseNode HOTLINE_RESERVE = MakeNode("HOTLINE_RESERVE", "예약되었습니다. 현재 시각은 {now}, {res_time}에 통화가 예약되었습니다.");

    public static ResponseNode HOTLINE_EMERGENCY = MakeNode("HOTLINE_EMERGENCY", "상황이 해결될 수 있도록 전력을 다해 도와드리겠습니다. 비상상황의 성격을 알려주십시오. 경제 위기라면 1번, 자연 재해라면 2번, 군사안보적 위험이라면 3번을 눌러주세요.");
    public static ResponseNode MILITARY_RISK = MakeNode("MILITARY_RISK", "군사적 위험의 성격을 알려주십시오. 테러 첩보는 1번, 군사적 충돌 징후는 2번, 국지적인 소요는 3번, 선전포고는 4번, 핵 관련 비상사태는 5번을 눌러주세요.");
    public static ResponseNode NUCLEAR_EMERGENCY = MakeNode("NUCLEAR_EMERGENCY", "핵 관련 비상사태는 굉장히 중요한 문제입니다. 실수 혹은 기술적 문제로 인한 미사일 요격 도움이 필요하시다면 1번, 원자력 발전소 이상 및 파괴 전망 시 2번을 눌러주세요.");

    // --- 군사 행위 및 요격 임무 ---
    public static ResponseNode INTERCEPT_REQ = MakeNode("INTERCEPT_REQ", "미합중국 국방부를 통해 미합중국 공군 특별 요격 임무를 요청할 수 있습니다. 임무 요청을 위해 군사 행위 요청 특별 권한 코드가 필요합니다. 준비되셨으면 1번, 상위 메뉴는 0번을 눌러주세요.");
    public static ResponseNode MIL_CODE_ENTRY = MakeNode("MIL_CODE_ENTRY", "계속 진행하려면 군사 행위 요청 특별 권한 코드 네 자리를 입력하신 후 우물 정자를 눌러주세요.");
    public static ResponseNode MIL_CODE_CONFIRM_1 = MakeNode("MIL_CODE_CONFIRM_1", "귀하께서 입력하신 코드는 {action_desc}를 위한 코드입니다. 계속하시겠습니까? 맞다면 1번, 아니시라면 2번을 눌러주세요.");
    public static ResponseNode MIL_CODE_CONFIRM_2 = MakeNode("MIL_CODE_CONFIRM_2", "정말로 계속하시겠습니까? 한번 수행된 임무는 돌이킬 수 없습니다. 계속하려면 1번, 아니시라면 2번을 눌러주세요.");
    public static ResponseNode MIL_CODE_CANCEL = MakeNode("MIL_CODE_CANCEL", "군사 행위 요청이 중단되었습니다. 처음부터 다시 시도해주십시오.");
    public static ResponseNode MIL_CODE_FAIL = MakeNode("MIL_CODE_FAIL", "입력하신 코드는 올바른 코드가 아닙니다. 다시 시도하려면 1번, 상위 메뉴로 돌아가려면 0번을 눌러주세요.");
    public static ResponseNode MIL_ACTION_START = MakeNode("MIL_ACTION_START", "확인했습니다. 입력하신 코드가 미합중국 국방부에 전달되어 특별 요격 임무를 수행하기로 결정했습니다. 협조해주셔서 감사합니다.");

    // --- 상담원 및 기타 문의 ---
    public static ResponseNode OPERATOR_CONNECT = MakeNode("OPERATOR_CONNECT", "상담원과 연결을 선택하셨습니다. 잠시후 연결해드리겠습니다. 귀하의 대기 번호는 {wait_num}번, 예상 대기 시간은 {wait_min}분 입니다.");
    public static ResponseNode TRAFFIC_OVERLOAD = MakeNode("TRAFFIC_OVERLOAD", "현재 통화량이 비정상적으로 증가하여 대기가 길어지고 있습니다. 계속 대기하려면 1번, 상위 메뉴는 0번을 눌러주세요.");

    public static ResponseNode MISC_INQUIRY = MakeNode("MISC_INQUIRY", "무역 협정은 1번, 비핵화 정책 관련은 2번, 직통 인증 코드 관련 문의는 3번을 눌러주세요.");
    public static ResponseNode NUCLEAR_PROG = MakeNode("NUCLEAR_PROG", "핵무기에 대해 알아보기는 1번, 미국이 사용할 수 있는 무기에 관해서는 2번을 눌러주세요.");
    public static ResponseNode NUCLEAR_LEARN = MakeNode("NUCLEAR_LEARN", "우리는 언제나 평화를 유지할 방법을 강구하고 있습니다. 주요 국가의 미사일에 대해 알아보려면 1번을 눌러주세요. 더 자세한 내용은 미합중국 국제개발처를 방문해주십시오.");

    public static ResponseNode MISSILE_LIST_REQ = MakeNode("MISSILE_LIST_REQ", "러시아 1번, 중국 2번, 인도 3번, 한국 4번, 북한 5번, 일본 6번을 눌러주세요.");
    public static ResponseNode MISSILE_LIST_NK = MakeNode("MISSILE_LIST_NK", "조선민주주의인민공화국의 미사일은 화성-11, 화성-5, 화성-6, 화성-7, 북극성-2, 화살-2, 화성-10, 대륙간 탄도미사일 화성-20 등이 있습니다.");

    // --- 방어 무기 및 코드 확인 ---
    public static ResponseNode US_WEAPONS = MakeNode("US_WEAPONS", "공격 목적 무기는 1번, 요격 등 방어용 무기는 2번을 눌러주세요.");
    public static ResponseNode DEFENSE_WEAPONS = MakeNode("DEFENSE_WEAPONS", "방어 무기 중에는 탄도 미사일을 요격할 수 있는 전투기가 있습니다. 미사일 위력 코드는 1번, 요격 방식 코드는 2번을 눌러주세요.");
    public static ResponseNode POWER_CODE_REQ = MakeNode("POWER_CODE_REQ", "저속 탄도 미사일이라면 1번, 고속 탄도 미사일이라면 2번을 누르세요.");
    public static ResponseNode POWER_CODE_LOW = MakeNode("POWER_CODE_LOW", "저속 탄도 미사일에 대한 군사 행위 요청 특별 권한 코드의 앞 두자리는 {code}입니다.");
    public static ResponseNode POWER_CODE_HIGH = MakeNode("POWER_CODE_HIGH", "고속 탄도 미사일에 대한 군사 행위 요청 특별 권한 코드의 앞 두자리는 {code}입니다.");

    public static ResponseNode INTERCEPT_METHOD = MakeNode("INTERCEPT_METHOD", "북반구 미사일(X0 방식)은 1번, 남반구 미사일(Y0 방식)은 2번을 눌러주세요.");
    public static ResponseNode INTERCEPT_CODE_X0 = MakeNode("INTERCEPT_CODE_X0", "X0 요격 방식에 대한 특별 권한 코드의 뒤 두자리는 {code}입니다.");
    public static ResponseNode INTERCEPT_CODE_Y0 = MakeNode("INTERCEPT_CODE_Y0", "Y0 요격 방식에 대한 특별 권한 코드의 뒤 두자리는 {code}입니다.");

    // --- 코드 신청 및 로봇 확인 ---
    public static ResponseNode CODE_LOST_REQ = MakeNode("CODE_LOST_REQ", "인증 코드 신규 신청은 1번, 기존 코드 확인은 2번을 눌러주세요.");
    public static ResponseNode BOT_CHECK_REQ = MakeNode("BOT_CHECK_REQ", "로봇이 아님을 확인해야 합니다. 다음에 부르는 코드를 입력해주세요. {bot_code}");
    public static ResponseNode BOT_CHECK_FAIL = MakeNode("BOT_CHECK_FAIL", "잘못된 코드입니다. 다시 시도해주세요.");
    public static ResponseNode CODE_ISSUE_SUCC = MakeNode("CODE_ISSUE_SUCC", "직통 회선 코드를 신청해주셔서 감사합니다. 귀하의 코드는 {new_code}입니다.");

    // --- 에러 및 기타 ---
    public static ResponseNode ERROR_404 = MakeNode("ERROR_404", "문제가 발생했습니다. 이 페이지는 없습니다. 상위 페이지로 돌아가려면 0번을 눌러주세요.");

    static CallingInteractionDefinitions()
    {
      ConnectResponseNodes();
    }

    public static void ConnectResponseNodes()
    {
      // --- 1. 시작 및 국적 확인 섹션 ---
      CALLING_START.Transfers = new[]
      {
        new NodeTransfer {
            Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Always },
            ToNode = NATIONALITY_REQ
        }
      };

      CALLING_START.SuppressNavigationHint = true;

      NATIONALITY_REQ.Transfers = new[]
      {
        new NodeTransfer { // # 버튼을 포함한 숫자 입력 시 국적 확인 완료로 이동
            Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.NumberSequence },
            ToNode = NATN_SELECTED
        },
        new NodeTransfer { // * 버튼 입력 시 국가번호 설명으로 이동
            Condition = new NodeTransferCondition {
                Condition = NodeTransferConditionType.SinglePressed,
                Value = new[] { TelephoneButtonType.Star }
            },
            ToNode = NATN_CODE_ENTRY
        }
      };

      NATIONALITY_REQ.SuppressNavigationHint = true;

      // 국가번호 설명 (지역 선택)
      NATN_CODE_ENTRY.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, NATN_CODE_P1),
        CreateTransfer(TelephoneButtonType.Number2, NATN_CODE_P2),
        CreateTransfer(TelephoneButtonType.Number3, NATN_CODE_P3),
        CreateTransfer(TelephoneButtonType.Number5, NATN_CODE_P5),
        CreateTransfer(TelephoneButtonType.Number6, NATN_CODE_P6),
        CreateTransfer(TelephoneButtonType.Number7, NATN_CODE_P7),
        CreateTransfer(TelephoneButtonType.Number8, NATN_CODE_P8),
        CreateTransfer(TelephoneButtonType.Number9, NATN_CODE_P9),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };
      ResponseNode[] regionalNodes = { NATN_CODE_P1, NATN_CODE_P2, NATN_CODE_P3, NATN_CODE_P5, NATN_CODE_P6, NATN_CODE_P7, NATN_CODE_P8, NATN_CODE_P9 };
      foreach (var regionalNode in regionalNodes)
      {
        var tempNode = regionalNode;
        tempNode.Transfers = new[] { CreateTransfer(TelephoneButtonType.Number0, CALLING_START) };
      }

      // 국적 확인 완료 후 신분 확인으로 자동 전이
      NATN_SELECTED.Transfers = new[]
      {
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Always }, ToNode = IDENTITY_REQ }
      };
      NATN_SELECTED.IsContentFormattingRequired = true;

      // --- 2. 신분 확인 및 연결 대상 설정 ---
      IDENTITY_REQ.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, IDENTITY_SELECTED),
        CreateTransfer(TelephoneButtonType.Number2, IDENTITY_SELECTED),
        CreateTransfer(TelephoneButtonType.Number3, IDENTITY_SELECTED),
        CreateTransfer(TelephoneButtonType.Number4, IDENTITY_SELECTED),
        CreateTransfer(TelephoneButtonType.Number5, IDENTITY_SELECTED),
        CreateTransfer(TelephoneButtonType.Number6, IDENTITY_SELECTED),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      IDENTITY_SELECTED.Transfers = new[]
      {
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Always }, ToNode = TARGET_SETTING }
      };
      IDENTITY_SELECTED.IsContentFormattingRequired = true;

      TARGET_SETTING.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, HOTLINE_AUTH_REQ),
        CreateTransfer(TelephoneButtonType.Number2, OPERATOR_CONNECT),
        CreateTransfer(TelephoneButtonType.Number3, MISC_INQUIRY),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      // --- 3. 직통 회선 및 군사 비상 상황 ---
      HOTLINE_AUTH_REQ.Transfers = new[]
      {
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.NumberSequence }, ToNode = HOTLINE_AUTH_SUCC },
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Custom, CustomConditionId = "AUTH_FAIL" }, ToNode = HOTLINE_AUTH_FAIL }
      };

      HOTLINE_AUTH_FAIL.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, HOTLINE_AUTH_REQ),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      HOTLINE_AUTH_SUCC.Transfers = new[]
      {
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Always }, ToNode = HOTLINE_ABSENCE }
      };

      HOTLINE_ABSENCE.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, HOTLINE_RESERVE),
        CreateTransfer(TelephoneButtonType.Number2, HOTLINE_EMERGENCY),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      HOTLINE_RESERVE.IsContentFormattingRequired = true;

      HOTLINE_EMERGENCY.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number2, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number3, MILITARY_RISK),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      MILITARY_RISK.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number5, NUCLEAR_EMERGENCY),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      NUCLEAR_EMERGENCY.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, INTERCEPT_REQ),
        CreateTransfer(TelephoneButtonType.Number2, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      INTERCEPT_REQ.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, MIL_CODE_ENTRY),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      MIL_CODE_ENTRY.Transfers = new[]
      {
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.NumberSequence }, ToNode = MIL_CODE_CONFIRM_1 },
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Custom, CustomConditionId = "INVALID_CODE" }, ToNode = MIL_CODE_CONFIRM_2 }
      };

      MIL_CODE_FAIL.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, MIL_CODE_ENTRY),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      MIL_CODE_CONFIRM_1.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, MIL_CODE_CONFIRM_2),
        CreateTransfer(TelephoneButtonType.Number2, MIL_CODE_CANCEL)
      };
      MIL_CODE_CONFIRM_1.IsContentFormattingRequired = true;

      MIL_CODE_CONFIRM_2.Transfers = new[] {
        CreateTransfer(TelephoneButtonType.Number1, MIL_ACTION_START),
        CreateTransfer(TelephoneButtonType.Number2, MIL_CODE_CANCEL)
      };

      MIL_CODE_CANCEL.Transfers = new[]
      {
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Always }, ToNode = CALLING_START }
      };

      // --- 4. 상담원 및 기타 문의 (핵 정책 등) ---
      OPERATOR_CONNECT.Transfers = new[]
      {
        new NodeTransfer { Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Always }, ToNode = TRAFFIC_OVERLOAD }
      };
      OPERATOR_CONNECT.IsContentFormattingRequired = true;

      TRAFFIC_OVERLOAD.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, TRAFFIC_OVERLOAD), // 계속 대기
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      MISC_INQUIRY.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number2, NUCLEAR_PROG),
        CreateTransfer(TelephoneButtonType.Number3, CODE_LOST_REQ)
      };

      NUCLEAR_PROG.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, NUCLEAR_LEARN),
        CreateTransfer(TelephoneButtonType.Number2, US_WEAPONS)
      };

      NUCLEAR_LEARN.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, MISSILE_LIST_REQ)
      };

      MISSILE_LIST_REQ.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number2, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number3, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number4, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number5, MISSILE_LIST_NK),
        CreateTransfer(TelephoneButtonType.Number6, ERROR_404)
      };

      US_WEAPONS.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, ERROR_404),
        CreateTransfer(TelephoneButtonType.Number2, DEFENSE_WEAPONS)
      };

      DEFENSE_WEAPONS.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, DEFENSE_WEAPONS),
        CreateTransfer(TelephoneButtonType.Number2, INTERCEPT_METHOD)
      };

      INTERCEPT_METHOD.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, INTERCEPT_CODE_X0),
        CreateTransfer(TelephoneButtonType.Number2, INTERCEPT_CODE_Y0),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      MISSILE_LIST_NK.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };
      DEFENSE_WEAPONS.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, POWER_CODE_REQ),
        CreateTransfer(TelephoneButtonType.Number2, INTERCEPT_METHOD),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      // 미사일 위력 코드 선택
      POWER_CODE_REQ.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number1, POWER_CODE_LOW),
        CreateTransfer(TelephoneButtonType.Number2, POWER_CODE_HIGH),
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };

      // 위력 코드 결과 페이지 (0번 시 상위 메뉴)
      POWER_CODE_LOW.Transfers = new[] { CreateTransfer(TelephoneButtonType.Number0, CALLING_START) };
      POWER_CODE_LOW.IsContentFormattingRequired = true;
      POWER_CODE_HIGH.Transfers = new[] { CreateTransfer(TelephoneButtonType.Number0, CALLING_START) };
      POWER_CODE_HIGH.IsContentFormattingRequired = true;

      // 요격 방식 코드 결과 페이지 (0번 시 상위 메뉴)
      INTERCEPT_CODE_X0.Transfers = new[] { CreateTransfer(TelephoneButtonType.Number0, CALLING_START) };
      INTERCEPT_CODE_X0.IsContentFormattingRequired = true;
      INTERCEPT_CODE_Y0.Transfers = new[] { CreateTransfer(TelephoneButtonType.Number0, CALLING_START) };
      INTERCEPT_CODE_Y0.IsContentFormattingRequired = true;

      // --- 5. 코드 복구 (CAPTCHA) 섹션 ---
      CODE_LOST_REQ.Transfers = new[]
      {
        // Additional Logic in Script Required
        CreateTransfer(TelephoneButtonType.Number1, BOT_CHECK_REQ),
        CreateTransfer(TelephoneButtonType.Number2, BOT_CHECK_REQ),
      };


      // --- 5. 코드 복구 (CAPTCHA) 및 발급 섹션 ---

      // 로봇 확인 요청 (CAPTCHA)
      BOT_CHECK_REQ.Transfers = new[]
      {
        new NodeTransfer { 
            // 캡차 번호 일치 시 성공 페이지로
            Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.NumberSequence },
            ToNode = CODE_ISSUE_SUCC
        },
        new NodeTransfer { 
            // 캡차 번호 불일치 시 실패 페이지로 (시스템에서 WRONG_CAPTCHA 이벤트를 던진다고 가정)
            Condition = new NodeTransferCondition { Condition = NodeTransferConditionType.Custom, CustomConditionId = "WRONG_CAPTCHA" },
            ToNode = BOT_CHECK_FAIL
        }
      };
      BOT_CHECK_REQ.IsContentFormattingRequired = true;

      CODE_ISSUE_SUCC.IsContentFormattingRequired = true;

      // --- 6. 공통 에러 처리 ---
      ERROR_404.Transfers = new[]
      {
        CreateTransfer(TelephoneButtonType.Number0, CALLING_START)
      };
    }

    /// <summary>
    /// 단일 버튼 입력을 통한 노드 전이를 생성하는 헬퍼 메서드
    /// </summary>
    private static NodeTransfer CreateTransfer(TelephoneButtonType button, ResponseNode targetNode)
    {
      return new NodeTransfer
      {
        Condition = new NodeTransferCondition
        {
          Condition = NodeTransferConditionType.SinglePressed,
          Value = new[] { button }
        },
        ToNode = targetNode
      };
    }

    private static ResponseNode MakeNode(string id, string description, bool suppressNavHint = false)
    {
      return new ResponseNode
      {
        Id = id,
        Description = description,
        CustomContentId = null,
        SuppressNavigationHint = suppressNavHint,
        ContentL10NKey = $"L10N_KEY_{id}",
        AudioL10NKey = id,
        IsContentFormattingRequired = false,
        Transfers = System.Array.Empty<NodeTransfer>()
      };
    }
  }
}
