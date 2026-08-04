# Unity Composition Guide

## Purpose

This page preserves a detailed external composition-focused Unity reference that informed the project's composition guidance.

## Scope

Use this as a secondary reference for examples and component-oriented design patterns.

Project-specific source of truth for this repository remains:

- `AGENTS.md`
- `docs/unity/runtime-architecture-guidelines.md`

If this page conflicts with those project rules, follow the project rules.

## External Reference

### Unity Way: Руководство по Композиции и Компонентному Подходу

## 📋 Содержание

1. [Введение](#введение)
2. [Философия Unity Way](#философия-unity-way)
3. [Композиция vs Наследование](#композиция-vs-наследование)
4. [SOLID Принципы в Unity](#solid-принципы-в-unity)
5. [Архитектурные Паттерны](#архитектурные-паттерны)
6. [Практическое Руководство](#практическое-руководство)
7. [Дробление Фич на Компоненты](#дробление-фич-на-компоненты)
8. [Best Practices](#best-practices)
9. [Примеры из Проекта](#примеры-из-проекта)
10. [Типичные Ошибки](#типичные-ошибки)
11. [Чеклист Разработчика](#чеклист-разработчика)
12. [GameShooter: Пример 3D Шутера](#gameshooter-пример-3d-шутера)
13. [Паттерн: Weapon Slot System](#паттерн-weapon-slot-system-система-слотов-оружия)
14. [Паттерн: AI vs Player Controller](#паттерн-ai-vs-player-controller)
15. [Паттерн: ConditionComponent](#паттерн-conditioncomponent-базовый-класс)
16. [Дробление фичи: Система стрельбы](#дробление-фичи-система-стрельбы-в-3d-шутере)
17. [3D vs 2D: Особенности композиции](#3d-vs-2d-особенности-композиции)
18. [Расширенный чеклист для шутера](#расширенный-чеклист-для-шутера)

---

## Введение

### Что такое Unity Way?

**Unity Way** — это подход к разработке игр на Unity, который приоритизирует **композицию над наследованием**. Это означает, что вместо создания глубоких иерархий классов мы создаем небольшие, переиспользуемые компоненты и комбинируем их для достижения сложного поведения.

### Почему это важно?

```
❌ ПЛОХО (Глубокое наследование):
GameObject
  └── Entity
      └── LivingEntity
          └── Character
              ├── Player
              └── Enemy
                  ├── MeleeEnemy
                  └── RangedEnemy

✅ ХОРОШО (Композиция):
GameObject + [LifeComponent, MoveComponent, AttackComponent, ...]
```

**Преимущества композиции:**
- 🔄 Высокая переиспользуемость кода
- 🧩 Гибкость в комбинировании поведений
- 🧪 Легкое тестирование отдельных компонентов
- 📦 Модульность и изоляция логики
- 🚀 Упрощенная поддержка и расширение

---

## Философия Unity Way

### Ключевые Принципы

#### 1. **Один компонент — одна ответственность**

Каждый `MonoBehaviour` должен делать только одну вещь и делать её хорошо.

```csharp
// ✅ ХОРОШО: Компонент отвечает только за здоровье
public class LifeComponent : MonoBehaviour, IDamageTaker
{
    public event Action OnTakeDamage;
    public event Action OnEmpty;

    [SerializeField] private int _maxPoints;
    [SerializeField] private int _hitPoints;

    public bool TakeDamage(int damage)
    {
        _hitPoints -= damage;
        OnTakeDamage?.Invoke();

        if (_hitPoints <= 0)
        {
            OnEmpty?.Invoke();
        }

        return true;
    }

    public bool IsAlive() => _hitPoints > 0;
}
```

```csharp
// ❌ ПЛОХО: "God Object" делает всё
public class Character : MonoBehaviour
{
    private int health;
    private float speed;
    private bool isGrounded;

    void Update()
    {
        HandleInput();      // Ввод
        Move();            // Движение
        CheckGround();     // Физика
        UpdateAnimation(); // Анимация
        PlaySounds();      // Звук
        CheckDamage();     // Урон
        // ... 500 строк кода
    }
}
```

#### 2. **Композиция через SerializeField**

Связывайте компоненты через Inspector, а не через `GetComponent` в runtime.

```csharp
public class Character : MonoBehaviour
{
    // ✅ ХОРОШО: Явные зависимости видны в Inspector
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private MoveComponent _moveComponent;
    [SerializeField] private JumpComponent _jumpComponent;

    // ❌ ПЛОХО: Скрытые зависимости, поиск в runtime
    private void Start()
    {
        var life = GetComponent<LifeComponent>();
        var move = GetComponent<MoveComponent>();
    }
}
```

#### 3. **Event-Driven архитектура**

Компоненты общаются через события, а не прямые вызовы методов.

```csharp
// Компонент публикует события
public class JumpComponent : MonoBehaviour
{
    public event Action OnJump;

    public void Jump()
    {
        // Логика прыжка
        OnJump?.Invoke();
    }
}

// Другие компоненты подписываются
public class Character : MonoBehaviour
{
    [SerializeField] private JumpComponent _jumpComponent;
    [SerializeField] private AudioComponent _audioComponent;

    private void OnEnable()
    {
        _jumpComponent.OnJump += OnJump;
    }

    private void OnDisable()
    {
        _jumpComponent.OnJump -= OnJump;
    }

    private void OnJump()
    {
        _audioComponent.Play(_jumpSound);
    }
}
```

#### 4. **Зависимость от интерфейсов, а не реализаций**

```csharp
// Определяем контракт
public interface IDamageTaker
{
    event Action OnTakeDamage;
    bool TakeDamage(int damage);
}

// Используем интерфейс
public class DamageMakerComponent : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Работаем с интерфейсом, а не конкретным классом
        if (collision.gameObject.TryGetComponent(out IDamageTaker damageTaker))
        {
            damageTaker.TakeDamage(_damage);
        }
    }
}
```

---

## Композиция vs Наследование

### Когда использовать композицию?

**В 95% случаев используйте композицию!**

#### ✅ Используйте композицию когда:

1. **Нужна гибкость в комбинировании поведений**

```csharp
// Персонаж = Жизнь + Движение + Прыжок
Character: LifeComponent + MoveComponent + JumpComponent

// Враг-змея = Жизнь + Подбрасывание + Урон
Snake: LifeComponent + TossComponent + DamageMakerComponent

// Враг-паук = Жизнь + Толчок + Урон + Взгляд
Spider: LifeComponent + PushComponent + DamageMakerComponent + LookComponent
```

2. **Компонент может быть полезен разным типам объектов**

```csharp
// LifeComponent используется всеми живыми сущностями
public class Character : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent; // ✅
}

public class Snake : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent; // ✅
}

public class Spider : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent; // ✅
}
```

3. **Нужно независимо добавлять/удалять функциональность**

```csharp
// Легко добавить звук к прыжку
_jumpComponent.OnJump += () => _audioComponent.Play(_jumpSound);

// Легко добавить эффект частиц
_jumpComponent.OnJump += () => _particleSystem.Play();

// Легко добавить камера-shake
_jumpComponent.OnJump += () => _cameraShake.Shake();
```

### Когда использовать наследование?

**Только в 5% случаев, когда есть чёткое обоснование!**

#### ✅ Используйте наследование когда:

1. **Template Method Pattern — общий алгоритм с вариативными шагами**

```csharp
// Базовый класс определяет общий алгоритм патрулирования
public abstract class BasePatrolComponent : MonoBehaviour
{
    protected Vector3[] _points;
    private int _currentIndex;

    protected virtual void Awake()
    {
        // Шаблонный метод — дочерние классы определяют источник точек
        _points = InitPoints();
    }

    // Общая логика для всех типов патрулирования
    public Vector3 GetCurrentPoint() => _points[_currentIndex];
    public bool IsArrived() => Vector3.Distance(transform.position, GetCurrentPoint()) < 0.1f;
    public void NextPoint() => _currentIndex = (_currentIndex + 1) % _points.Length;

    // Абстрактный метод — каждый подкласс определяет как получить точки
    protected abstract Vector3[] InitPoints();
}

// Патрулирование по точкам из Transform массива
public class PatrolByTransformsComponent : BasePatrolComponent
{
    [SerializeField] private Transform[] _points;

    protected override Vector3[] InitPoints()
    {
        return _points.Select(p => p.position).ToArray();
    }
}

// Патрулирование по точкам из BoxCollider2D
public class PatrolByGroundComponent : BasePatrolComponent
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private BoxCollider2D _groundArea;

    protected override Vector3[] InitPoints()
    {
        // Вычисляет точки патрулирования на основе коллайдера
        var bounds = _groundArea.bounds;
        return new[] { bounds.min, bounds.max };
    }
}
```

**Когда это оправдано:**
- Общая логика составляет 80%+ кода
- Есть чёткое отношение "is-a"
- Только 1-2 метода различаются в реализациях

2. **Специализация с добавлением специфичных данных**

```csharp
// Базовый компонент обнаружения и применения силы
public class DetectAndForceTargetByPointComponent : MonoBehaviour
{
    [SerializeField] protected Transform _forcePoint;
    [SerializeField] protected float _forceValue = 10f;

    public event Action OnForce;

    public Collider2D GetTarget()
    {
        return Physics2D.OverlapCircle(_forcePoint.position, 0.5f);
    }

    public void Force(Collider2D target, Vector3 direction)
    {
        if (target.TryGetComponent(out Rigidbody2D rb))
        {
            rb.AddForce(direction * _forceValue, ForceMode2D.Impulse);
            OnForce?.Invoke();
        }
    }
}

// Специализация добавляет эффекты подбрасывания
public class TossComponent : DetectAndForceTargetByPointComponent
{
    [SerializeField] private AudioClip _tossSound;
    [SerializeField] private ParticleSystem _tossVfx;

    public void Toss()
    {
        var target = GetTarget();
        if (target != null)
        {
            Force(target, Vector3.up);
            _tossVfx?.Play();
        }
    }
}

// Специализация добавляет эффекты толчка
public class PushComponent : DetectAndForceTargetByPointComponent
{
    [SerializeField] private AudioClip _pushSound;
    [SerializeField] private ParticleSystem _pushVfx;

    public void Push()
    {
        var target = GetTarget();
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Force(target, direction);
            _pushVfx?.Play();
        }
    }
}
```

### Правило принятия решения

```
ВОПРОС: Мне нужно добавить новое поведение. Наследование или композиция?

1. Есть ли общий алгоритм с 1-2 вариативными шагами?
   ДА → Рассмотри наследование (Template Method)
   НЕТ → Используй композицию

2. Могу ли я описать отношение как "X является Y"?
   ДА → Рассмотри наследование
   НЕТ → Используй композицию

3. Нужно ли мне комбинировать это поведение с другими?
   ДА → Используй композицию
   НЕТ → Рассмотри наследование

4. Сомневаешься?
   → Используй композицию (всегда безопаснее!)
```

---

## SOLID Принципы в Unity

### S — Single Responsibility Principle (Принцип единственной ответственности)

**Определение:** Каждый компонент должен иметь только одну причину для изменения.

#### ✅ Примеры из проекта:

```csharp
// LifeComponent — только управление здоровьем
public class LifeComponent : MonoBehaviour
{
    private int _hitPoints;
    public bool TakeDamage(int damage) { /* ... */ }
    public bool IsAlive() { /* ... */ }
}

// AudioComponent — только воспроизведение звука
public class AudioComponent : MonoBehaviour
{
    private AudioSource _source;
    public void Play(AudioClip clip) { /* ... */ }
}

// GroundCheckerComponent — только проверка земли
public class GroundCheckerComponent : MonoBehaviour
{
    public bool IsGrounded() { /* ... */ }
}

// ReloadComponent — только управление cooldown
public class ReloadComponent : MonoBehaviour
{
    public bool IsReady() { /* ... */ }
    public void Reload() { /* ... */ }
}
```

#### Практический пример разделения:

```csharp
// ❌ ПЛОХО: Один компонент делает всё
public class JumpController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Физика прыжка
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10);

            // Анимация
            transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);

            // Звук
            GetComponent<AudioSource>().Play();

            // Cooldown
            StartCoroutine(CooldownCoroutine());
        }
    }
}

// ✅ ХОРОШО: Каждый компонент имеет одну ответственность
// 1. Физика прыжка
public class JumpComponent : MonoBehaviour
{
    public event Action OnJump;

    public void Jump()
    {
        _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        OnJump?.Invoke();
    }
}

// 2. Анимация прыжка
public class JumpAnimationComponent : MonoBehaviour
{
    public void AnimateJump()
    {
        transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
    }
}

// 3. Cooldown
public class ReloadComponent : MonoBehaviour
{
    public bool IsReady() => _currentTime >= _reloadTime;
    public void Reload() => _currentTime = 0;
}

// 4. Оркестрация
public class Character : MonoBehaviour
{
    [SerializeField] private JumpComponent _jumpComponent;
    [SerializeField] private JumpAnimationComponent _jumpAnimationComponent;
    [SerializeField] private AudioComponent _audioComponent;
    [SerializeField] private ReloadComponent _reloadComponent;

    private void OnEnable()
    {
        _jumpComponent.OnJump += OnJump;
    }

    private void OnJump()
    {
        _jumpAnimationComponent.AnimateJump();
        _audioComponent.Play(_jumpSound);
        _reloadComponent.Reload();
    }
}
```

### O — Open/Closed Principle (Принцип открытости/закрытости)

**Определение:** Компоненты должны быть открыты для расширения, но закрыты для модификации.

#### ✅ Пример: Система условий

```csharp
// Компонент можно расширять без изменения его кода
public class MoveComponent : MonoBehaviour
{
    private readonly AndCondition _andCondition = new();

    // Добавляем условия извне
    public void AddCondition(Func<bool> condition)
        => _andCondition.AddCondition(condition);

    private void Update()
    {
        // Проверяем все условия
        if (!_andCondition.IsTrue())
            return;

        // Движение
    }
}

// Расширяем без модификации MoveComponent
public class Character : MonoBehaviour
{
    private void Awake()
    {
        // Добавляем условие: двигаться только если жив
        _moveComponent.AddCondition(_lifeComponent.IsAlive);

        // Добавляем условие: двигаться только если не оглушён
        _moveComponent.AddCondition(_stunComponent.IsNotStunned);

        // Можем добавить любое другое условие без изменения MoveComponent
    }
}
```

#### ✅ Пример: Система событий

```csharp
public class LifeComponent : MonoBehaviour
{
    public event Action OnTakeDamage;
    public event Action OnEmpty;

    public bool TakeDamage(int damage)
    {
        _hitPoints -= damage;
        OnTakeDamage?.Invoke(); // Открыто для расширения

        if (_hitPoints <= 0)
        {
            OnEmpty?.Invoke(); // Открыто для расширения
        }

        return true;
    }
}

// Расширяем поведение без изменения LifeComponent
public class Character : MonoBehaviour
{
    private void OnEnable()
    {
        // Добавляем анимацию урона
        _lifeComponent.OnTakeDamage += () => _damageAnimationComponent.AnimateDamage();

        // Добавляем звук урона
        _lifeComponent.OnTakeDamage += () => _audioComponent.Play(_damageSound);

        // Добавляем камера-шейк
        _lifeComponent.OnTakeDamage += () => _cameraShake.Shake();

        // Добавляем UI обновление
        _lifeComponent.OnTakeDamage += () => _healthBar.UpdateHealth();
    }
}
```

### L — Liskov Substitution Principle (Принцип подстановки Барбары Лисков)

**Определение:** Объекты подтипа должны быть заменяемы объектами базового типа без нарушения работы программы.

#### ✅ Пример: Патрулирование

```csharp
// Контроллер работает с любой реализацией BasePatrolComponent
public class PatrolController : MonoBehaviour
{
    [SerializeField] private GameObject _patrolObject;
    [SerializeField] private BasePatrolComponent _patrolComponent; // Базовый тип

    private void Update()
    {
        if (_patrolComponent.IsArrived())
        {
            _patrolComponent.NextPoint();
        }

        // Работает одинаково для PatrolByTransformsComponent
        // и PatrolByGroundComponent
    }
}
```

### I — Interface Segregation Principle (Принцип разделения интерфейса)

**Определение:** Клиенты не должны зависеть от интерфейсов, которые они не используют.

#### ✅ Пример: Разделённые интерфейсы

```csharp
// ❌ ПЛОХО: Толстый интерфейс
public interface IGameEntity
{
    void TakeDamage(int damage);
    void Heal(int amount);
    void Move(Vector3 direction);
    void Attack();
    void UseItem(Item item);
    void LevelUp();
}

// ✅ ХОРОШО: Маленькие, целевые интерфейсы
public interface IDamageTaker
{
    event Action OnTakeDamage;
    bool TakeDamage(int damage);
}

public interface IHealable
{
    void Heal(int amount);
}

public interface IMovable
{
    void Move(Vector3 direction);
}

// Компонент реализует только то, что ему нужно
public class LifeComponent : MonoBehaviour, IDamageTaker, IHealable
{
    public event Action OnTakeDamage;

    public bool TakeDamage(int damage) { /* ... */ }
    public void Heal(int amount) { /* ... */ }
    // Не реализует движение или атаку
}
```

### D — Dependency Inversion Principle (Принцип инверсии зависимостей)

**Определение:** Зависимости должны быть направлены на абстракции, а не на конкретные реализации.

#### ✅ Пример: Зависимость от интерфейса

```csharp
// DamageMakerComponent зависит от интерфейса IDamageTaker
public class DamageMakerComponent : MonoBehaviour
{
    [SerializeField] private int _damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Работаем с абстракцией (интерфейсом)
        if (collision.gameObject.TryGetComponent(out IDamageTaker damageTaker))
        {
            damageTaker.TakeDamage(_damage);
        }
    }
}

// Любой компонент, реализующий IDamageTaker, будет работать
public class LifeComponent : MonoBehaviour, IDamageTaker { }
public class ShieldComponent : MonoBehaviour, IDamageTaker { }
public class ArmorComponent : MonoBehaviour, IDamageTaker { }
```

#### ✅ Пример: Proxy Pattern для делегирования

```csharp
// Дочерний объект (например, голова персонажа) делегирует урон родителю
public class TakeDamageProxy : MonoBehaviour, IDamageTaker
{
    [SerializeField] private LifeComponent _lifeComponent;

    public event Action OnTakeDamage
    {
        add => _lifeComponent.OnTakeDamage += value;
        remove => _lifeComponent.OnTakeDamage -= value;
    }

    public bool TakeDamage(int damage)
    {
        return _lifeComponent.TakeDamage(damage);
    }
}
```

---

## Архитектурные Паттерны

### 1. Component Pattern (Паттерн Компонента)

**Суть Unity:** GameObject сам по себе ничего не делает. Всё поведение добавляется через компоненты.

```csharp
// GameObject — это контейнер для компонентов
public class Character : MonoBehaviour
{
    // Компоненты определяют возможности персонажа
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private MoveComponent _moveComponent;
    [SerializeField] private JumpComponent _jumpComponent;
    [SerializeField] private AttackComponent _attackComponent;
}
```

### 2. Event-Driven Architecture (Событийная архитектура)

**Преимущества:** Слабое связывание, легко добавлять новые реакции.

```csharp
// Publisher (Издатель)
public class LifeComponent : MonoBehaviour
{
    public event Action OnTakeDamage;
    public event Action OnEmpty;
    public event Action<int> OnHealthChanged;

    public bool TakeDamage(int damage)
    {
        _hitPoints -= damage;
        OnTakeDamage?.Invoke();
        OnHealthChanged?.Invoke(_hitPoints);

        if (_hitPoints <= 0)
        {
            OnEmpty?.Invoke();
        }

        return true;
    }
}

// Subscribers (Подписчики)
public class Character : MonoBehaviour
{
    private void OnEnable()
    {
        // Множественные подписчики на одно событие
        _lifeComponent.OnTakeDamage += OnTakeDamage;
        _lifeComponent.OnEmpty += OnHealthEmpty;
        _lifeComponent.OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        // ВАЖНО: Всегда отписываться в OnDisable!
        _lifeComponent.OnTakeDamage -= OnTakeDamage;
        _lifeComponent.OnEmpty -= OnHealthEmpty;
        _lifeComponent.OnHealthChanged -= OnHealthChanged;
    }

    private void OnTakeDamage()
    {
        _damageAnimationComponent.AnimateDamage();
        _audioComponent.Play(_damageSound);
    }

    private void OnHealthEmpty()
    {
        gameObject.SetActive(false);
    }

    private void OnHealthChanged(int newHealth)
    {
        _healthBar.SetHealth(newHealth);
    }
}
```

### 3. Condition System Pattern (Паттерн системы условий)

**Задача:** Контролировать когда компонент может выполнять своё действие.

```csharp
// Система проверки множественных условий
public sealed class AndCondition
{
    private readonly List<Func<bool>> _conditions = new();

    public void AddCondition(Func<bool> condition)
        => _conditions.Add(condition);

    public void RemoveCondition(Func<bool> condition)
        => _conditions.Remove(condition);

    public bool IsTrue()
    {
        for (int i = _conditions.Count - 1; i >= 0; i--)
        {
            if (_conditions[i].Invoke() == false)
                return false;
        }
        return true;
    }
}

// Использование в компонентах
public class MoveComponent : MonoBehaviour
{
    private readonly AndCondition _andCondition = new();

    public void AddCondition(Func<bool> condition)
        => _andCondition.AddCondition(condition);

    private void Move()
    {
        if (!_andCondition.IsTrue())
            return;

        // Логика движения
    }
}

// Настройка условий в Character
public class Character : MonoBehaviour
{
    private void Awake()
    {
        // Персонаж может двигаться только если:
        _moveComponent.AddCondition(_lifeComponent.IsAlive);           // Жив
        _moveComponent.AddCondition(() => !_isStunned);                // Не оглушён
        _moveComponent.AddCondition(() => !_isInCutscene);             // Не в катсцене

        // Персонаж может прыгать только если:
        _jumpComponent.AddCondition(_groundCheckerComponent.IsGrounded); // На земле
        _jumpComponent.AddCondition(_lifeComponent.IsAlive);             // Жив
        _jumpComponent.AddCondition(_jumpReloadComponent.IsReady);       // Cooldown готов
    }
}
```

### 4. Controller Pattern (Паттерн Контроллера)

**Задача:** Отделить ввод (input) от логики игры.

```csharp
// Компоненты содержат логику, но не знают об источнике команд
public class MoveComponent : MonoBehaviour
{
    public void SetDirection(Vector3 direction) { /* ... */ }
}

public class JumpComponent : MonoBehaviour
{
    public void Jump() { /* ... */ }
}

// Контроллер преобразует ввод в команды
public class MoveController : MonoBehaviour
{
    [SerializeField] private GameObject _character;

    private MoveComponent _moveComponent;
    private LookComponent _lookComponent;
    private JumpComponent _jumpComponent;

    private void Awake()
    {
        _moveComponent = _character.GetComponent<MoveComponent>();
        _lookComponent = _character.GetComponent<LookComponent>();
        _jumpComponent = _character.GetComponent<JumpComponent>();
    }

    private void Update()
    {
        // Клавиатурный ввод
        if (Input.GetKey(KeyCode.A))
        {
            _moveComponent.SetDirection(Vector3.left);
            _lookComponent.SetDirection(Vector3.left);
        }

        if (Input.GetKey(KeyCode.D))
        {
            _moveComponent.SetDirection(Vector3.right);
            _lookComponent.SetDirection(Vector3.right);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpComponent.Jump();
        }
    }
}
```

**Преимущества:**
- Легко заменить на AI контроллер
- Легко добавить сетевой контроллер
- Легко записывать и воспроизводить действия (replay system)

```csharp
// AI контроллер использует те же компоненты
public class AIController : MonoBehaviour
{
    [SerializeField] private GameObject _character;
    private MoveComponent _moveComponent;

    private void Update()
    {
        // AI принимает решения
        Vector3 direction = CalculateAIDirection();
        _moveComponent.SetDirection(direction);
    }
}
```

### 5. Component Orchestration Pattern (Паттерн оркестрации компонентов)

**Задача:** Игровой объект координирует работу своих компонентов.

```csharp
public class Character : MonoBehaviour
{
    // Ссылки на компоненты
    [Header("Core Components")]
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private MoveComponent _moveComponent;
    [SerializeField] private JumpComponent _jumpComponent;

    [Header("Visual Components")]
    [SerializeField] private DamageAnimationComponent _damageAnimationComponent;
    [SerializeField] private JumpAnimationComponent _jumpAnimationComponent;

    [Header("Audio Components")]
    [SerializeField] private AudioComponent _audioComponent;
    [SerializeField] private AudioClip _damageSound;
    [SerializeField] private AudioClip _jumpSound;

    [Header("Other Components")]
    [SerializeField] private GroundCheckerComponent _groundCheckerComponent;
    [SerializeField] private ReloadComponent _jumpReloadComponent;

    // Настройка условий
    private void Awake()
    {
        _moveComponent.AddCondition(_lifeComponent.IsAlive);
        _jumpComponent.AddCondition(_groundCheckerComponent.IsGrounded);
        _jumpComponent.AddCondition(_lifeComponent.IsAlive);
        _jumpComponent.AddCondition(_jumpReloadComponent.IsReady);
    }

    // Подписка на события
    private void OnEnable()
    {
        _lifeComponent.OnEmpty += OnHealthEmpty;
        _lifeComponent.OnTakeDamage += OnTakeDamage;
        _jumpComponent.OnJump += OnJump;
    }

    // Отписка от событий
    private void OnDisable()
    {
        _lifeComponent.OnEmpty -= OnHealthEmpty;
        _jumpComponent.OnJump -= OnJump;
        _lifeComponent.OnTakeDamage -= OnTakeDamage;
    }

    // Обработчики событий координируют реакцию компонентов
    private void OnTakeDamage()
    {
        _damageAnimationComponent.AnimateDamage();
        _audioComponent.Play(_damageSound);
    }

    private void OnJump()
    {
        _jumpAnimationComponent.AnimateJump();
        _reloadComponent.Reload();
        _audioComponent.Play(_jumpSound);
    }

    private void OnHealthEmpty()
    {
        gameObject.SetActive(false);
    }
}
```

**Принципы оркестрации:**
1. Объект содержит только ссылки на компоненты
2. В `Awake()` настраивает связи и условия
3. В `OnEnable()`/`OnDisable()` управляет подписками
4. Обработчики событий координируют взаимодействие компонентов
5. Минимум собственной логики

---

## Практическое Руководство

### Шаг 1: Анализ требований

Перед написанием кода, определите:

1. **Какие сущности есть в игре?**
   - Персонаж (Character)
   - Враги (Snake, Spider)
   - Препятствия (Trap, Lava)
   - Интерактивные объекты (Trampoline)

2. **Какие общие возможности они имеют?**
   - Получение урона → `LifeComponent`
   - Движение → `MoveComponent`
   - Нанесение урона → `DamageMakerComponent`
   - Звук → `AudioComponent`

3. **Какие уникальные возможности?**
   - Персонаж: прыжок, управление → `JumpComponent`, `MoveController`
   - Змея: подбрасывание → `TossComponent`
   - Паук: толчок → `PushComponent`
   - Батут: подбрасывание без жизни → `TossComponent`

### Шаг 2: Создание компонентов

#### Чеклист создания компонента:

```
□ Компонент делает только одну вещь?
□ Имя компонента заканчивается на "Component"?
□ Все зависимости через [SerializeField]?
□ Есть публичные события для важных действий?
□ Нет GetComponent в Update/FixedUpdate?
□ Логика изолирована от других компонентов?
```

#### Шаблон базового компонента:

```csharp
using System;
using UnityEngine;

namespace YourGame
{
    /// <summary>
    /// Краткое описание что делает компонент
    /// </summary>
    public class ExampleComponent : MonoBehaviour
    {
        // === СОБЫТИЯ ===
        public event Action OnSomethingHappened;

        // === НАСТРОЙКИ ===
        [Header("Settings")]
        [SerializeField] private float _speed = 5f;
        [SerializeField] private LayerMask _targetLayer;

        // === ЗАВИСИМОСТИ ===
        [Header("Dependencies")]
        [SerializeField] private Rigidbody2D _rigidbody;

        // === ПРИВАТНЫЕ ПОЛЯ ===
        private bool _isActive;

        // === ПУБЛИЧНЫЕ МЕТОДЫ ===
        public void DoSomething()
        {
            // Логика
            OnSomethingHappened?.Invoke();
        }

        // === УСЛОВИЯ ===
        public bool CanDoSomething()
        {
            return _isActive;
        }

        // === UNITY CALLBACKS ===
        private void Awake()
        {
            // Инициализация
        }

        private void Update()
        {
            // Логика каждый кадр
        }
    }
}
```

### Шаг 3: Создание интерфейсов для полиморфизма

```csharp
// Определяем контракты для взаимодействия
public interface IDamageTaker
{
    event Action OnTakeDamage;
    bool TakeDamage(int damage);
}

public interface IHealable
{
    void Heal(int amount);
}

public interface IInteractable
{
    void Interact(GameObject interactor);
}
```

### Шаг 4: Композиция игровых объектов

```csharp
// Игровой объект = Оркестратор компонентов
public class Enemy : MonoBehaviour
{
    // Компоненты, которые определяют возможности врага
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private MoveComponent _moveComponent;
    [SerializeField] private DamageMakerComponent _damageMakerComponent;
    [SerializeField] private AudioComponent _audioComponent;

    private void Awake()
    {
        // Настройка условий
        _moveComponent.AddCondition(_lifeComponent.IsAlive);
    }

    private void OnEnable()
    {
        // Подписка на события
        _lifeComponent.OnEmpty += OnDeath;
    }

    private void OnDisable()
    {
        // Отписка от событий
        _lifeComponent.OnEmpty -= OnDeath;
    }

    private void OnDeath()
    {
        _audioComponent.Play(_deathSound);
        Destroy(gameObject);
    }
}
```

---

## Дробление Фич на Компоненты

### Методология: Feature → Components

#### Пример 1: Фича "Система прыжков"

**Анализ:**
- Физика прыжка (применение силы)
- Проверка земли
- Cooldown между прыжками
- Анимация прыжка
- Звук прыжка

**Разбиение на компоненты:**

```csharp
// 1. Физика прыжка
public class JumpComponent : MonoBehaviour
{
    public event Action OnJump;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _jumpForce = 10f;

    private readonly AndCondition _andCondition = new();

    public void AddCondition(Func<bool> condition)
        => _andCondition.AddCondition(condition);

    public void Jump()
    {
        if (!_andCondition.IsTrue())
            return;

        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        OnJump?.Invoke();
    }
}

// 2. Проверка земли
public class GroundCheckerComponent : MonoBehaviour
{
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            _groundCheckPoint.position,
            _groundCheckRadius,
            _groundLayer
        );
    }
}

// 3. Cooldown
public class ReloadComponent : MonoBehaviour
{
    [SerializeField] private float _reloadTime = 1f;
    private float _currentTime;

    private void Update()
    {
        if (_currentTime < _reloadTime)
        {
            _currentTime += Time.deltaTime;
        }
    }

    public bool IsReady() => _currentTime >= _reloadTime;

    public void Reload()
    {
        _currentTime = 0;
    }
}

// 4. Анимация прыжка
public class JumpAnimationComponent : MonoBehaviour
{
    [SerializeField] private float _punchScale = 0.15f;
    [SerializeField] private float _punchDuration = 0.3f;

    public void AnimateJump()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(new Vector3(0, _punchScale, 0), _punchDuration));
        sequence.Append(transform.DOScaleY(1, 0.1f));
    }
}

// 5. Оркестрация в Character
public class Character : MonoBehaviour
{
    [SerializeField] private JumpComponent _jumpComponent;
    [SerializeField] private JumpAnimationComponent _jumpAnimationComponent;
    [SerializeField] private GroundCheckerComponent _groundCheckerComponent;
    [SerializeField] private ReloadComponent _jumpReloadComponent;
    [SerializeField] private AudioComponent _audioComponent;
    [SerializeField] private AudioClip _jumpSound;

    private void Awake()
    {
        // Настройка условий: можно прыгать только если...
        _jumpComponent.AddCondition(_groundCheckerComponent.IsGrounded);
        _jumpComponent.AddCondition(_jumpReloadComponent.IsReady);
    }

    private void OnEnable()
    {
        _jumpComponent.OnJump += OnJump;
    }

    private void OnDisable()
    {
        _jumpComponent.OnJump -= OnJump;
    }

    private void OnJump()
    {
        _jumpAnimationComponent.AnimateJump();
        _jumpReloadComponent.Reload();
        _audioComponent.Play(_jumpSound);
    }
}
```

#### Пример 2: Фича "Система урона"

**Анализ:**
- Получение урона
- Хранение здоровья
- Нанесение урона
- Визуальный эффект получения урона
- Звук получения урона
- Делегирование урона (для дочерних объектов)

**Разбиение на компоненты:**

```csharp
// 1. Интерфейс
public interface IDamageTaker
{
    event Action OnTakeDamage;
    bool TakeDamage(int damage);
}

// 2. Хранение здоровья и логика получения урона
public class LifeComponent : MonoBehaviour, IDamageTaker
{
    public event Action OnTakeDamage;
    public event Action OnEmpty;

    [SerializeField] private int _maxPoints = 3;
    [SerializeField] private int _hitPoints = 3;

    public bool TakeDamage(int damage)
    {
        if (_hitPoints <= 0)
            return false;

        _hitPoints -= damage;
        OnTakeDamage?.Invoke();

        if (_hitPoints <= 0)
        {
            OnEmpty?.Invoke();
        }

        return true;
    }

    public bool IsAlive() => _hitPoints > 0;
}

// 3. Нанесение урона
public class DamageMakerComponent : MonoBehaviour
{
    public event Action OnMakeDamage;

    [SerializeField] private int _damage = 1;
    [SerializeField] private LayerMask _targetLayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_targetLayer & (1 << collision.gameObject.layer)) == 0)
            return;

        if (collision.gameObject.TryGetComponent(out IDamageTaker damageable))
        {
            if (damageable.TakeDamage(_damage))
            {
                OnMakeDamage?.Invoke();
            }
        }
    }
}

// 4. Визуальный эффект урона
public class DamageAnimationComponent : MonoBehaviour
{
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private float _duration = 0.2f;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void AnimateDamage()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_spriteRenderer.DOColor(_damageColor, _duration / 2));
        sequence.Append(_spriteRenderer.DOColor(Color.white, _duration / 2));
    }
}

// 5. Proxy для дочерних объектов
public class TakeDamageProxy : MonoBehaviour, IDamageTaker
{
    [SerializeField] private LifeComponent _lifeComponent;

    public event Action OnTakeDamage
    {
        add => _lifeComponent.OnTakeDamage += value;
        remove => _lifeComponent.OnTakeDamage -= value;
    }

    public bool TakeDamage(int damage)
    {
        return _lifeComponent.TakeDamage(damage);
    }
}

// 6. Оркестрация
public class Character : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private DamageAnimationComponent _damageAnimationComponent;
    [SerializeField] private AudioComponent _audioComponent;
    [SerializeField] private AudioClip _damageSound;

    private void OnEnable()
    {
        _lifeComponent.OnTakeDamage += OnTakeDamage;
        _lifeComponent.OnEmpty += OnHealthEmpty;
    }

    private void OnDisable()
    {
        _lifeComponent.OnTakeDamage -= OnTakeDamage;
        _lifeComponent.OnEmpty -= OnHealthEmpty;
    }

    private void OnTakeDamage()
    {
        _damageAnimationComponent.AnimateDamage();
        _audioComponent.Play(_damageSound);
    }

    private void OnHealthEmpty()
    {
        gameObject.SetActive(false);
    }
}
```

### Алгоритм дробления фичи:

```
1. ИДЕНТИФИКАЦИЯ
   Опиши фичу одним предложением
   Пример: "Персонаж может прыгать"

2. ДЕКОМПОЗИЦИЯ
   Какие аспекты включает фича?
   - Физика (применение силы)
   - Условия (на земле?)
   - Cooldown (можно ли снова прыгнуть?)
   - Визуализация (анимация)
   - Звук
   - Ввод (откуда команда?)

3. ГРУППИРОВКА
   Какие аспекты логически связаны?
   - Физика + Условия → JumpComponent
   - Cooldown → ReloadComponent (переиспользуемый!)
   - Визуализация → JumpAnimationComponent
   - Звук → AudioComponent (универсальный!)
   - Ввод → MoveController (отдельно!)

4. ИНТЕРФЕЙСЫ
   Нужны ли интерфейсы для полиморфизма?
   - Если компонент взаимодействует с разными типами → ДА
   - Если компонент уникален → НЕТ

5. СОБЫТИЯ
   Какие моменты должны уведомлять другие системы?
   - Успешный прыжок → OnJump
   - Приземление → OnLand
   - и т.д.

6. ВАЛИДАЦИЯ
   □ Каждый компонент делает одну вещь?
   □ Компоненты независимы друг от друга?
   □ Компоненты можно переиспользовать?
   □ Легко тестировать отдельно?
```

---

## Best Practices

### 1. Структура проекта

```
Assets/
└── Game/
    └── Scripts/
        ├── Common/              # Утилиты, общие классы
        │   ├── AndCondition.cs
        │   └── Extensions.cs
        ├── Components/          # Переиспользуемые компоненты
        │   ├── Movement/
        │   │   ├── MoveComponent.cs
        │   │   └── LookComponent.cs
        │   ├── Combat/
        │   │   ├── LifeComponent.cs
        │   │   ├── IDamageTaker.cs
        │   │   └── DamageMakerComponent.cs
        │   ├── Animation/
        │   │   ├── DamageAnimationComponent.cs
        │   │   └── JumpAnimationComponent.cs
        │   └── Audio/
        │       └── AudioComponent.cs
        ├── Controllers/         # Контроллеры ввода
        │   ├── PlayerController.cs
        │   └── AIController.cs
        └── Objects/            # Игровые сущности (оркестраторы)
            ├── Character.cs
            ├── Snake.cs
            └── Spider.cs
```

### 2. Именование

```csharp
// ✅ ХОРОШО
public class LifeComponent : MonoBehaviour { }
public class MoveComponent : MonoBehaviour { }
public class JumpAnimationComponent : MonoBehaviour { }

// ❌ ПЛОХО
public class Life : MonoBehaviour { }       // Непонятно что это компонент
public class Mover : MonoBehaviour { }      // Непонятно что это компонент
public class JumpAnim : MonoBehaviour { }   // Сокращения
```

**Правила именования:**
- Компоненты заканчиваются на `Component`
- Контроллеры заканчиваются на `Controller`
- Интерфейсы начинаются с `I`
- События начинаются с `On`
- Приватные поля начинаются с `_`

### 3. Организация кода в компоненте

```csharp
public class ExampleComponent : MonoBehaviour
{
    // 1. СОБЫТИЯ
    public event Action OnSomething;

    // 2. ПУБЛИЧНЫЕ КОНСТАНТЫ
    public const int MAX_VALUE = 100;

    // 3. СЕРИАЛИЗУЕМЫЕ ПОЛЯ (настройки)
    [Header("Settings")]
    [SerializeField] private float _speed = 5f;

    // 4. СЕРИАЛИЗУЕМЫЕ ПОЛЯ (зависимости)
    [Header("Dependencies")]
    [SerializeField] private Rigidbody2D _rigidbody;

    // 5. ПРИВАТНЫЕ ПОЛЯ
    private bool _isActive;
    private readonly AndCondition _condition = new();

    // 6. ПУБЛИЧНЫЕ СВОЙСТВА
    public bool IsActive => _isActive;

    // 7. UNITY CALLBACKS (в порядке жизненного цикла)
    private void Awake() { }
    private void OnEnable() { }
    private void Start() { }
    private void Update() { }
    private void FixedUpdate() { }
    private void OnDisable() { }
    private void OnDestroy() { }

    // 8. ПУБЛИЧНЫЕ МЕТОДЫ
    public void DoSomething() { }

    // 9. ПРИВАТНЫЕ МЕТОДЫ
    private void InternalMethod() { }
}
```

### 4. Управление подписками на события

```csharp
public class Character : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private JumpComponent _jumpComponent;

    // ✅ ХОРОШО: Подписка в OnEnable, отписка в OnDisable
    private void OnEnable()
    {
        _lifeComponent.OnTakeDamage += OnTakeDamage;
        _lifeComponent.OnEmpty += OnHealthEmpty;
        _jumpComponent.OnJump += OnJump;
    }

    private void OnDisable()
    {
        _lifeComponent.OnTakeDamage -= OnTakeDamage;
        _lifeComponent.OnEmpty -= OnHealthEmpty;
        _jumpComponent.OnJump -= OnJump;
    }

    // ❌ ПЛОХО: Подписка в Start, нет отписки
    private void Start()
    {
        _lifeComponent.OnTakeDamage += OnTakeDamage;
        // Утечка памяти при деактивации/активации объекта!
    }
}
```

**Правила работы с событиями:**
- Подписка → `OnEnable()`
- Отписка → `OnDisable()`
- Всегда отписываться!
- Использовать `?.Invoke()` при вызове событий

### 5. Избегайте GetComponent в Update/FixedUpdate

```csharp
// ❌ ПЛОХО: GetComponent каждый кадр
private void Update()
{
    GetComponent<Rigidbody2D>().velocity = Vector2.right;
}

// ✅ ХОРОШО: Кешировать в Awake
[SerializeField] private Rigidbody2D _rigidbody;

private void Update()
{
    _rigidbody.velocity = Vector2.right;
}

// ✅ ЕЩЁ ЛУЧШЕ: Связать через Inspector
[SerializeField] private Rigidbody2D _rigidbody;
```

### 6. Условия для компонентов

```csharp
// ✅ ХОРОШО: Расширяемая система условий
public class MoveComponent : MonoBehaviour
{
    private readonly AndCondition _andCondition = new();

    public void AddCondition(Func<bool> condition)
        => _andCondition.AddCondition(condition);

    private void Move()
    {
        if (!_andCondition.IsTrue())
            return;

        // Логика движения
    }
}

// ❌ ПЛОХО: Жёсткие зависимости
public class MoveComponent : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private StunComponent _stunComponent;

    private void Move()
    {
        if (!_lifeComponent.IsAlive() || _stunComponent.IsStunned())
            return;

        // Логика движения
    }
}
```

### 7. Используйте SerializeField вместо public

```csharp
// ✅ ХОРОШО: Приватные поля с SerializeField
[SerializeField] private float _speed = 5f;
[SerializeField] private Rigidbody2D _rigidbody;

// ❌ ПЛОХО: Публичные поля
public float speed = 5f;
public Rigidbody2D rigidbody;

// ✅ ИСКЛЮЧЕНИЕ: Когда нужен публичный доступ
[SerializeField] private float _speed = 5f;
public float Speed => _speed; // Read-only свойство
```

### 8. Группировка полей с Header

```csharp
public class Character : MonoBehaviour
{
    [Header("Core Components")]
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private MoveComponent _moveComponent;

    [Header("Visual Components")]
    [SerializeField] private DamageAnimationComponent _damageAnimationComponent;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Audio")]
    [SerializeField] private AudioComponent _audioComponent;
    [SerializeField] private AudioClip _damageSound;
    [SerializeField] private AudioClip _jumpSound;

    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private int _maxHealth = 3;
}
```

### 9. Документирование компонентов

```csharp
/// <summary>
/// Управляет здоровьем игрового объекта.
/// Реализует интерфейс IDamageTaker для получения урона.
/// Публикует события OnTakeDamage и OnEmpty.
/// </summary>
public class LifeComponent : MonoBehaviour, IDamageTaker
{
    /// <summary>
    /// Вызывается при получении урона
    /// </summary>
    public event Action OnTakeDamage;

    /// <summary>
    /// Вызывается когда здоровье достигает нуля
    /// </summary>
    public event Action OnEmpty;

    /// <summary>
    /// Наносит урон объекту
    /// </summary>
    /// <param name="damage">Количество урона</param>
    /// <returns>true если урон был нанесён, false если объект уже мёртв</returns>
    public bool TakeDamage(int damage)
    {
        // ...
    }
}
```

---

## Примеры из Проекта

### Character (Персонаж игрока)

**Композиция:** `Character.cs:1`
```
Character =
    LifeComponent +           // Здоровье
    MoveComponent +           // Движение
    JumpComponent +           // Прыжок
    GroundCheckerComponent +  // Проверка земли
    LookComponent +           // Направление взгляда
    JumpAnimationComponent +  // Анимация прыжка
    DamageAnimationComponent + // Анимация урона
    AudioComponent +          // Звук
    ReloadComponent           // Cooldown прыжка
```

**Ключевые особенности:**
- Минимум логики в самом `Character`
- Все возможности через компоненты
- Настройка условий в `Awake()`
- Event-driven реакции

### Snake (Враг-змея)

**Композиция:** `Snake.cs:1`
```
Snake =
    LifeComponent +           // Здоровье
    TossComponent +           // Подбрасывание игрока
    DamageMakerComponent +    // Нанесение урона
    DamageAnimationComponent + // Анимация получения урона
    AudioComponent            // Звук
```

**Ключевые особенности:**
- Переиспользует `LifeComponent` от персонажа
- `TossComponent` наследуется от базового класса применения силы
- Может наносить и получать урон

### Trampoline (Батут)

**Композиция:** `Trampoline.cs:1`
```
Trampoline =
    CollisionComponent +      // Обнаружение столкновения
    TossComponent +           // Подбрасывание
    AudioComponent            // Звук
```

**Ключевые особенности:**
- Нет `LifeComponent` (не живой объект)
- Переиспользует `TossComponent` от Snake
- Простейший пример композиции

### Сравнение: как бы выглядело с наследованием?

```csharp
// ❌ ПЛОХО: Глубокое наследование
public class Entity : MonoBehaviour
{
    protected int health;
    protected AudioSource audioSource;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

public class LivingEntity : Entity
{
    protected float speed;

    protected virtual void Move(Vector3 direction)
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}

public class Character : LivingEntity
{
    private float jumpForce;

    private void Jump()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
    }
}

// Проблемы:
// - Snake не может переиспользовать Jump
// - Trampoline не может переиспользовать Toss
// - Изменение Entity влияет на всех потомков
// - Невозможно комбинировать поведения
```

---

## Типичные Ошибки

### ❌ Ошибка 1: "God Object"

```csharp
// ❌ ПЛОХО: Один компонент делает всё
public class Player : MonoBehaviour
{
    private int health;
    private float speed;
    private bool isGrounded;
    private Animator animator;
    private AudioSource audioSource;

    private void Update()
    {
        HandleInput();
        Move();
        Jump();
        CheckGround();
        UpdateAnimation();
        CheckHealth();
        PlaySounds();
        // ... 500 строк кода
    }
}

// ✅ ХОРОШО: Разделение ответственности
public class Character : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private MoveComponent _moveComponent;
    [SerializeField] private JumpComponent _jumpComponent;
    [SerializeField] private GroundCheckerComponent _groundCheckerComponent;
    [SerializeField] private AnimationComponent _animationComponent;
    [SerializeField] private AudioComponent _audioComponent;
}
```

### ❌ Ошибка 2: GetComponent в Update

```csharp
// ❌ ПЛОХО: Поиск компонента каждый кадр
private void Update()
{
    GetComponent<Rigidbody2D>().velocity = Vector2.right * 5;
}

// ✅ ХОРОШО: Кеширование
[SerializeField] private Rigidbody2D _rigidbody;

private void Update()
{
    _rigidbody.velocity = Vector2.right * 5;
}
```

### ❌ Ошибка 3: Утечки памяти с событиями

```csharp
// ❌ ПЛОХО: Подписка без отписки
private void Start()
{
    _lifeComponent.OnTakeDamage += OnTakeDamage;
    // Объект может быть деактивирован и активирован снова
    // Подписка произойдёт дважды!
}

// ✅ ХОРОШО: Правильное управление подписками
private void OnEnable()
{
    _lifeComponent.OnTakeDamage += OnTakeDamage;
}

private void OnDisable()
{
    _lifeComponent.OnTakeDamage -= OnTakeDamage;
}
```

### ❌ Ошибка 4: Жёсткие зависимости между компонентами

```csharp
// ❌ ПЛОХО: Прямая зависимость
public class MoveComponent : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent;

    private void Move()
    {
        if (!_lifeComponent.IsAlive())
            return;

        // Движение
    }
}

// ✅ ХОРОШО: Через систему условий
public class MoveComponent : MonoBehaviour
{
    private readonly AndCondition _andCondition = new();

    public void AddCondition(Func<bool> condition)
        => _andCondition.AddCondition(condition);

    private void Move()
    {
        if (!_andCondition.IsTrue())
            return;

        // Движение
    }
}

// В Character:
_moveComponent.AddCondition(_lifeComponent.IsAlive);
```

### ❌ Ошибка 5: Публичные поля вместо SerializeField

```csharp
// ❌ ПЛОХО: Публичные поля
public class MoveComponent : MonoBehaviour
{
    public float speed = 5f;              // Может быть изменено из любого места
    public Rigidbody2D rigidbody;        // Нарушение инкапсуляции
}

// ✅ ХОРОШО: Приватные поля с SerializeField
public class MoveComponent : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Rigidbody2D _rigidbody;

    // Если нужен доступ - через свойство
    public float Speed => _speed;
}
```

### ❌ Ошибка 6: Преждевременное наследование

```csharp
// ❌ ПЛОХО: Наследование для переиспользования одного метода
public class BaseEnemy : MonoBehaviour
{
    protected int health;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
    }
}

public class Snake : BaseEnemy { }
public class Spider : BaseEnemy { }

// ✅ ХОРОШО: Композиция с переиспользуемым компонентом
public class LifeComponent : MonoBehaviour
{
    public bool TakeDamage(int damage) { /* ... */ }
}

public class Snake : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent;
}

public class Spider : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent;
}
```

### ❌ Ошибка 7: Игнорирование интерфейсов

```csharp
// ❌ ПЛОХО: Зависимость от конкретного класса
public class DamageMakerComponent : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var life = collision.gameObject.GetComponent<LifeComponent>();
        if (life != null)
        {
            life.TakeDamage(1);
        }
    }
}

// ✅ ХОРОШО: Зависимость от интерфейса
public interface IDamageTaker
{
    bool TakeDamage(int damage);
}

public class DamageMakerComponent : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageTaker damageTaker))
        {
            damageTaker.TakeDamage(_damage);
        }
    }
}
```

---

## Чеклист Разработчика

### При создании нового компонента:

```
□ Имя заканчивается на "Component"?
□ Наследуется от MonoBehaviour?
□ Делает только одну вещь?
□ Все зависимости через [SerializeField]?
□ Нет GetComponent в Update/FixedUpdate?
□ Есть события для важных действий?
□ Правильная подписка/отписка на события (OnEnable/OnDisable)?
□ Использует приватные поля с _prefix?
□ Логика изолирована от других компонентов?
□ Можно переиспользовать в других объектах?
```

### При создании игрового объекта:

```
□ Объект — это оркестратор компонентов?
□ Минимум собственной логики?
□ Все компоненты связаны через SerializeField?
□ Условия настроены в Awake()?
□ Подписки в OnEnable(), отписки в OnDisable()?
□ Обработчики событий координируют компоненты?
```

### При рефакторинге:

```
□ Можно ли разделить компонент на несколько?
□ Есть ли дублирование кода между компонентами?
□ Можно ли извлечь общую логику в новый компонент?
□ Используется ли наследование там, где подходит композиция?
□ Есть ли "God Objects"?
□ Все ли зависимости явные?
```

### Перед коммитом:

```
□ Код соответствует SOLID принципам?
□ Компоненты независимы и переиспользуемы?
□ Нет утечек памяти (события отписываются)?
□ Нет GetComponent в горячих путях?
□ Интерфейсы используются для полиморфизма?
□ Код документирован (XML комментарии)?
□ Тесты написаны (если применимо)?
```

---

## GameShooter: Пример 3D Шутера

Этот раздел демонстрирует применение Unity Way в контексте 3D шутера с турелями и системой оружия.

### Архитектура проекта GameShooter

```
Assets/GameShooter/
├── Scripts/
│   ├── Common/                    # Утилиты
│   │   └── AndCondition.cs        # Система композиции условий
│   ├── Components/                # Переиспользуемые компоненты
│   │   ├── ConditionComponent.cs  # Базовый класс с условиями
│   │   ├── IDamageable.cs         # Интерфейс получения урона
│   │   ├── ILeftHandComponent.cs  # Интерфейс левой руки
│   │   ├── IRightHandComponent.cs # Интерфейс правой руки
│   │   ├── DetectTargetComponent.cs # Обнаружение целей
│   │   ├── LifeComponent.cs       # Здоровье
│   │   ├── MoveComponent.cs       # Движение
│   │   ├── ReloadComponent.cs     # Перезарядка
│   │   ├── RotateComponent.cs     # Вращение в 3D
│   │   ├── ShootComponent.cs      # Стрельба
│   │   └── TakeDamageProxy.cs     # Прокси для урона
│   ├── Controllers/               # Контроллеры ввода
│   │   ├── MoveController.cs      # Управление движением
│   │   └── ShootController.cs     # Управление стрельбой
│   └── Objects/                   # Игровые сущности
│       ├── Bullet.cs              # Пуля
│       ├── Character.cs           # Персонаж игрока
│       └── Tower.cs               # Турель (AI)
```

### Композиция игровых объектов

#### Character (Игрок)

```
Character =
    LifeComponent +           // Здоровье
    MoveComponent +           // Движение WASD
    RotateComponent +         // Поворот в направлении движения
    ShootComponent (Right) +  // Стрельба правой рукой (Space)
    ShootComponent (Left)     // Стрельба левой рукой (E)
```

```csharp
// Character.cs — Оркестратор компонентов игрока
public class Character : MonoBehaviour, IRightHandComponent, ILeftHandComponent
{
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private RotateComponent _rotateComponent;
    [SerializeField] private MoveComponent _moveComponent;
    [SerializeField] private ShootComponent _rightHandShootComponent;
    [SerializeField] private ShootComponent _leftHandShootComponent;

    private void Awake()
    {
        // Все действия доступны только если персонаж жив
        _rightHandShootComponent.AddCondition(_lifeComponent.IsAlive);
        _leftHandShootComponent.AddCondition(_lifeComponent.IsAlive);
        _rotateComponent.AddCondition(_lifeComponent.IsAlive);
        _moveComponent.AddCondition(_lifeComponent.IsAlive);
    }

    private void OnEnable()
    {
        _lifeComponent.OnEmpty += OnHealthEmpty;
    }

    private void OnDisable()
    {
        _lifeComponent.OnEmpty -= OnHealthEmpty;
    }

    // Реализация интерфейсов для контроллера
    void IRightHandComponent.Shoot() => _rightHandShootComponent.Shoot();
    void ILeftHandComponent.Shoot() => _leftHandShootComponent.Shoot();

    private void OnHealthEmpty()
    {
        gameObject.SetActive(false);
    }
}
```

#### Tower (AI Турель)

```
Tower =
    LifeComponent +           // Здоровье
    RotateComponent +         // Поворот к цели
    ShootComponent +          // Стрельба
    ReloadComponent +         // Перезарядка
    DetectTargetComponent     // Обнаружение игрока
```

```csharp
// Tower.cs — AI-управляемая турель
public class Tower : MonoBehaviour
{
    [SerializeField] private LifeComponent _lifeComponent;
    [SerializeField] private RotateComponent _rotateComponent;
    [SerializeField] private ShootComponent _shootComponent;
    [SerializeField] private ReloadComponent _reloadComponent;
    [SerializeField] private DetectTargetComponent _detectTargetComponent;

    private void Awake()
    {
        // Турель вращается только если жива
        _rotateComponent.AddCondition(_lifeComponent.IsAlive);

        // Турель стреляет только если:
        _shootComponent.AddCondition(_lifeComponent.IsAlive);      // 1. Жива
        _shootComponent.AddCondition(_detectTargetComponent.HasTarget); // 2. Есть цель
        _shootComponent.AddCondition(_reloadComponent.IsReady);    // 3. Перезаряжена
    }

    private void OnEnable()
    {
        _shootComponent.OnFire += _reloadComponent.Reload; // Перезарядка после выстрела
        _lifeComponent.OnEmpty += OnHealthEmpty;
    }

    private void OnDisable()
    {
        _shootComponent.OnFire -= _reloadComponent.Reload;
        _lifeComponent.OnEmpty -= OnHealthEmpty;
    }

    private void Update()
    {
        // AI логика: поворот к цели и попытка выстрела
        _rotateComponent.SetDirection(_detectTargetComponent.GetTarget());
        _shootComponent.Shoot(); // Условия проверяются внутри
    }

    private void OnHealthEmpty()
    {
        Destroy(gameObject);
    }
}
```

#### Bullet (Пуля)

```csharp
// Bullet.cs — Простейший компонент
public class Bullet : MonoBehaviour
{
    [SerializeField] private int _damage = 2;

    private void OnTriggerEnter(Collider other)
    {
        // Работает через интерфейс — может повредить любой IDamageable
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
        }
    }
}
```

---

## Паттерн: Weapon Slot System (Система слотов оружия)

### Проблема

Персонажу нужно несколько слотов для оружия (левая рука, правая рука), каждый с независимым управлением.

### Решение: Интерфейсы для слотов

```csharp
// Интерфейс для правой руки
public interface IRightHandComponent
{
    void Shoot();
}

// Интерфейс для левой руки
public interface ILeftHandComponent
{
    void Shoot();
}
```

### Реализация в Character

```csharp
public class Character : MonoBehaviour, IRightHandComponent, ILeftHandComponent
{
    [SerializeField] private ShootComponent _rightHandShootComponent;
    [SerializeField] private ShootComponent _leftHandShootComponent;

    // Контроллер вызывает через интерфейс
    void IRightHandComponent.Shoot() => _rightHandShootComponent.Shoot();
    void ILeftHandComponent.Shoot() => _leftHandShootComponent.Shoot();
}
```

### Контроллер использует интерфейсы

```csharp
public class ShootController : MonoBehaviour
{
    [SerializeField] private GameObject _character;

    private IRightHandComponent _rightHandComponent;
    private ILeftHandComponent _leftHandComponent;

    private void Awake()
    {
        // Работаем с интерфейсами, а не конкретными классами
        _rightHandComponent = _character.GetComponent<IRightHandComponent>();
        _leftHandComponent = _character.GetComponent<ILeftHandComponent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _rightHandComponent.Shoot();

        if (Input.GetKeyDown(KeyCode.E))
            _leftHandComponent.Shoot();
    }
}
```

### Преимущества

1. **Расширяемость**: Легко добавить новые слоты (ISpecialAbilityComponent, IMeleeComponent)
2. **Полиморфизм**: Разные персонажи могут реализовывать слоты по-разному
3. **Тестируемость**: Контроллер зависит от интерфейсов, можно подменить mock-объектами
4. **Гибкость**: Один ShootComponent можно назначить на разные слоты

---

## Паттерн: AI vs Player Controller

### Ключевая идея

Компоненты не знают, кто ими управляет — игрок или AI. Контроллер определяет источник команд.

### Player Controller

```csharp
public class MoveController : MonoBehaviour
{
    [SerializeField] private GameObject _character;

    private MoveComponent _moveComponent;
    private RotateComponent _rotateComponent;

    private void Awake()
    {
        _moveComponent = _character.GetComponent<MoveComponent>();
        _rotateComponent = _character.GetComponent<RotateComponent>();
    }

    private void Update()
    {
        HandleKeyboard();
    }

    private void HandleKeyboard()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
            direction = Vector3.forward;
        else if (Input.GetKey(KeyCode.DownArrow))
            direction = Vector3.back;
        else if (Input.GetKey(KeyCode.LeftArrow))
            direction = Vector3.left;
        else if (Input.GetKey(KeyCode.RightArrow))
            direction = Vector3.right;

        _moveComponent.SetDirection(direction);
        _rotateComponent.SetDirection(direction);
    }
}
```

### AI Controller (встроен в Tower)

```csharp
// В Tower.Update()
private void Update()
{
    // AI принимает решения автоматически
    var target = _detectTargetComponent.GetTarget();
    _rotateComponent.SetDirection(target);
    _shootComponent.Shoot(); // Условия фильтруют невалидные выстрелы
}
```

### Выделенный AI Controller (альтернатива)

```csharp
public class AITowerController : MonoBehaviour
{
    [SerializeField] private RotateComponent _rotateComponent;
    [SerializeField] private ShootComponent _shootComponent;
    [SerializeField] private DetectTargetComponent _detectTargetComponent;

    private void Update()
    {
        var target = _detectTargetComponent.GetTarget();

        if (target != null)
        {
            _rotateComponent.SetDirection(target);
            _shootComponent.Shoot();
        }
    }
}
```

### Сравнение подходов

| Аспект | Player Controller | AI Controller |
|--------|------------------|---------------|
| Источник команд | Input.GetKey | Алгоритм/AI |
| Компоненты | Те же самые | Те же самые |
| Скорость реакции | Мгновенная | Зависит от Update |
| Сложность | Простой | Может быть сложным |

---

## Паттерн: ConditionComponent (Базовый класс)

### Когда использовать наследование для условий

Если множество компонентов требуют систему условий, можно создать базовый класс.

```csharp
// Базовый класс с системой условий
public class ConditionComponent : MonoBehaviour
{
    protected AndCondition AndCondition = new();

    public void AddCondition(Func<bool> condition)
    {
        AndCondition.AddCondition(condition);
    }
}
```

### ShootComponent наследует ConditionComponent

```csharp
public class ShootComponent : ConditionComponent
{
    public event Action OnFire;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shootPoint;

    public void Shoot()
    {
        // Используем AndCondition из базового класса
        if (!AndCondition.IsTrue())
            return;

        var bullet = Instantiate(_bulletPrefab, _shootPoint.position, Quaternion.identity);

        if (bullet.TryGetComponent(out MoveComponent moveComponent))
            moveComponent.SetDirection(_shootPoint.forward);

        OnFire?.Invoke();
    }
}
```

### Когда это оправдано?

**Используйте наследование когда:**
- Множество компонентов (5+) требуют одинаковую систему условий
- Базовая логика составляет >80% кода
- Нет необходимости комбинировать с другими базовыми классами

**Используйте композицию (поле AndCondition) когда:**
- Мало компонентов с условиями (1-3)
- Нужна гибкость в наследовании от других классов
- Разные компоненты требуют разную логику условий

---

## Дробление фичи: Система стрельбы в 3D шутере

### Шаг 1: Анализ фичи "Стрельба"

**Описание:** Персонаж может стрелять из двух рук, турель автоматически стреляет по игроку.

**Аспекты:**
- Создание пули
- Направление выстрела
- Условия (жив? перезаряжен? есть цель?)
- Перезарядка
- Обнаружение цели (для AI)
- Нанесение урона пулей

### Шаг 2: Декомпозиция на компоненты

```
┌─────────────────────────────────────────────────────────────┐
│                    СИСТЕМА СТРЕЛЬБЫ                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────┐    ┌─────────────────┐                │
│  │ ShootComponent  │    │ ReloadComponent │                │
│  │ ─────────────── │    │ ─────────────── │                │
│  │ • Создаёт пулю  │    │ • Cooldown      │                │
│  │ • Задаёт напр.  │◄───│ • IsReady()     │                │
│  │ • Событие OnFire│    │ • Reload()      │                │
│  └────────┬────────┘    └─────────────────┘                │
│           │                                                 │
│           ▼                                                 │
│  ┌─────────────────┐    ┌─────────────────────┐            │
│  │     Bullet      │    │ DetectTargetComponent│            │
│  │ ─────────────── │    │ ───────────────────  │            │
│  │ • MoveComponent │    │ • Поиск цели         │            │
│  │ • OnTrigger     │    │ • HasTarget()        │            │
│  │ • IDamageable   │    │ • GetTarget()        │            │
│  └─────────────────┘    └─────────────────────┘            │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Шаг 3: Реализация компонентов

#### ShootComponent — создание пули

```csharp
public class ShootComponent : ConditionComponent
{
    public event Action OnFire;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shootPoint;

    public void Shoot()
    {
        if (!AndCondition.IsTrue())
            return;

        // Создаём пулю
        var bullet = Instantiate(_bulletPrefab, _shootPoint.position, Quaternion.identity);

        // Задаём направление через композицию
        if (bullet.TryGetComponent(out MoveComponent moveComponent))
            moveComponent.SetDirection(_shootPoint.forward);

        OnFire?.Invoke();
    }
}
```

#### ReloadComponent — cooldown

```csharp
public class ReloadComponent : MonoBehaviour
{
    [SerializeField] private float _maxTime;
    private float _currentTime;
    private bool _isReady;

    private void Update()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime > _maxTime && !_isReady)
        {
            _isReady = true;
        }
    }

    public bool IsReady() => _isReady;

    public void Reload()
    {
        _isReady = false;
        _currentTime = 0f;
    }
}
```

#### DetectTargetComponent — обнаружение цели

```csharp
public class DetectTargetComponent : MonoBehaviour
{
    [SerializeField] private float _detectDistance = 3f;
    [SerializeField] private GameObject _character;

    public Transform GetTarget()
    {
        var direction = _character.transform.position - transform.position;

        if (direction.sqrMagnitude <= _detectDistance * _detectDistance)
        {
            return _character.transform;
        }

        return null;
    }

    public bool HasTarget() => GetTarget() != null;
}
```

### Шаг 4: Оркестрация в Tower

```csharp
private void Awake()
{
    // Композиция условий
    _shootComponent.AddCondition(_lifeComponent.IsAlive);
    _shootComponent.AddCondition(_detectTargetComponent.HasTarget);
    _shootComponent.AddCondition(_reloadComponent.IsReady);
}

private void OnEnable()
{
    // Связывание событий
    _shootComponent.OnFire += _reloadComponent.Reload;
}
```

### Шаг 5: Валидация по SOLID

| Принцип | Проверка | Статус |
|---------|----------|--------|
| **S** — Single Responsibility | Каждый компонент делает одно дело | ✅ |
| **O** — Open/Closed | Условия добавляются без изменения кода | ✅ |
| **L** — Liskov Substitution | Любой IDamageable может получить урон | ✅ |
| **I** — Interface Segregation | Маленькие интерфейсы (IDamageable, IRightHand) | ✅ |
| **D** — Dependency Inversion | Bullet зависит от IDamageable, не от LifeComponent | ✅ |

---

## 3D vs 2D: Особенности композиции

### RotateComponent для 3D

```csharp
public class RotateComponent : MonoBehaviour
{
    [SerializeField] private Transform _rotationRoot;
    [SerializeField] private float _rotateRate;

    private Vector3 _rotateDirection;
    private AndCondition _andCondition = new();

    public void SetDirection(Vector3 direction)
    {
        _rotateDirection = direction;
    }

    public void SetDirection(Transform target)
    {
        if (target == null) return;

        var direction = target.position - transform.position;
        direction.y = 0f; // Игнорируем вертикаль для горизонтального поворота
        SetDirection(direction);
    }

    private void Rotate()
    {
        if (!_andCondition.IsTrue() || _rotateDirection == Vector3.zero)
            return;

        // 3D поворот через Quaternion
        var targetRotation = Quaternion.LookRotation(_rotateDirection, Vector3.up);
        _rotationRoot.rotation = Quaternion.Lerp(
            _rotationRoot.rotation,
            targetRotation,
            _rotateRate
        );
    }
}
```

### Сравнение 2D и 3D компонентов

| Аспект | 2D Платформер | 3D Шутер |
|--------|---------------|----------|
| **Движение** | `Vector2`, Rigidbody2D | `Vector3`, Transform |
| **Поворот** | `SpriteRenderer.flipX` или `localScale.x` | `Quaternion.LookRotation` |
| **Столкновения** | `OnCollisionEnter2D`, `OnTriggerEnter2D` | `OnCollisionEnter`, `OnTriggerEnter` |
| **Проверка земли** | `Physics2D.OverlapCircle` | `Physics.SphereCast` |
| **Направление** | `Vector2.left/right` | `Vector3.forward/back/left/right` |

### Переиспользуемые компоненты

Несмотря на различия в измерениях, многие компоненты остаются идентичными:

```csharp
// Работает и в 2D, и в 3D без изменений
public class LifeComponent : MonoBehaviour, IDamageable
{
    public event Action OnEmpty;

    [SerializeField] private int _hitPoints;

    public void TakeDamage(int damage)
    {
        _hitPoints -= damage;
        if (_hitPoints <= 0)
            OnEmpty?.Invoke();
    }

    public bool IsAlive() => _hitPoints > 0;
}

// ReloadComponent — независим от измерений
public class ReloadComponent : MonoBehaviour
{
    [SerializeField] private float _maxTime;
    private float _currentTime;

    public bool IsReady() => _currentTime >= _maxTime;
    public void Reload() => _currentTime = 0f;
}

// AndCondition — чистая логика, независима от Unity
public class AndCondition
{
    private readonly List<Func<bool>> _conditions = new();

    public bool IsTrue()
    {
        foreach (var condition in _conditions)
            if (!condition()) return false;
        return true;
    }
}
```

---

## Расширенный чеклист для шутера

### При создании системы оружия:

```
□ Оружие — отдельный компонент (ShootComponent)?
□ Перезарядка — отдельный компонент (ReloadComponent)?
□ Интерфейсы для слотов (ILeftHand, IRightHand)?
□ Пуля использует интерфейс для урона (IDamageable)?
□ Условия стрельбы через AddCondition?
□ Событие OnFire для связи с другими системами?
```

### При создании AI врага:

```
□ AI логика отделена от компонентов?
□ Компоненты такие же, как у игрока?
□ DetectTargetComponent для поиска цели?
□ Условия включают HasTarget?
□ Можно заменить AI на Player Controller?
```

### При переходе с 2D на 3D:

```
□ Vector2 → Vector3?
□ Rigidbody2D → Rigidbody или Transform?
□ OnCollisionEnter2D → OnCollisionEnter?
□ Physics2D → Physics?
□ Quaternion.LookRotation для поворота?
□ Учтена ось Y (вертикаль)?
```

---

## Заключение

### Ключевые принципы Unity Way:

1. **Композиция > Наследование** (в 95% случаев)
2. **Один компонент = одна ответственность** (SRP)
3. **Event-Driven Architecture** (слабое связывание)
4. **Зависимость от интерфейсов** (полиморфизм)
5. **Явные зависимости** (SerializeField)
6. **Расширяемость** (Open/Closed Principle)

### Преимущества подхода:

- 🔄 Высокая переиспользуемость кода
- 🧩 Гибкость в комбинировании поведений
- 🧪 Легкое тестирование
- 📦 Модульность
- 🚀 Упрощенная поддержка
- 👥 Понятность для команды
- 🎯 Фокус на функциональности, а не на иерархии

### Когда использовать наследование:

- Template Method Pattern (общий алгоритм)
- Специализация с сохранением 80%+ базовой логики
- Чёткое отношение "is-a"

### Следующие шаги:

1. Изучите примеры в проекте
2. Практикуйтесь разбивать фичи на компоненты
3. Рефакторите существующий код
4. Применяйте SOLID принципы
5. Пишите переиспользуемые компоненты

---

## Полезные ресурсы

### Статьи и туториалы:
- [How to use script composition in Unity - Game Dev Beginner](https://gamedevbeginner.com/how-to-use-script-composition-in-unity/)
- [Level up your code with game programming patterns - Unity Blog](https://blog.unity.com/games/level-up-your-code-with-game-programming-patterns)
- [Composition Over Inheritance: Best Practices for 2025](https://toxigon.com/composition-over-inheritance-best-practices)
- [Simplifying Game Development with Component-Based Architecture in Unity](https://azumo.com/insights/simplifying-game-development-with-component-based-architecture-in-unity)
- [The Importance and Application of SOLID Principles in Unity](https://medium.com/@mthndmr16/the-importance-and-application-of-solid-principles-in-unity-game-development-94be186ad51f)

### Stack Overflow обсуждения:
- [Composition over inheritance - Weapon system](https://stackoverflow.com/questions/19344639/composition-over-inheritance-weapon-system)
- [Should I avoid using object inheritance as possible to develop a game?](https://gamedev.stackexchange.com/questions/160604/should-i-avoid-using-object-inheritance-as-possible-to-develop-a-game)

### Unity документация:
- [Unity Design Patterns Course](https://learn.unity.com/project/65de084fedbc2a0699d68bfb)
- [Game programming patterns in Unity with C#](https://www.habrador.com/tutorials/programming-patterns/)

---

**Версия:** 2.0
**Дата:** 2025-11-27
**Автор:** Senior Unity Developer & Prompt Engineer
**Проект:** component-way

### История изменений

| Версия | Дата | Изменения |
|--------|------|-----------|
| 1.0 | 2025-11-24 | Начальная версия: основы Unity Way, SOLID, паттерны |
| 2.0 | 2025-11-27 | Добавлен раздел GameShooter: 3D шутер, Weapon Slots, AI Controller, ConditionComponent |
