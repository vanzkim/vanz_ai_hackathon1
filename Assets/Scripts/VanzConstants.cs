namespace VanzAI
{
    /// <summary>
    /// 프로젝트 전역에서 사용하는 오브젝트 이름/태그 상수 모음.
    /// 하드코드된 문자열을 제거해 리팩터링 내성을 확보한다.
    /// </summary>
    public static class VanzConstants
    {
        // GameObject Names
        public const string PlayerModelName = "Player_Model";
        public const string PlayerCutsceneName = "Player_Cutscene";
        public const string GameplayCameraName = "CM ThirdPerson Camera";
        public const string PlayerCameraRootName = "PlayerCameraRoot";

        // Tags
        public const string PlayerTag = "Player";
    }
}
