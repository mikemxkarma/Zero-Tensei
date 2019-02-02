using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameControll
{
    public class UIManager : MonoBehaviour
    {
        public float lerpSpeed=2;
        public Slider health;
        public Slider h_used;
        public Slider mana;
        public Slider m_used;
        public Slider stamina;
        public Slider s_used;
        public Text exp;
        private int currentExperience;

        public float sizeMultiplier = 2;

        public void InitExperience(int xp)
        {
            currentExperience = xp;
        }
        public void InitSlider(StatSlider type, int value)
        {
            Slider s = null;
            Slider v = null;

            switch (type)
            {
                case StatSlider.health:
                    s = health;
                    v = h_used;
                    break;
                case StatSlider.mana:
                    s = mana;
                    v = m_used;
                    break;
                case StatSlider.stamina:
                    s = stamina;
                    v = s_used;
                    break;
                default:
                    break;
            }

            s.maxValue = value;
            v.maxValue = value;

            RectTransform r = s.GetComponent<RectTransform>();
            RectTransform r_ = v.GetComponent<RectTransform>();
            float value_actual = value * sizeMultiplier;
            value_actual = Mathf.Clamp(value_actual, 0, 1000);
            r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
            r_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
        }


        public void Tick(CharacterStats stats, float delta)
        {
            health.value = Mathf.Lerp(health.value,stats._health,delta*lerpSpeed*2);
            mana.value = Mathf.Lerp(mana.value, stats._mana, delta * lerpSpeed * 2);
            stamina.value = stats._stamina;
            
            // slow bar descrease
            currentExperience = Mathf.RoundToInt(Mathf.Lerp(currentExperience, stats._fame, delta * lerpSpeed));
            
            exp.text = stats._fame.ToString();
            h_used.value = Mathf.Lerp(h_used.value, stats._health, delta * lerpSpeed);
            m_used.value = Mathf.Lerp(m_used.value, stats._mana, delta * lerpSpeed);
            s_used.value = Mathf.Lerp(s_used.value, stats._stamina, delta * lerpSpeed);
        }
        public void AffectAll(int h, int m, int s)
        {
            InitSlider(StatSlider.health, h);
            InitSlider(StatSlider.mana, m);
            InitSlider(StatSlider.stamina, s);
        }
        public enum StatSlider
        {
            health, mana, stamina
        }

        public static UIManager singleton;
        private void Awake()
        {
            singleton = this;
        }
    }
}

