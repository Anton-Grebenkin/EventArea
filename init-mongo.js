db = db.getSiblingDB('kudago');

// Создаем коллекцию messages
db.createCollection('messages');

// Заполняем коллекцию messages
db.messages.insertMany([
    {
        Text: 'Приветсвие',
        MessageTemplateType: 0
    },
    {
        Text: 'Выберите город',
        MessageTemplateType: 1
    }
]);

