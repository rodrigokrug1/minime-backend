# MiniMe Backend

API do encurtador de URLs [minime.cloud](https://minime.cloud), hospedada como função AWS Lambda com armazenamento em DynamoDB. Os links curtos são servidos em `link.minime.cloud`.

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

| Método | Rota      | Descrição                                                                               | Respostas |
|--------|-----------|-----------------------------------------------------------------------------------------|-----------|
| POST   | `/link`   | Gera uma URL encurtada. Exige token reCAPTCHA no header `Authorization: Bearer {token}` | 201, 429  |
| GET    | `/{uid}`  | Redireciona para a URL original                                                          | 302, 404  |

O `shortUrl` devolvido pelo `POST /link` é a URL absoluta e já pronta para uso — o cliente não monta o host.

## 🔑 Geração do UId

O `UId` tem 6 caracteres sorteados de um alfabeto fixo de 56 símbolos, sem os ambíguos (`0`/`O`, `1`/`l`/`I`), usando `RandomNumberGenerator`. A gravação usa `PutItem` com `attribute_not_exists(UId)` e refaz o sorteio em caso de colisão, até 5 tentativas — um `UId` nunca sobrescreve um link existente.

## ⚙️ Configuração

| Variável / chave          | Onde                 | Descrição                                               |
|---------------------------|----------------------|---------------------------------------------------------|
| `GOOGLE_RECAPTCHA_SECRET` | Variável de ambiente | Secret do reCAPTCHA (server-side)                       |
| `BaseUrl`                 | `appsettings.json`   | Origem completa dos links (`https://link.minime.cloud`) |
| `OrigensPermitidas`       | `appsettings.json`   | Origens aceitas pelo CORS, separadas por vírgula        |
| `AWS`                     | `appsettings.json`   | Profile e região para o DynamoDB                        |

A tabela do DynamoDB é `MinimeUrls`, com hash key `UId` (string).

O CORS não usa `AllowAnyOrigin`: com qualquer site podendo chamar a API, o reCAPTCHA seria a única defesa contra abuso do endpoint de criação.

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

A suíte de ferramentas (Astro + React) está em [minime-tools](../minime-tools).
