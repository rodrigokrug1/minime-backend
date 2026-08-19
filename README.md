# MiniMe Backend

API do encurtador de URLs [minime.cloud](https://minime.cloud), hospedada como função AWS Lambda com armazenamento em DynamoDB.

## 🚀 Tecnologias

- .NET 10 (Minimal API)
- AWS Lambda (runtime `dotnet10`, via `Amazon.Lambda.AspNetCoreServer.Hosting`)
- Amazon DynamoDB (AWS SDK v4)
- Google reCAPTCHA v2 (validação server-side)
- OpenAPI nativo do ASP.NET Core (`/openapi/v1.json` em ambiente de desenvolvimento)

## 📁 Estrutura

```
Minime.Lambda/
├── Program.cs                     # Bootstrap: DI, Lambda hosting, CORS, OpenAPI
├── Endpoints/
│   └── UrlEndpoints.cs            # Rotas da API (Minimal API)
├── Models/
│   ├── MinimeUrl.cs               # Entidade da tabela MinimeUrls (DynamoDB)
│   └── NewLinkRequest.cs          # Payload do POST /link
└── Services/
    ├── DynamoDbUrlRepository.cs   # Persistência (IUrlRepository)
    └── RecaptchaService.cs        # Validação do token reCAPTCHA (IRecaptchaService)
```

## 🔗 Endpoints

| Método | Rota          | Descrição                                                        | Respostas |
|--------|---------------|------------------------------------------------------------------|-----------|
| POST   | `/link`       | Gera uma URL encurtada. Exige token reCAPTCHA no header `Authorization: Bearer {token}` | 201, 429 |
| GET    | `/{uid}`      | Redireciona para a URL original                                  | 302, 204 |
| GET    | `/url?uId=`   | Retorna a URL original como texto                                | 200 |

## ⚙️ Configuração

| Variável / chave          | Onde                | Descrição                          |
|---------------------------|---------------------|------------------------------------|
| `GOOGLE_RECAPTCHA_SECRET` | Variável de ambiente | Secret do reCAPTCHA (server-side) |
| `BaseUrl`                 | `appsettings.json`  | Domínio usado na URL encurtada     |
| `AWS`                     | `appsettings.json`  | Profile e região para o DynamoDB   |

A tabela do DynamoDB é `MinimeUrls`, com hash key `UId` (string).

## 🔧 Desenvolvimento

```bash
dotnet run --project Minime.Lambda
```

A API sobe em `http://localhost:5118`. Requisições de exemplo em [Minime.Lambda.http](Minime.Lambda/Minime.Lambda.http).

## 📦 Deploy

Deploy via [Amazon.Lambda.Tools](https://github.com/aws/aws-extensions-for-dotnet-cli):

```bash
dotnet lambda deploy-function
```

As configurações da função (runtime, memória, role, região) estão em [aws-lambda-tools-defaults.json](Minime.Lambda/aws-lambda-tools-defaults.json). O valor real de `GOOGLE_RECAPTCHA_SECRET` deve ser informado no deploy ou configurado no console da AWS — não é versionado neste repositório.

## 🖥️ Frontend

A interface web (Vue 3) está no repositório [minime-frontend-vue](https://github.com/rodrigokrug1/minime-frontend-vue).
