db = db.getSiblingDB('kudago');

// ������� ��������� messages
db.createCollection('messages');

// ��������� ��������� messages
db.messages.insertMany([
    {
        text: '����������',
        messageTemplateType: 0
    },
    {
        text: '�������� �����',
        messageTemplateType: 1
    }
]);

