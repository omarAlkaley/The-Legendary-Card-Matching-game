using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
	public static EventManager I { get; private set; }

	public event Action<int> OnScoreChanged;
	public event Action OnGameOver;

	[Header("SFX (assign exactly 4)")]
	public AudioClip flipClip;
	public AudioClip matchClip;
	public AudioClip mismatchClip;
	public AudioClip gameoverClip;

	AudioSource src;

	void Awake()
	{
		if (I == null) I = this;
		src = GetComponent<AudioSource>();
		if (src == null) src = gameObject.AddComponent<AudioSource>();
	}

	public void PlayFlipSFX() { if (flipClip != null) src.PlayOneShot(flipClip); }
	public void PlayMatchSFX() { if (matchClip != null) src.PlayOneShot(matchClip); }
	public void PlayMismatchSFX() { if (mismatchClip != null) src.PlayOneShot(mismatchClip); }
	public void PlayGameOverSFX() { if (gameoverClip != null) src.PlayOneShot(gameoverClip); }

	public void DispatchScore( int s ) => OnScoreChanged?.Invoke(s);
	public void DispatchGameOver() { OnGameOver?.Invoke(); PlayGameOverSFX(); }
}
