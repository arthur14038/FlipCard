﻿using UnityEngine;
using System.Collections;

public delegate void VoidNoneParameter();
public delegate void VoidBool(bool value);
public delegate void VoidInt(int value);
public delegate void VoidTwoInt(int value, int value2);
public delegate void VoidGameObject(GameObject go);
public delegate void VoidTransform(Transform target);
public delegate void VoidCardArrayLevelGameMode(CardArrayLevel level, GameMode mode);
public delegate void VoidString(string value);
public delegate void VoidCard(Card card, bool checkMatch);
public delegate void VoidVector2(Vector2 vec2);
public delegate void VoidVector3(Vector3 vec3);
public delegate void VoidParticleListener(ParticleListener pl);
public delegate void VoidScoreText(ScoreText st);
public delegate void VoidBoolAndCards(bool value, params Card[] cards);
public delegate void VoidPressButtonTool(PressButtonTool pressButton);
public delegate IEnumerator IEnumeratorNoneParameter();