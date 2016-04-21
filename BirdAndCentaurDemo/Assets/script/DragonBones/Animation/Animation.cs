// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Com.Viperstudio.Utils;
namespace DragonBones
{
	public class Animation
	{

		public enum AnimationFadeOutMode
		{
		    NONE, SAME_LAYER, SAME_GROUP, SAME_LAYER_AND_GROUP, ALL
		}

		     
		public bool autoTween;

		public bool _isFading;
		protected bool _isPlaying;
		public float _timeScale;

		protected List<string> _animationList = new List<string>();
		protected List<AnimationData> _animationDataList = new List<AnimationData>();
		protected List<AnimationState> _animationStateList = new List<AnimationState>();
		
		public Armature _armature;
		public AnimationState _lastAnimationState;


		public Animation ()
		{
			_isPlaying = false;
			autoTween = true;
			_timeScale = 1.0f;
			_armature = null;
			_lastAnimationState = null;
		}

		public virtual List<string> getAnimationList()
		{
			return _animationList;
		}
		
		public virtual bool getIsPlaying()
		{
			return _isPlaying && !getIsComplete();
		}
		
		public virtual bool getIsComplete()
		{
			if (_lastAnimationState!=null)
			{
				if (!_lastAnimationState._isComplete)
				{
					return false;
				}
				
				for (int i = 0; i < _animationStateList.Count;  ++i)
				{
					if (!_animationStateList[i]._isComplete)
					{
						return false;
					}
				}
				
				return true;
			}
			
			return true;

		}
		
		public virtual AnimationState getLastAnimationState()
		{
			return _lastAnimationState;
		}
		
		public virtual float getTimeScale()
		{
			return _timeScale;
		}
		
		public virtual void setTimeScale(float timeScale)
		{
			if (timeScale < 0)
			{
				timeScale = 1;
			}
			
			_timeScale = timeScale;

			
		}
		
		public virtual List<AnimationData> getAnimationDataList()
		{
			return _animationDataList;

		}
		
		public virtual void setAnimationDataList(List<AnimationData> animationDataList)
		{
			_animationDataList = animationDataList;
			_animationList.Clear();
			
			for (int i = 0; i < _animationDataList.Count;  ++i)
			{
				_animationList.Add(_animationDataList[i].name);
			}

		}
		
		public virtual AnimationState gotoAndPlay( string animationName,
		                                          float fadeInTime = -1.0f,
		                                          float duration = -1.0f,
		                                          int playTimes = -1,
		                                          int layer = 0,
		                                          string group = "",
		                                          AnimationFadeOutMode fadeOutMode = AnimationFadeOutMode.SAME_LAYER_AND_GROUP,
		                                          bool pauseFadeOut = true,
		                                          bool pauseFadeIn = true
		                                          )
		{
			AnimationData animationData = null;
			
			for (int i = 0; i < _animationDataList.Count;  ++i)
			{
				if (_animationDataList[i].name == animationName)
				{
					animationData = _animationDataList[i];
					break;
				}
			}
			
			if (animationData == null)
			{
				//assert (0);
				//throw std::runtime_error("No animation data.");
				return null;
			}

			_isPlaying = true;
			_isFading = true;
			fadeInTime = fadeInTime < 0 ? (animationData.fadeTime < 0 ? 0.3f : animationData.fadeTime) : fadeInTime;
			float durationScale;
			
			if (duration < 0)
			{
				durationScale = animationData.scale < 0 ? 1.0f : animationData.scale;
			}
			else
			{
				durationScale = duration * 1000.0f / animationData.duration;
			}
			
			if (durationScale == 0)
			{
				durationScale = 0.001f;
			}

			playTimes = playTimes < 0 ? animationData.playTimes : playTimes;
			
			switch (fadeOutMode)
			{
			case AnimationFadeOutMode.NONE:
				break;
				
			case AnimationFadeOutMode.SAME_LAYER:
				for (int i = 0; i< _animationStateList.Count; ++i)
				{
					AnimationState animationState = _animationStateList[i];
					
					if (animationState._layer == layer)
					{
						animationState.fadeOut(fadeInTime, pauseFadeOut);
					}
				}
				
				break;
				
			case AnimationFadeOutMode.SAME_GROUP:
				for (int i = 0; i < _animationStateList.Count;  ++i)
				{
					AnimationState animationState = _animationStateList[i];
					if (animationState._group == group)
					{
						animationState.fadeOut(fadeInTime, pauseFadeOut);
					}
				}
				break;
				
			case AnimationFadeOutMode.ALL:
				for (int i = 0; i < _animationStateList.Count;  ++i)
				{
					AnimationState animationState = _animationStateList[i];
					animationState.fadeOut(fadeInTime, pauseFadeOut);
				}	
				break;

			case AnimationFadeOutMode.SAME_LAYER_AND_GROUP:
			default:
				for (int i = 0; i < _animationStateList.Count; ++i)
				{
					AnimationState animationState = _animationStateList[i];

					if (animationState._layer == layer && animationState._group == group)
					{
						animationState.fadeOut(fadeInTime, pauseFadeOut);
					}
				}
				
				break;
			}
			
			_lastAnimationState = AnimationState.borrowObject();
			_lastAnimationState._layer = layer;
			_lastAnimationState._group = group;
			_lastAnimationState.autoTween = autoTween;
			_lastAnimationState.fadeIn(_armature, animationData, fadeInTime, 1.0f / durationScale, playTimes, pauseFadeIn);
			addState(_lastAnimationState);
			
			for (int i = 0; i < _armature.getSlots().Count; ++i)
			{
				Slot slot = _armature.getSlots()[i];
				
				if (slot._childArmature!=null && slot._childArmature._animation.hasAnimation(animationName))
				{
					slot._childArmature._animation.gotoAndPlay(animationName, fadeInTime);
				}
			}
			
			return _lastAnimationState;

			
		}
		
