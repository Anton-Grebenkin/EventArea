db = db.getSiblingDB('kudago');

// Создаем коллекцию messages
db.createCollection('messages');

// Заполняем коллекцию messages
db.messages.insertMany([
    {
        "Text": "Привет! Я ваш личный гид по событиям в вашем городе. Я создан, чтобы рекомендовать вам захватывающие мероприятия, концерты, фестивали и многое другое!",
        "MessageTemplateType": 0
    },
    {
        "Text": "Выберете город, о событиях которого вы хотите получать информацию",
        "MessageTemplateType": 1
    },
    {
        "Text": "Вы выбрали город",
        "MessageTemplateType": 2
    },
    {
        "Text": "Выбери категории",
        "MessageTemplateType": 3
    },
    {
        "Text": "Вы выбрали категории",
        "MessageTemplateType": 4
    }
]);

