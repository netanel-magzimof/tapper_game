using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DrunksPool : MonoBehaviour
{
        #region Inspector

        [SerializeField] private Drunk template;
        [SerializeField] private RuntimeAnimatorController[] animatorsGreen;
        [SerializeField] private RuntimeAnimatorController[] animatorsRed;
        [SerializeField] private RuntimeAnimatorController[] animatorsPurple;
        [SerializeField] private RuntimeAnimatorController[] animatorsYellow;
        [SerializeField] private Transform[] entranceLocations;
        [SerializeField] private Sprite[] wantedColors;
        #endregion
        
   
        #region Fields

        private RuntimeAnimatorController[][] animators;
        private ObjectPool<Drunk> pool;
        private List<Drunk> activeDrunks;

        #endregion
        
        #region Properties

        public static DrunksPool Shared { get; private set;}

        #endregion
        
        #region MonoBehaviour

        private void Awake()
        {
            Shared = this;
            pool = new ObjectPool<Drunk>(Create, OnActivate, OnDeactivate,defaultCapacity:1);
            activeDrunks = new List<Drunk>();
            animators = new RuntimeAnimatorController[4][]
                { animatorsYellow,animatorsPurple,animatorsGreen, animatorsRed};
        }
        #endregion
        
        #region Methods

        public Drunk Get()
        {
            Drunk drunk =  pool.Get();
            activeDrunks.Add(drunk);
            return drunk;
        }

        public void Release(Drunk drunk)
        {
            drunk.ReleaseMug();
            if (drunk) pool.Release(drunk);
            activeDrunks.Remove(drunk);
        }

        private Drunk Create()
        {
            Drunk drunk = Instantiate(template);
            drunk.gameObject.SetActive(false);
            return drunk;
        }

        private void OnActivate(Drunk drunk)
        {
            drunk.gameObject.SetActive(true);
            int curLocation = Random.Range(0, entranceLocations.Length);
            int curColor = Random.Range(0, animators.Length);
            int curController = Random.Range(0, animators[curColor].Length);
            Vector3 pos = entranceLocations[curLocation].position;
            pos.y += 1.37f;
            drunk.transform.position = pos;
            drunk.animator.runtimeAnimatorController = animators[curColor][curController];
            drunk.SetColor((GameManager.DrinkColors)curColor, wantedColors[curColor]);
        }

        private void OnDeactivate(Drunk drunk)
        {
            drunk.gameObject.SetActive(false);
        }
        public void FreezeAll()
        {
            foreach (Drunk d in activeDrunks)
            {
                d.FreezeDrunk();
            }
        }

        public void ReleaseAll()
        {
            foreach (Drunk m in activeDrunks)
            {
                m.ReleaseMug();

                pool.Release(m);
                
            }
            activeDrunks.Clear();
        }

        #endregion

        
}
