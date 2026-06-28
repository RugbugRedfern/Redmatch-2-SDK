using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CustomMapSecurityValidator
{
	static Type[] whitelistedComponents = {
		typeof(Transform),
		typeof(Collider),
		typeof(MeshFilter),
		typeof(Renderer),
		Type.GetType("UnityEngine.ProBuilder.ProBuilderMesh, Unity.ProBuilder, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", false),
		Type.GetType("UnityEngine.ProBuilder.PolyShape, Unity.ProBuilder, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", false),
		typeof(AudioSource),
		typeof(AudioReverbZone),
		typeof(AudioListener),
		typeof(ReflectionProbe),
		typeof(LightProbes),
		typeof(LightProbeGroup),
		typeof(ParticleSystem),
		typeof(ParticleSystemForceField),
		typeof(TrailRenderer),
		typeof(LineRenderer),
		typeof(Canvas),
		typeof(CanvasScaler),
		typeof(TextMeshProUGUI),
		typeof(CanvasRenderer),
		typeof(Camera),
		typeof(Light),
		typeof(UpgradeCabinetProxy),
		typeof(KingOfTheHillProxy),
		typeof(Spawnpoint),
		typeof(MapInfo),
		typeof(Animator),
		typeof(MyceliumIdentity),
		typeof(AnimatorSyncer),
		typeof(Activator),
		Type.GetType("UnityEngine.Rendering.PostProcessing.PostProcessLayer, Unity.Postprocessing.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", false),
		Type.GetType("UnityEngine.Rendering.PostProcessing.PostProcessVolume, Unity.Postprocessing.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", false),
		typeof(PostProcessVolumeBuddy),
		typeof(Text),
		typeof(Rigidbody),
		typeof(Outline),
		typeof(Shadow),
		typeof(RigidbodySyncer),
		typeof(Image),
		typeof(RawImage),
		typeof(VerticalLayoutGroup),
		typeof(HorizontalLayoutGroup),
		typeof(CanvasGroup),
		typeof(TMP_Text),
		typeof(CameraFacingBillboard),
		typeof(HealthSyncer),
		typeof(ValueDisplay),
		typeof(DamageableTrigger),
		typeof(StayTrigger),
		typeof(EnterTrigger),
		typeof(ExitTrigger),
		typeof(GameObjectSyncer),
		typeof(TimeTrialProxy),
		typeof(TimeTrialNodeProxy),
		typeof(Terrain),
		typeof(ConstantForce),
		typeof(OcclusionArea),
		typeof(OcclusionPortal),
		typeof(Joint),
		typeof(LODGroup),
		typeof(NavMeshAgent),
		typeof(NavMeshObstacle),
		typeof(OffMeshLink),
		typeof(WindZone),
		typeof(AudioChorusFilter),
		typeof(AudioDistortionFilter),
		typeof(AudioEchoFilter),
		typeof(AudioHighPassFilter),
		typeof(AudioLowPassFilter),
		typeof(AudioReverbFilter),
		typeof(PlayerDiedTrigger),
		typeof(TimeTrialStartedTrigger),
		typeof(TimeTrialFinishedTrigger),
		typeof(MapLoadedTrigger),
		typeof(InputTrigger),
		typeof(IntSyncer),
		typeof(MathActivator),
		typeof(HostActivator),
	};

	public static bool IsGameObjectValid(GameObject go, out string error)
	{
		foreach(var component in go.GetComponentsInChildren(typeof(Component), true))
		{
			// can't be illegal if it doesn't exist
			if(component == null)
			{
				continue;
			}

			bool isSafe = false;

			// go through all whitelisted components
			// if it matches a single one, we're ok!
			foreach(var type in whitelistedComponents)
			{
				if(type.IsInstanceOfType(component))
				{
					isSafe = true;
					break;
				}
			}

			if(!isSafe)
			{
				error = $"{component.gameObject} contains a non-whitelisted component ({component.GetType()})";
				return false;
			}
		}

		error = "";
		return true;
	}

	public static bool IsSceneValid(Scene scene, out string error)
	{
		foreach(var root in scene.GetRootGameObjects())
		{
			if(!IsGameObjectValid(root, out error))
			{
				return false;
			}
		}

		error = "";
		return true;
	}

	public static void SanitizeAnimators(Scene scene)
	{
		foreach(var root in scene.GetRootGameObjects())
		{
			foreach(Animator animator in root.GetComponentsInChildren(typeof(Animator), true))
			{
				animator.fireEvents = false;

				if(animator.runtimeAnimatorController != null)
				{
					foreach(var clip in animator.runtimeAnimatorController.animationClips)
					{
						if(clip.events.Length > 0)
						{
							Debug.Log($"Animator on GameObject {animator.gameObject.name} had events which were removed.");
							clip.events = new AnimationEvent[0];
						}
					}
				}
			}
		}
	}
}