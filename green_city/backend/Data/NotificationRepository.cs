using FakeNewsGame.Api.Models;

namespace FakeNewsGame.Api.Data;

public class NotificationRepository
{
    private readonly List<Notification> _items;
    private static readonly Random _rnd = Random.Shared;

    public NotificationRepository()
    {
        var now = DateTime.UtcNow;

        _items = new List<Notification>
        {
            // ===== РЕАЛЬНЫЕ =====
            new() {
                Id = 1, AppName = "Сбербанк", Sender = "900", Title = "Перевод выполнен",
                Text = "Перевод 5000 ₽ на карту *4477 выполнен 14:32.",
                Timestamp = now.AddMinutes(-12),
                IsFake = false,
                Explanation = "Стандартный формат SMS от 900. Нет ссылок, нет просьб сообщить код из SMS."
            },
            new() {
                Id = 2, AppName = "Telegram", Sender = "Мария", Title = "Сообщение",
                Text = "Ок, давай встретимся в 18:00 у метро.",
                Timestamp = now.AddMinutes(-40),
                IsFake = false,
                Explanation = "Обычная переписка. Нет подозрительных ссылок или просьб."
            },
            new() {
                Id = 3, AppName = "Госуслуги", Sender = "Gosuslugi", Title = "Вход в аккаунт",
                Text = "Выполнен вход в личный кабинет с устройства Android. Если это были не вы — смените пароль.",
                Timestamp = now.AddHours(-2),
                IsFake = false,
                Explanation = "Реальное уведомление Госуслуг. Просит сменить пароль, но не переходить по ссылке."
            },
            new() {
                Id = 4, AppName = "Тинькофф", Sender = "Tinkoff", Title = "Покупка",
                Text = "Покупка в МАГНИТ на 847.50 ₽. Баланс: 12 430 ₽.",
                Timestamp = now.AddHours(-3),
                IsFake = false,
                Explanation = "Типичное push-уведомление банка о покупке. Нет запросов данных."
            },
            new() {
                Id = 5, AppName = "Авито", Sender = "Avito", Title = "Объявление одобрено",
                Text = "Ваше объявление «iPhone 13, 128 GB» прошло модерацию и опубликовано.",
                Timestamp = now.AddHours(-5),
                IsFake = false,
                Explanation = "Стандартное уведомление о публикации объявления."
            },
            new() {
                Id = 6, AppName = "Погода", Sender = "Weather", Title = "Завтра дождь",
                Text = "Завтра в Москве ожидается дождь с 14:00 до 19:00, +12°C.",
                Timestamp = now.AddHours(-6),
                IsFake = false,
                Explanation = "Информационное уведомление без ссылок и призывов к действию."
            },

            // ===== ФЕЙКИ =====
            new() {
                Id = 7, AppName = "Сбербанк", Sender = "SBER-INFO", Title = "⚠ Карта заблокирована!",
                Text = "Ваша карта заблокирована из-за подозрительной активности. Для разблокировки перейдите: http://sber-online-bez.ru",
                Timestamp = now.AddMinutes(-5),
                IsFake = true,
                Explanation = "Фейк. Банк никогда не присылает ссылки на сторонние домены и не угрожает блокировкой в SMS. Домен sber-online-bez.ru — подделка."
            },
            new() {
                Id = 8, AppName = "WhatsApp", Sender = "Мама", Title = "Срочно!",
                Text = "Сынок, скинь 5000 на эту карту 2202 2000 1111 2222, потом объясню.",
                Timestamp = now.AddMinutes(-20),
                IsFake = true,
                Explanation = "Классический взлом аккаунта WhatsApp. Мошенники пишут от имени родственника и просят перевод. Всегда звоните человеку голосом."
            },
            new() {
                Id = 9, AppName = "Госуслуги", Sender = "Gosuslugi-Info", Title = "Вам положена выплата 12 000 ₽",
                Text = "Государство начисляет вам выплату. Заполните форму: gosuslugi-vyplata.info",
                Timestamp = now.AddHours(-1),
                IsFake = true,
                Explanation = "Фейк. Госуслуги не рассылают ссылки на сторонние сайты. Все уведомления — только в личном кабинете на gosuslugi.ru."
            },
            new() {
                Id = 10, AppName = "Telegram", Sender = "Telegram Support", Title = "Аккаунт будет удалён",
                Text = "Ваш аккаунт будет удалён через 24 часа. Подтвердите данные через @support_bot.",
                Timestamp = now.AddHours(-4),
                IsFake = true,
                Explanation = "Фейк. Telegram не пишет в личные сообщения и не просит подтверждать данные через сторонних ботов."
            },
            new() {
                Id = 11, AppName = "Сбербанк", Sender = "900", Title = "Код подтверждения",
                Text = "Код: 4821. Никому не сообщайте. Для отмены перевода звоните 8-800-XXX-XX-XX.",
                Timestamp = now.AddHours(-7),
                IsFake = true,
                Explanation = "Фейк. Банк присылает код, но не просит перезванивать по неизвестным номерам. Мошенники так воруют коды и деньги."
            },
            new() {
                Id = 12, AppName = "Wildberries", Sender = "WB-Support", Title = "Ваш заказ задержан",
                Text = "Оплатите доставку 349 ₽ по ссылке ниже, иначе заказ будет отменён: wb-dostavka.pay",
                Timestamp = now.AddHours(-8),
                IsFake = true,
                Explanation = "Фейк. Wildberries никогда не требует доплату по сторонним ссылкам — оплата только в приложении."
            }
        };
    }

    public Notification? GetRandom() =>
        _items.Count == 0 ? null : _items[_rnd.Next(_items.Count)];

    public Notification? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);
}