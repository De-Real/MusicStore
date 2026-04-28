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
