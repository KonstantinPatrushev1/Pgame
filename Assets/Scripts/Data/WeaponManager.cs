using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int weaponId = -1; // ID оружия
    public Sprite weaponSprite; // Спрайт оружия
    public float damage; // Урон оружия
    public Vector2[] customCollider; // Коллайдер
    public static WeaponManager instance;

    private void Awake()
    {
        instance = this;  // Инициализируем синглтон
    }
    void Update()
    {
        switch (weaponId)
        {
            case 7:
                weaponSprite = Resources.Load<Sprite>("axe");
                damage = 10;
                customCollider = new Vector2[] // Используем существующую переменную
                {
                    new Vector2(0.54f, 0.68f),
                    new Vector2(-0.04f, 0.05522266f),
                    new Vector2(0.08f, -0.03656036f),
                    new Vector2(0.65f, 0.56f),
                    new Vector2(0.45f, 0.15f),
                    new Vector2(0.83f, 0.54f)
                };
                break;
            
            case 4:
                weaponSprite = Resources.Load<Sprite>("palka");
                damage = 10;
                customCollider = new Vector2[] // Используем существующую переменную
                {
                    new Vector2(0.5480952f, 1.050072f),
                    new Vector2(-0.04434848f, 0.05522266f),
                    new Vector2(0.1143249f, -0.03656036f),
                    new Vector2(0.7065791f, 0.9573961f)
                };
                break;

            case 5:
                weaponSprite = Resources.Load<Sprite>("wood_sword");
                damage = 50;
                customCollider = new Vector2[]
                {
                    new Vector2(0.6361573f, 0.7797551f),
                    new Vector2(-0.06085439f, 0.07027919f),
                    new Vector2(0.05852706f, -0.06126295f),
                    new Vector2(0.7786385f, 0.6262706f)
                };
                break;
            case 6:
                weaponSprite = null;
                damage = 50;
                customCollider = new Vector2[0];
                break;
            
            case 3:
                weaponSprite = Resources.Load<Sprite>("iron_sword");
                damage = 100;
                customCollider = new Vector2[]
                {
                    new Vector2(0.6361573f, 0.7797551f),
                    new Vector2(-0.06085439f, 0.07027919f),
                    new Vector2(0.05852706f, -0.06126295f),
                    new Vector2(0.7786385f, 0.6262706f)
                };
                break;
            case -1:
                // Логика для пустого состояния оружия
                weaponSprite = null;  // Или другое значение для пустого оружия
                damage = 0;
                customCollider = new Vector2[0];  // Пустой коллайдер
                break;
        }

        ChangeSwordSprite();
        ChangeSwordCollider();
        ChangeSwordDamage();
    }

    void ChangeSwordSprite()
    {
        GetComponent<SwordSpawner>().SetSwordSprite(weaponSprite);
    }

    void ChangeSwordCollider()
    {
        GetComponent<SwordSpawner>().UpdateCollider(customCollider);
    }
    
    void ChangeSwordDamage()
    {
        GetComponent<SwordSpawner>().UpdateGamage(damage);
    }
}