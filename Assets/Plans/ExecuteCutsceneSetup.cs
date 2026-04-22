using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using Unity.Cinemachine;
using System.IO;

namespace Unity.AI.Assistant.Agent.Dynamic.Extension.Editor
{
    internal class CommandScript : IRunCommand
    {
        public string Title => "Create New Cutscene Setup";
        public string Description => "Creates a Timeline asset, a Manager GameObject, and Virtual Cameras with bindings.";

        public void Execute(ExecutionResult result)
        {
            string timelinePath = "Assets/Timeline/New_Cutscene.playable";
            if (!Directory.Exists("Assets/Timeline"))
            {
                Directory.CreateDirectory("Assets/Timeline");
            }

            // 1. Create Timeline Asset
            TimelineAsset timeline = ScriptableObject.CreateInstance<TimelineAsset>();
            AssetDatabase.CreateAsset(timeline, timelinePath);

            // 2. Setup Scene Manager
            GameObject manager = new GameObject("New_Cutscene_Manager");
            PlayableDirector director = manager.AddComponent<PlayableDirector>();
            director.playableAsset = timeline;

            // Parent under CutScenes if exists
            GameObject cutScenesParent = GameObject.Find("CutScenes");
            if (cutScenesParent != null)
            {
                manager.transform.SetParent(cutScenesParent.transform);
            }

            // 3. Create Cameras
            GameObject camerasRoot = new GameObject("Cameras");
            camerasRoot.transform.SetParent(manager.transform);

            GameObject vcam1Obj = new GameObject("Vcam_Cutscene_01");
            vcam1Obj.transform.SetParent(camerasRoot.transform);
            var vcam1 = vcam1Obj.AddComponent<CinemachineCamera>();

            GameObject vcam2Obj = new GameObject("Vcam_Cutscene_02");
            vcam2Obj.transform.SetParent(camerasRoot.transform);
            var vcam2 = vcam2Obj.AddComponent<CinemachineCamera>();

            // 4. Setup Tracks
            CinemachineTrack cinemachineTrack = timeline.CreateTrack<CinemachineTrack>(null, "Cinemachine Track");

            // Find Main Camera (with Brain)
            GameObject mainCamObj = GameObject.Find("Main Camera");
            if (mainCamObj == null) mainCamObj = GameObject.Find("MainCamera");
            
            if (mainCamObj != null)
            {
                CinemachineBrain brain = mainCamObj.GetComponent<CinemachineBrain>();
                if (brain != null)
                {
                    director.SetGenericBinding(cinemachineTrack, brain);
                }
            }

            // Add Clips
            var clip1 = cinemachineTrack.CreateClip<CinemachineShot>();
            clip1.start = 0;
            clip1.duration = 5;
            clip1.displayName = "Shot 01";
            var shot1 = clip1.asset as CinemachineShot;
            if (shot1 != null)
            {
                director.SetReferenceValue(shot1.VirtualCamera.exposedName, vcam1);
            }

            var clip2 = cinemachineTrack.CreateClip<CinemachineShot>();
            clip2.start = 5;
            clip2.duration = 5;
            clip2.displayName = "Shot 02";
            var shot2 = clip2.asset as CinemachineShot;
            if (shot2 != null)
            {
                director.SetReferenceValue(shot2.VirtualCamera.exposedName, vcam2);
            }

            AssetDatabase.SaveAssets();
            
            // Focus on the new manager
            Selection.activeGameObject = manager;
            
            result.Log("Cutscene setup created successfully at " + timelinePath);
            result.Log("Cameras created: Vcam_Cutscene_01, Vcam_Cutscene_02");
            result.Log("Timeline clips bound to cameras.");
            
            Undo.RegisterCreatedObjectUndo(manager, "Create Cutscene Setup");
        }
    }
}
