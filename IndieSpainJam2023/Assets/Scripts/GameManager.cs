using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    [Header("Energy")]
    public float energyCur;
    public float energyMax;
    public float wolfDamage;

    [Header("Wood")]
    public float woodCur;
    public float woodMax;

    [Header("Time Minus")]
    public float energyTimeMinus;
    public float energyTimeMinusRun;

    [Header("UI")]
    [SerializeField] Text txtWood;
    [SerializeField] Image imgEnergy;
    [SerializeField] CanvasGroup hud;
    [SerializeField] Image imgStarMap;
    [SerializeField] CanvasGroup imgTransition;
    [SerializeField] CanvasGroup txtShowMap;

    [Header("Objects")]
    [SerializeField] bool isTelecopeOpen = false;
    [SerializeField] TelescopeManager telescopeManager;
    [SerializeField] GameObject telescope;
    public GameObject starSkyCur;
    public GameObject daySky;

    public float nShrines;

    public Player player;

    public static GameManager instance;

    public bool IsTelecopeOpen { get => isTelecopeOpen; set => isTelecopeOpen = value; }

    void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        ShowHudInfo();
        HungerSystem();

        if (DayNightSystem2D.instance.IsDay())
        {
            AudioManager.instance.PlayMusic("day");
        }
        else if (DayNightSystem2D.instance.IsNight())
        {
            AudioManager.instance.PlayMusic("night");
        }

        if (energyCur <= 0)
        {
            Debug.Log("Lose");
            SceneManager.LoadScene("Lose");
        }
    }

    void ShowHudInfo()
    {
        txtWood.text = "x" + woodCur.ToString();
        imgEnergy.fillAmount = energyCur / energyMax;
    }

    void HungerSystem()
    {
        if (player == null)
            return;
        if (player.CanMove)
        {
            if (player.IsRunning)
            {
                Damage(energyTimeMinusRun * Time.deltaTime);
            }
            else
            {
                Damage(energyTimeMinus * Time.deltaTime);
            }
        }
    }

    public bool Heal(float amount)
    {
        if (energyCur < energyMax)
        {
            energyCur += amount;

            if (energyCur >= energyMax)
                energyCur = energyMax;

            return true;
        }
        return false;
    }

    public bool Damage(float amount)
    {
        if (energyCur > 0)
        {
            energyCur -= amount;
            return true;
        }
        return false;
    }

    public void DamageWolf()
    {
        Damage(wolfDamage);
    }

    public bool AddWood(float amount)
    {
        if (woodCur < woodMax)
        {
            woodCur += amount;
            return true;
        }
        return false;
    }

    public void ResetWood()
    {
        woodCur = 0;
    }

    public void UseTelescope()
    {
        if (starSkyCur == null)
            return;

        if (isTelecopeOpen)
        {
            isTelecopeOpen = false;

            player.CanMove = true;
            player.IsInvincible = false;
            hud.alpha = 1;
            txtShowMap.alpha = 0;
            telescope.SetActive(false);
            DayNightSystem2D.instance.enabled = true;

            Tween tw = imgStarMap.rectTransform.DOAnchorPosY(-2000, 0f);

            daySky.SetActive(false);
            starSkyCur.SetActive(false);
        }
        else
        {
            isTelecopeOpen = true;

            player.CanMove = false;
            player.IsInvincible = true;
            hud.alpha = 0;
            txtShowMap.alpha = 1;
            telescope.SetActive(true);
            DayNightSystem2D.instance.enabled = false;

            if (DayNightSystem2D.instance.IsDay())
                daySky.SetActive(true);

            else if (DayNightSystem2D.instance.IsNight())
                starSkyCur.SetActive(true);
        }
    }

    public void ShowMap()
    {
        //AudioManager.instance.Play("findStar");
        AudioManager.instance.Play("showStarMap");
        Tween tw = imgStarMap.rectTransform.DOAnchorPosY(0, 0.2f);
    }

    public void HideMap()
    {
        Tween tw = imgStarMap.rectTransform.DOAnchorPosY(-2000, 0.2f);
    }

    public void PassToNextMorning()
    {
        StartCoroutine(TransitionToNextMorning());
    }

    IEnumerator TransitionToNextMorning()
    {
        Debug.Log("Game Manager TransitionToNextMorning");

        energyCur = energyMax;

        player.CanMove = false;
        player.CanUseTelescope = false;
        player.IsInvincible = true;

        //Día siguiente
        telescopeManager.gameObject.SetActive(false);
        Sequence sq1 = DOTween.Sequence();
        sq1.Append(imgTransition.DOFade(1, 0.5f));
        yield return sq1.WaitForCompletion();

        yield return new WaitForSeconds(0.5f);
        DayNightSystem2D.instance.enabled = true;
        DayNightSystem2D.instance.ResetDay();

        Sequence sq2 = DOTween.Sequence();
        sq2.Append(imgStarMap.rectTransform.DOAnchorPosY(-2000, 0f));
        sq2.Append(imgTransition.DOFade(0, 0.5f));
        yield return sq2.WaitForCompletion();

        hud.alpha = 1;
        telescope.SetActive(false);
        player.CanMove = true;
        player.CanUseTelescope = true;
        player.IsInvincible = false;

    }

    public bool CanMakeABonfire()
    {
        return woodCur == woodMax;
    }
}