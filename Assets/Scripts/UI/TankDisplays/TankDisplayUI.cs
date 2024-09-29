using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankDisplayUI : MonoBehaviour
{
    [SerializeField] private Gradient DamageGradient;

    [Header("Tank")]
    [SerializeField] private TMP_Text nameText;

    [Header("Damage")]
    [SerializeField] private Image bodyDamageImage;
    [SerializeField] private Image leftTrackDamageImage;
    [SerializeField] private Image rightTrackDamageImage;

    [Header("Ammo")]
    [SerializeField] private TMP_Text ammoInfoText;
    [SerializeField] private Image ammoImage;

    private Tank tank;

    public void SetTank(Tank tank)
    {
        if (this.tank != null)
        {
            if (this.tank.TryGetComponent(out TankBarrelController barrelController))
            {
                barrelController.OnBulletFired -= UpdateTankMagazineUI;
                barrelController.OnBulletReloaded -= UpdateTankMagazineUI;
            }

            if (this.tank.TryGetComponent(out TankDamage tankDamage))
            {
                tankDamage.OnBodyDamaged -= UpdateTankDamageUI;
                tankDamage.OnLeftTrackDamaged -= UpdateTankDamageUI;
                tankDamage.OnRightTrackDamaged -= UpdateTankDamageUI;
            }
        }

        this.tank = tank;

        GameObject UIContainer = transform.GetChild(0).gameObject;
        if (tank == null)
        {
            UIContainer.SetActive(false);
            return;
        }
        UIContainer.SetActive(true);

        UpdateUI();
    }

    private void UpdateTankMagazineUI(object sender, TankBarrelController.TankMagazineState magazineState)
    {
        ammoInfoText.text = $"{magazineState.MaxAmmo - magazineState.UsedAmmo} / {magazineState.MaxAmmo}";
    }

    private void UpdateTankDamageUI(object sender, TankDamage.TankDamageState damageState)
    {
        bodyDamageImage.color = DamageGradient.Evaluate(damageState.BodyDamage);
        leftTrackDamageImage.color = DamageGradient.Evaluate(damageState.LeftTrackDamage);
        rightTrackDamageImage.color = DamageGradient.Evaluate(damageState.RightTrackDamage);
    }

    public void UpdateUI()
    {
        if (tank == null)
            return;

        TankSettings settings = tank.GetSettings();

        nameText.text = settings.name;

        bool hasTankBarrel = tank.TryGetComponent(out TankBarrelController barrelController);
        ammoImage.gameObject.SetActive(hasTankBarrel);
        if (hasTankBarrel)
        {
            ammoImage.sprite = settings.BulletSprite;
            barrelController.OnBulletFired += UpdateTankMagazineUI;
            barrelController.OnBulletReloaded += UpdateTankMagazineUI;

            UpdateTankMagazineUI(barrelController, barrelController.GetTankMagazineState());
        }
        else
        {
            ammoInfoText.text = $"N/A";
        }

        if (tank.TryGetComponent(out TankDamage tankDamage))
        {
            tankDamage.OnBodyDamaged += UpdateTankDamageUI;
            tankDamage.OnLeftTrackDamaged += UpdateTankDamageUI;
            tankDamage.OnRightTrackDamaged += UpdateTankDamageUI;

            UpdateTankDamageUI(tankDamage, tankDamage.GetTankDamageState());
        }
    }
}
