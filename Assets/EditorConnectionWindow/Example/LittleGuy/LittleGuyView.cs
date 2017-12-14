using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyView : MonoBehaviour
{

	[SerializeField] private Animator AnimationControl;
	

	public void StartIdleAnimation()
	{
		AnimationControl.SetTrigger("Idle");	
	}
	
	public void StartWalkAnimation()
	{
		AnimationControl.SetTrigger("Walk");	
	}
}
