db = db.getSiblingDB('kudago');

// Создаем коллекцию messages
db.createCollection('messages');

// Заполняем коллекцию messages
db.messages.insertMany([
    {
        text: 'Приветсвие',
        messageTemplateType: 0
    },
    {
        text: 'Выберите город',
        messageTemplateType: 1
    }
]);

