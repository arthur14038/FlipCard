using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	struct ST{
		public int data;
	};

	class CT{
		public int data;
	}

	// Use this for initialization
	void Start () {
		//int a = 1;
		CT ct = new CT();
		ct.data = 1;

		TCT(ct);
		Debug.Log(ct.data);
		//TestLog();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void TI(int value)
	{
		value *= 2;
	}

	void TCT(CT tct)
	{
		tct.data = 3;
		tct.data *= 2;
	}

	public void TestLog()
	{
		ST st1 = new ST(){data = 1};
		ST st2 = new ST(){data = 1};
		ST st3;
		CaSt(st1, ref st2, out st3);
		Debug.Log(st1.data);
		Debug.Log(st2.data);
		Debug.Log(st3.data);

		Debug.Log("------------------------");

		CT ct1 = new CT(){data = 1};
		CT ct2 = new CT(){data = 1};
		CT ct3;
		CaCt(ct1, ct2, out ct3);
		Debug.Log(ct1.data);
		Debug.Log(ct2.data);
		Debug.Log(ct3.data);
	}

	void CaSt(ST s1, ref ST s2, out ST s3)
	{
		s1.data *= 2;
		s2.data *=2;
		s3 = s2;
		s3.data *= 2;
	}

	void CaCt(CT c1, CT c2, out CT c3)
	{
		c1.data *= 2;
		c2.data *=2;
		c3 = c2;
		c3.data *= 2;
	}
}
