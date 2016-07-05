﻿using UnityEngine;
using System.Collections;

public delegate void VoidNoneParameter();
public delegate void VoidBool(bool value);
public delegate void VoidInt(int value);
public delegate void VoidTwoInt(int value, int value2);
public delegate void VoidGameObject(GameObject go);
public delegate void VoidTransform(Transform target);
public delegate void VoidString(string value);
public delegate void VoidTwoString(string value, string value2);
public delegate void VoidCardBase(CardBase card);
public delegate void VoidVector2(Vector2 vec2);
public delegate void VoidVector3(Vector3 vec3);
public delegate void VoidParticleListener(ParticleListener pl);
public delegate void VoidScoreText(ScoreText st);
public delegate void VoidBoolAndCards(bool value, params CardBase[] cards);
public delegate void VoidPressButtonTool(PressButtonTool pressButton);
public delegate void VoidThemeInfo(ThemeInfo themeinfo);
public delegate void VoidGameRecord(NormalGameRecord record);
public delegate void VoidPickGameRecord(PickGameRecord record);
public delegate bool BoolNoneParameter();
public delegate bool BoolCardBase(CardBase card);
public delegate IEnumerator IEnumeratorNoneParameter();