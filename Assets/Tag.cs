using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag : MonoBehaviour {
	public enum TagName{
		BABY,
		DIAPER,
		BOTTLE,
		FULL,
		EMPTY,
		DIRTY,
		CLEAN,
		PLAYER,
		CRIB
	}

	public List<TagName> tags;

	public bool HasTag(TagName other) {
		return tags.Contains (other);
	}

	public bool HasTags(TagName[] others) {
		foreach (TagName other in others) {
			if (!tags.Contains (other)) {
				return false;
			}
		}
		return true;
	}
}
