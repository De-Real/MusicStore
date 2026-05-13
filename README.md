# Практична робота №4
## Тактичний Domain-Driven Design: Реалізація Use Case створення замовлення

---

## Value Objects

### `Money` (`Domain/ValueObjects/Money.cs`)
Represents a monetary amount with currency. Immutable record.

- **Validation**: amount must be ≥ 0; currency must be non-empty
- **Factory**: `Money.Create(decimal amount, string currency)`
- **Used in**: `Order.TotalPrice` (mapped to `TotalAmount` + `TotalCurrency` columns via EF Core owned entity)

### `Address` (`Domain/ValueObjects/Address.cs`)
Represents a delivery address. Immutable record.

- **Validation**: Street, City, Country must be non-empty
- **Factory**: `Address.Create(string street, string city, string country, string? postalCode)`
- **Used in**: `Order.ShippingAddress` (mapped to `ShippingAddress*` columns via EF Core owned entity)

---

## Aggregate Root — Order

`Order` (`Domain/Entities/Order.cs`) extends `AggregateRoot`.

- **No public setters** — state changes only through methods
- `Order.Create(customerId, customer, shippingAddress?)` — factory method
- `Order.AddItem(...)` — validates quantity, adds `OrderItem`, recalculates `TotalPrice`
- `Order.Place()` — validates order is non-empty, raises `OrderCreatedEvent`
- `Order.ChangeStatus(OrderStatus)` — enforces the valid state machine (Pending→Confirmed→Shipped→Delivered / Cancelled); throws `InvalidOperationException` on invalid transition
- `Order.GetAllowedTransitions()` — returns allowed next statuses (used by API response)

---

## Domain Events & Dispatcher

### Event: `OrderCreatedEvent` (`Domain/Events/OrderCreatedEvent.cs`)
Raised inside `Order.Place()`. Holds a reference to the `Order` aggregate so the handler can read the DB-assigned Id after persistence.

### Dispatcher: `InMemoryDomainEventDispatcher` (`Infrastructure/Events/InMemoryDomainEventDispatcher.cs`)
Resolves `IDomainEventHandler<TEvent>` from the DI container and invokes them.

**Dispatch flow:**
1. `AppDbContext.SaveChangesAsync` collects aggregates with pending events **before** calling `base.SaveChangesAsync`
2. `base.SaveChangesAsync` runs — database assigns real IDs
3. `InMemoryDomainEventDispatcher.DispatchAndClearAsync` fires all handlers (by this point `Order.Id` is the real DB value)

### Handler: `OrderCreatedEventHandler` (`Application/Orders/EventHandlers/OrderCreatedEventHandler.cs`)
Implements `IDomainEventHandler<OrderCreatedEvent>`. Logs:
```
[Domain Event] Order #42 created for Customer #7. Total: 1250.00 UAH
```
Side effects (logging, emails, etc.) are kept out of the aggregate and executed only through handlers.

---

## Use Case: CreateOrderCommandHandler

`CreateOrderCommandHandler` (`Application/Orders/Commands/CreateOrderCommandHandler.cs`) orchestrates order creation:

1. Loads customer and validates existence
2. Builds `Address` VO from optional command fields
3. Calls `Order.Create(...)` — produces a new aggregate
4. Per item: loads product, checks stock, calculates price via `PricingCalculator`, calls `Order.AddItem(...)` with `Money` VOs
5. Calls `Order.Place()` — raises `OrderCreatedEvent` inside the aggregate
6. Persists via repository; `SaveChangesAsync` override dispatches the event automatically

---

# Практична робота №3  
## Стратегічний Domain-Driven Design: Аналіз домену та проектування меж  
## Проєкт: MusicStore

---

# Етап 1: Виявлення подій, команд та агрегатів

## 1. Domain Events (Що відбулося?)

Domain Events — це події, які вже відбулися в системі.

OrderCreated — замовлення створено, сформовано список товарів і клієнта  
InventoryReserved — товар зарезервовано під замовлення  
OrderConfirmed — замовлення підтверджено  
OrderShipped — замовлення передано доставці  
OrderDelivered — замовлення доставлено клієнту  
OrderCancelled — замовлення скасовано, резерв знято  
PromotionApplied — застосовано знижку  
CustomerCreated — створено нового клієнта  
CustomerUpdated — оновлено дані клієнта  

---

## 2. Commands (Що спричинило подію?)

Commands — це дії, які ініціюють зміну стану системи.

CreateOrder — створити замовлення  
UpdateOrderStatus — змінити статус замовлення  
CancelOrder — скасувати замовлення  
ShipOrder — відправити замовлення  
DeliverOrder — підтвердити доставку  
CreateCustomer — створити клієнта  
UpdateCustomer — оновити дані клієнта  
ApplyPromotion — застосувати акцію  
ReserveInventory — зарезервувати товар  
UpdateStock — оновити склад  

---

## 3. Aggregates (Над чим виконується дія?)

### Order (Замовлення)
Відповідає за життєвий цикл замовлення.

Команди: CreateOrder, UpdateOrderStatus, CancelOrder, ShipOrder, DeliverOrder  
Події: OrderCreated, OrderConfirmed, OrderShipped, OrderDelivered, OrderCancelled  

---

### Inventory (Склад / товар)
Відповідає за кількість товарів та резервування.

Команди: ReserveInventory, UpdateStock  
Події: InventoryReserved  

---

### Customer (Клієнт)
Відповідає за інформацію про клієнта.

Команди: CreateCustomer, UpdateCustomer  
Події: CustomerCreated, CustomerUpdated  

---

### Promotion (Акції)
Відповідає за знижки та акційні правила.

Команди: ApplyPromotion  
Події: PromotionApplied  

---

# Етап 2: Визначення Bounded Contexts

Bounded Context — це межа, всередині якої терміни мають однозначне значення.

---

## Ordering Context
Відповідає за створення та управління замовленнями.  
Містить логіку статусів та складу замовлення.

---

## Inventory Context
Відповідає за склад, кількість товарів та резервування.  

---

## Pricing Context
Відповідає за розрахунок ціни та знижок (лояльність, кількість, акції).  

---

## Promotions Context
Відповідає за створення та управління акціями і знижками.  

---

## Customers Context
Відповідає за дані клієнтів та рівні лояльності.  

---

# Етап 3: Ubiquitous Language

Єдина мова для кожного контексту.

---

## Ordering
Order — замовлення  
Order Item — позиція в замовленні  
Status — стан замовлення  
Pending — очікує  
Confirmed — підтверджено  
Shipped — відправлено  
Delivered — доставлено  
Cancelled — скасовано  

---

## Inventory
Product — товар  
Stock Quantity — кількість на складі  
Reserved Quantity — зарезервовано  
Available Quantity — доступно  
Reservation — резерв  

---

## Pricing
Base Price — базова ціна  
Tier Discount — знижка за рівень лояльності  
Bulk Discount — знижка за кількість  
Promo Discount — акційна знижка  
Final Price — фінальна ціна  

---

## Promotions
Promotion — акція  
Time-Based — за часом  
Category-Based — за категорією  
Discount Percent — відсоток знижки  
Active Promotion — активна акція  

---

## Customers
Customer — клієнт  
Loyalty Tier — рівень лояльності  
Bronze — базовий рівень  
Silver — середній рівень  
Gold — високий рівень  
Total Spent — витрачена сума  

---