		public virtual AnimationState gotoAndStop(
			string animationName,
			float time,
			float normalizedTime = -1.0f,
			float fadeInTime = 0,
			float duration = -1.0f,
			int layer = 0,
			string group = "",
			AnimationFadeOutMode fadeOutMode = AnimationFadeOutMode.ALL
			)
		{
			AnimationState animationState = getState(animationName, layer);
			
			if (animationState==null)
			{
				animationState = gotoAndPlay(animationName, fadeInTime, duration, -1, layer, group, fadeOutMode);
			}
			
			if (normalizedTime >= 0)
			{
				animationState.setCurrentTime(animationState.getTotalTime() * normalizedTime);
			}
			else
			{
				animationState.setCurrentTime(time);
			}
			
			animationState.stop();
			return animationState;

		}
		
		public virtual void play()
		{
			if (_animationDataList.Count<=0)
			{
				return;
			}
			
			if (_lastAnimationState == null)
			{
				gotoAndPlay(_animationDataList[0].name);
			}
			else if (!_isPlaying)
			{
				_isPlaying = true;
			}

		}
		public virtual void stop()
		{
				_isPlaying = false;
		}
		public virtual void advanceTime(float passedTime)
		{
			if (!_isPlaying)
			{
				return;
			}
			
			bool isFading = false;
			passedTime *= _timeScale;

			for (int i = 0; i < _animationStateList.Count; ++i)
			{
				AnimationState animationState = _animationStateList[i];
				
				if (animationState.advanceTime(passedTime))
				{
					removeState(animationState);
					--i;

				}
				else if (animationState._fadeState != AnimationState.FadeState.FADE_COMPLETE)
				{
					isFading = true;
				}
			}
			
			_isFading = isFading;
            
		}
		
		public virtual bool hasAnimation(string animationName)
		{
			for (int i = 0; i < _animationDataList.Count;  ++i)
			{
				if (_animationDataList[i].name == animationName)
				{
					return true;
				}
			}
			
			return false;
		}

		public virtual AnimationState getState(string name, int layer = 0)
		{
			for (int i = _animationStateList.Count; i >= 0; i-- )
			{
				AnimationState animationState = _animationStateList[i];
				
				if (animationState.name == name && animationState._layer == layer)
				{
					return animationState;
				}
			}
			
			return null;
		}

	    protected virtual void addState(AnimationState animationState)
		{
			if(	!_animationStateList.Contains (animationState))
			{
				_animationStateList.Add(animationState);
			}
		}

		protected	virtual void removeState(AnimationState animationState)
		{
			if(_animationStateList.Contains (animationState))
			{
				_animationStateList.Remove(animationState);
				AnimationState.returnObject(animationState);

				if (_lastAnimationState == animationState)
				{
					if (_animationStateList.Count<=0)
					{
						_lastAnimationState = null;
					}
					else
					{
						_lastAnimationState = _animationStateList[_animationStateList.Count - 1];
					}
				}
			}


		}
		public	virtual void updateAnimationStates()
        {
		    for (int i = 0; i < _animationStateList.Count;  ++i)
			{
				_animationStateList[i].updateTimelineStates();
			}
		}

       

		public void dispose()
		{
			_animationDataList.Clear();
			
			for (int i = 0; i < _animationStateList.Count;  ++i)
			{
				AnimationState.returnObject(_animationStateList[i]);
			}
			
			_animationStateList.Clear();
			_armature = null;
			_lastAnimationState = null;
		}


	}

		
}
