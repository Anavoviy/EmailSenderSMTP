# **EmailSender.SMTP**

### Introduction
This project is a Nuget-package. 
>It is needed in order to make it easier to work with the standard SmtpClient class in **.NET CORE 7/8.**

### Content:
- Introduction
- Content
- Supported MailServices
- SenderSMTP & its constructor
- Create Text/HTML MailMessage
- Send MailMessage
- Other Methods


___

### Supported MailServices
MailService - enum, which contains the value of SMTP-servers that are supported by this library.
Some SMTP-servers will be checked in the future, but have already been added for selection.
__MailServic enum values:__
| Value  | SMTP Server | Has it been checked? | 
|--------|------------|----------------------|
| MailRu | mail.ru    | checked              |
| GoogleCom |google.com| checked             |
| YandexRu | yandex.ru | not checked |
| PochtaRu | pochta.ru | not checked |
| RamblerRu| rambler.ru| not checked |
___

### SenderSMTP & its constructor
SenderSMTP is a class that configures the SmtpClient class itself when its constructor is called.NET CORE.

#### In the constructor, this class accepts:
- (required) __MailService__ - one of the MailService enum values __*(enum MailService)*__
- (required) __emailFrom__ - email address from which emails will be sent __*(string)*__
- (required) __pass__ - password for applications from the email address. from which emails will be sent __*(string)*__
- (optional) __nameAuthor__ -the name of the sender to be displayed in the email, it can be specified separately for each message __*(string)*__
- (optional) __emailsPerMinutes__ - number of emails that can be sent in 1 minute, it is recommended to leave the default value __*(int)*__

```C#
SenderSMTP(MailService mailService, string emailFrom, string pass, string nameAuthor = "", int emailsPerMinutes = 10)
```
___ 

### Create Text/HTML MailMessage
There are two methods for creating messages in the library:
```C#
//for messages with HTML Body
CreateMailMessageBodyIsHTML(string emailTo, string name = null, string subject = "", string bodyHTML = "") ;
```
```C#
//for messages with Text Body
CreateMailMessageBodyIsText(string emailTo, string name = null, string subject = "", string bodyText = "");
```
Both methods take the same arguments:
- (required) __emailTo__ - the email address of the recipient of the help (string)
- (optional) __name__ - the name of the sender, which will be displayed in the email message. (string)
- (optional) __subject__ - title email message (string)
- (optional) __bodyHtml/bodyText__ - The content of the email message (string)

Below is an example of creating messages with an HTML or Text body.
#### **Example:**

```C#
//configure SenderSMTP class
SenderSMTP sender = new SenderSMTP(MailService.GoogleCom, "example@gmail.com", "examplepass");

//create MailMessage with HTML Body
string bodyHTML = "<h1>hello, my friend</h1>";
MailMessage messageHTML = sender.CreateMailMessageBodyIsHTML("example@email.com", "Your friend", "Letter", bodyHTML); 

//create MailMessage with HTML Body
string bodyText = "hello, my friend";
MailMessage messageText = sender.CreateMailMessageBodyIsHTML("example@email.com", "Your friend", "Letter", bodyText); 
```
___ 

### Send MailMessage
Each method for sending messages has its asynchronous version, which differs only in that the postfix 'Async' is added to the end of its name.
It is quite simple to work with all the methods, so next there will be only examples with signatures.

```C#
//configure SenderSMTP class
SenderSMTP sender = new SenderSMTP(MailService.GoogleCom, "example@gmail.com", "examplepass");

//configure MailMessage
string bodyHTML = "<h1>hello, my friend</h1>";
MailMessage message = sender.CreateMailMessageBodyIsHTML("example@email.com", "Your friend", "Letter", bodyHTML); 

//send MailMessage
sender.SendMail(message);

//async variation
await sender.SendMailAsync(message);
```
___ 

### Other Methods
This library contains not only methods for sending a single message, but also methods for sending a single message to multiple addresses, or multiple messages.
#### __SendMailToAddresses()__
is a method for sending a single message to multiple addresses.
#### **Example:**

```C#
//configure SenderSMTP class
SenderSMTP sender = new SenderSMTP(MailService.GoogleCom, "example@gmail.com", "examplepass");

//configure email addresses
string[] addresses = new string[]{"example1@email.com", "example1@email.com", "example1@email.com"}

//configure MailMessage
string bodyHTML = "<h1>hello, my friend</h1>";
MailMessage message = sender.CreateMailMessageBodyIsHTML("", "Your friend", "Letter", bodyHTML); 

//send one MailMessage to multiple addresses
sender.SendMailToAddresses(message, addresses);

//async variation
await sender.SendMailToAddressesAsync(message, addresses);
```

#### __SendMailsToAddresses()__
is a method for sending a multiple messages to multiple addresses.
#### **Example:**

```C#
//configure SenderSMTP class
SenderSMTP sender = new SenderSMTP(MailService.GoogleCom, "example@gmail.com", "examplepass");

//configure IEnumerable<MailMessage>
string bodyHTML = "<h1>hello, my friend</h1>";

MailMessage message1 = sender.CreateMailMessageBodyIsHTML("example1@email.com", "Your friend", "Letter", bodyHTML); 
MailMessage message2 = sender.CreateMailMessageBodyIsHTML("example2@email.com", "Your friend", "Letter", bodyHTML); 
MailMessage message3 = sender.CreateMailMessageBodyIsHTML("example3@email.com", "Your friend", "Letter", bodyHTML); 

List<MailMessage> messages = new List<MailMessage>();
messages.Add(message1);
messages.Add(message2);
messages.Add(message3);


//send multiple MailMessage to multiple addresses
sender.SendMailsToAddresses(messages);

//async variation
await sender.SendMailsToAddressesAsync(messages);
```

