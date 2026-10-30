# NihongoHub 开发约定

本文记录 NihongoHub 当前已经采用的项目结构、代码分层和数据处理约定，供后续开发与代码审查时参考。

本文以 `debug` 分支当前源码为依据。项目仍处于早期开发阶段；明确标记为“待完善”的内容不代表已经实现，也不应被误认为现有功能。

## 1. 项目基本原则

1. **API 负责 HTTP。** 接收请求、读取请求头和请求体、选择 HTTP 状态码、组织 HTTP 响应，属于 API 层。
2. **Services 负责业务。** 负责业务流程、输入业务校验、权限与资源归属检查，并协调多个 Repository。
3. **Data 负责数据库访问。** Repository 封装 LiteDB 的查询、插入和更新，不负责决定业务权限。
4. **Models 负责数据结构。** 区分数据库实体、配置对象、HTTP 请求 DTO 和 HTTP 响应 DTO。
5. **Init 负责基础设施初始化。** 配置、数据库、JWT 验证参数和 Web Host 在应用启动阶段初始化。
6. 不将数据库实体直接作为 API 响应返回，尤其不能将密码哈希等内部字段暴露给客户端。

依赖关系原则上为：

```text
API → Services → Data
       ↓
     Models

Program → Init → 基础设施
```

`Init` 和 `Logging` 是基础设施部分，不应成为业务规则的存放位置。

## 2. 当前目录结构

当前后端源码的主要结构如下：

```text
src/
├── Data/
│   ├── UserRepository.cs
│   ├── TannGoRepository.cs
│   └── TannGoCyouRepository.cs
├── Init/
│   ├── ConfigInit.cs
│   ├── DatabaseInit.cs
│   ├── JwtInit.cs
│   ├── Logginginit.cs
│   └── ServerInit.cs
├── Logging/
│   └── Log.cs
├── Models/
│   ├── Config/
│   │   └── AppConfig.cs
│   ├── Dtos/
│   │   ├── Requests/
│   │   └── Responses/
│   └── Entities/
│       ├── User.cs
│       └── Word.cs
├── Services/
│   ├── AuthService.cs
│   └── TokenService.cs
├── Program.cs
└── NihongoHub.csproj
```

当前仓库还没有独立的 `Api/` 目录、Web 前端目录和测试项目。未来新增 API 端点时，可按职责建立 API 层；不要把尚未创建的目录描述为当前已有功能。

### 目录职责

| 目录 | 职责 | 不应承担的职责 |
|---|---|---|
| `Data/` | LiteDB 查询、插入、更新等数据访问 | 登录流程、角色判断、资源授权 |
| `Init/` | 配置读取、数据库/JWT 初始化、Web Host 创建 | 用户业务规则 |
| `Logging/` | 统一日志分类、级别和输出 | 业务流程控制 |
| `Models/Entities/` | LiteDB 持久化实体及实体枚举 | HTTP 请求/响应格式 |
| `Models/Config/` | 应用配置结构 | 运行时初始化 |
| `Models/Dtos/Requests/` | API 请求数据结构 | 数据库查询 |
| `Models/Dtos/Responses/` | API 响应数据结构 | 业务处理 |
| `Services/` | 业务逻辑与服务间协调 | 直接解析 HTTP 请求头或组织 HTTP 响应 |

目前 `Models/Entities/User.cs` 同时定义了 `UserType` 和 `User`；`Models/Entities/Word.cs` 同时定义了 `TannGo` 和 `TannGoCyou`。这是当前文件组织，不要求为了拆分而立即重构。

## 3. 初始化约定

当前 `Program.Main()` 的初始化顺序为：

```csharp
Init.Logginginit.Init();
Init.ConfigInit.Load();
Init.DatabaseInit.Init();
Init.JwtInit.Init();
Init.ServerInit.Setup();
```

顺序有依赖关系，不应随意调整：

- 日志必须最先初始化，以便后续初始化失败时记录错误。
- 配置必须先于数据库和 JWT 初始化，因为它们需要读取配置。
- 数据库初始化依赖配置中的数据库文件名。
- JWT 验证参数初始化依赖配置中的密钥和 Issuer。
- Web Host 在基础设施初始化完成后启动。

### Init 的访问方式

初始化类使用静态入口提供初始化后的对象，例如：

- `ConfigInit.Current`
- `DatabaseInit.Current`
- `JwtInit.Current`

应先调用对应的 `Init()` / `Load()`，再访问 `Current`。未初始化时属于应用内部错误。

`LogReporter.Fatal()` 会记录 Fatal 日志、刷新日志并以非零退出码结束进程；调用后不应期待程序继续执行。

### 配置与文件路径

- 配置文件位置：`AppContext.BaseDirectory/Config/config.json`。
- 数据库文件位置：`AppContext.BaseDirectory/Config/{Database.FileName}`。
- `Database.FileName` 应填写文件名，例如 `nihongo.db`，不要再次包含 `Config/` 路径。
- 应用配置类目前使用 PascalCase 属性名，并由 `System.Text.Json` 读写。

## 4. Models 与 DTO 约定

### Entities

Entity 是数据库持久化对象。当前实体包括：

- `User`：用户 ID、用户名、密码哈希、启用状态和用户类型。
- `TannGo`：独立的用户词汇记录，包含所属 `userid`。
- `TannGoCyou`：单词本，包含所属 `userid`、单词 ID 列表和单词本标签。

数据关系约定：

- 单词是独立记录，具有自己的 ID。
- 单词本保存单词 ID，而不是嵌入完整的单词对象。
- 一个单词可以被多个单词本引用。
- `TannGo` 与 `TannGoCyou` 都有 `userid`，用于确定资源归属。
- 标签目前属于单词本，不属于单词本身。

Repository 取得资源后，Service 必须根据当前认证用户检查 `userid` 是否匹配。不能仅凭客户端提交的资源 ID 就允许读取或修改。

### Request / Response DTO

- 请求 DTO 放在 `Models/Dtos/Requests/`。
- 响应 DTO 放在 `Models/Dtos/Responses/`。
- 请求和响应模型分开定义，不共用 Entity 作为 API 契约。
- 必填字段使用 `required`；允许缺省或为空的字段使用 nullable 类型（如 `string?`）。
- 当前 DTO 属性使用小写名称，以保持现有 HTTP JSON 字段命名约定。
- 不在响应 DTO 中包含 `passwordhash)、签名密钥等敏感字段。

当前已有的请求 DTO 包括 `LoginRequest`、`SearchWordsRequest` 和 `WordBookWordsRequest`；响应 DTO 包括 `LoginResponse`、`SearchWordsResponse`、`WordBookSummaryResponse` 和 `WordResponse`。

`WordResponse.cs` 目前还包含 `WordBookReferenceResponse` 和 `WordBookWordsResponse`。相关 DTO 可以按功能分组，但文件名和内容应保持容易理解。

## 5. Data / Repository 约定

当前 Repository 使用 static class，通过 `DatabaseInit.Current` 访问数据库。

### Repository 应负责

- 执行 LiteDB 查询、插入和更新。
- 将数据库访问细节集中在 Data 层。
- 对预期的数据库异常进行记录，并返回调用方可处理的结果。
- 查询不到单条记录时使用 nullable 返回值；列表查询返回列表。

### Repository 不应负责

- 验证当前请求是否登录。
- 判断用户是否为管理员。
- 独立决定某个用户是否有权访问某条记录。
- 实现登录、密码验证或 Token 业务。

资源归属检查由 Service 负责。Repository 可以根据 Service 传入的用户 ID 执行带归属条件的查询，但不能自行信任客户端提供的用户 ID。

### 当前错误返回习惯

部分写入方法采用以下参数模式：

```csharp
bool Insert(Entity entity, ref string hint, ref LogCategory errorType)
bool Update(Entity entity, ref string hint, ref LogCategory errorType)
```

约定：

- 成功返回 `true`，失败返回 `false`。
- `hint` 在方法开始时清空；失败时写入可供上层理解的描述。
- `errorType` 仅在失败时有意义；调用方只应在返回 `false` 时读取它。
- Repository 负责记录底层异常；不要把异常堆栈或数据库内部细节直接作为客户端响应。
- 业务校验失败不应伪装成数据库异常。

`FindWordsByIds()` 目前通过 `errorIds` 返回未找到的 ID。调用方应明确区分“ID 不存在”和“用户无权访问该资源”。

## 6. Services 约定

Services 编排业务流程，并通过 Repository 访问数据库。需要检查用户权限、资源归属、业务条件时，应在 Service 中完成。

例如，单词本相关业务应使用已验证的当前用户 ID 检查单词本的 `userid`；单词相关业务也应检查 `TannGo.userid`。不要将 API 请求体中的 `userid` 当作可信身份来源。

Service 不应接收整个 `HttpRequest` 来处理 HTTP 细节。API 层应提取请求头、请求体等信息，再将必要的普通数据传给 Service。

### AuthService 与 TokenService

- `TokenService.GenerateToken()`：负责签发 JWT。
- `AuthService.ValidateToken()`：负责验证 JWT，并将已验证的 Claim 解析为项目自己的 `TokenUserData`。
- `TokenUserData` 是 Services 层内部传递用的数据封装，不是 LiteDB Entity，也不是 API Request/Response DTO；当前放在 `TokenService.cs` 中。
- API 层不应直接处理 `ClaimsPrincipal` 或解析 JWT Claim。

当前 `AuthService.ValidateToken()` 的返回习惯是布尔值加 `ref` 输出参数：`userData`、`hint` 和 `errorCode`。方法开始时清空输出参数；调用方根据返回值决定是否读取 `userData`。

Token 验证成功仅表示 JWT 的签名、签发者和有效期等检查通过，不代表数据库中的用户仍存在、仍启用或仍拥有 Token 内的角色。后续认证业务仍需根据用户 ID 查询数据库检查当前状态。

## 7. JWT 约定

### 当前签发格式

当前 Token 使用 HS256（HMAC-SHA256）签名，Claims 包括：

| Claim | 内容 |
|---|---|
| `sub` | 用户 ID 的字符串形式 |
| `unique_name` | 用户名 |
| `role` | 用户类型字符串（目前为 `admin` / `user`） |
| `jti` | 每次签发生成的 GUID |

`iss`、`nbf` 和 `exp` 由 JWT 构造参数设置。时间使用 UTC。默认 Token 有效期为 10080 分钟（7 天）。

JWT Payload 只是 Base64Url 编码，不是加密内容。不得将密码、密码哈希、签名密钥或其他秘密信息放进 Claims。

### 验证规则

`JwtInit` 在应用启动时根据配置创建并复用 `TokenValidationParameters`。初始化后应将它视为只读配置，不要在单次请求处理中修改其属性。

当前验证规则：

- 验证签名密钥。
- 验证 Issuer。
- 验证有效期及过期时间。
- 只接受 HS256。
- 时钟偏差为 30 秒。
- 当前没有设置 Audience，因此暂时关闭 Audience 验证。
- 验证时将 `JwtSecurityTokenHandler.MapInboundClaims` 设为 `false`，按原始 Claim 名称读取。

`MapInboundClaims` 是读取 Token 时的 Claim 映射选项；生成 Token 时调用 `WriteToken()` 不需要设置它。

### 权限与用户状态

- Token 中的 `role` 是签发时的角色快照，角色变更后旧 Token 不会自动更新。
- 需要最新角色的敏感操作应检查数据库中的当前角色。
- 用户禁用、删除或 Token 撤销机制尚未由当前 `ValidateToken()` 实现。
- 在面向公网部署前，应补充 Audience 配置及验证，并明确用户禁用、角色变更和 Token 撤销策略。

## 8. 命名与代码风格

当前代码主要采用以下风格：

- Namespace、类、方法和 DTO 类型使用 PascalCase。
- 局部变量和方法参数使用 camelCase。
- 私有字段使用下划线开头的 camelCase，例如 `_jwtConfig`。
- 当前 Entity 和 Request/Response DTO 的属性使用小写名称，以保持现有 LiteDB 字段和 JSON 字段形式；不要在没有评估数据迁移/API 兼容性的情况下随意改名。
- 配置模型属性当前使用 PascalCase。
- 当前 `UserType` 枚举成员为小写 `admin`、`user`；它们会被写入 Token，修改名称或数值时必须考虑 Token 与数据库兼容性。
- 方法名应表达动作，变量名应表达用途；避免无意义缩写。

当前 Entity 中的 `kannji`、`akusennto` 是已有字段拼写。它们并非理想的英文技术命名；若要改为 `kanji`、`pitchAccent` 等更清晰的名称，应在 API/数据库结构稳定前统一评估并安排迁移，不要只改其中一层。

## 9. 日志与错误处理

- 通过 `Logging.LogReporter` 统一记录日志。
- 日志分类和级别使用 `LogCategory`、`LogLevel`。
- `Fatal()` 用于无法继续运行的初始化或基础设施错误，会结束进程。
- Service 应返回项目定义的错误代码和描述；API 层负责将其转换为一致的 HTTP 状态码和 JSON 响应。
- 对外响应不应直接包含异常堆栈、数据库内部信息或敏感配置。

当前 Token 错误代码采用大写下划线格式，例如 `TOKEN_EXPIRED`、`TOKEN_INVALID_SIGNATURE` 和 `TOKEN_CLAIMS_INVALID`。

## 10. 分支与提交

- 日常开发目前在 `debug` 分支进行。
- 功能稳定并完成必要检查后，再合并到 `main`。
- Commit message 使用英文，采用 Conventional Commits 风格，例如 `feat(auth): implement JWT token validation`、`fix(data): handle missing word records` 和 `docs: update development conventions`。
- 修改代码结构或 API 约定时，应同步更新本文档及相关项目说明。

## 11. 当前待完善事项与审查提醒

以下内容是根据当前源码发现的待办事项，不代表已经实现：

1. **UserType 默认值风险：** 当前枚举声明顺序为 `admin`、`user`，因此默认枚举值（数值 0）是 `admin`。如果某段代码意外使用默认值，可能得到管理员角色。应在用户创建流程实现前调整为安全的默认值或明确的数值定义，并考虑 LiteDB 中已有枚举数据的迁移；不能只改枚举顺序而忽略旧数据。
2. **资源归属检查：** `FindWordById()`、`FindWordListById()` 等按 ID 查询的方法本身不会检查资源所有者。所有调用它们的业务 Service 都必须补上 `userid` 校验。
3. **Audience 尚未启用：** 当前 Token 不包含 `aud`，验证也未检查 Audience。面向正式部署前应增加。
4. **JWT Bearer 包当前未使用：** 项目引用了 `Microsoft.AspNetCore.Authentication.JwtBearer`，但当前代码采用手动 `JwtSecurityTokenHandler.ValidateToken()`，没有注册 `AddJwtBearer()` 或使用认证中间件。若继续采用手动验证方案，可评估移除该直接依赖；若未来改用 ASP.NET Core JWT Bearer，则应明确切换方案，避免两套认证流程混用。
5. **数据库释放：** `DatabaseService` 实现了 `IDisposable`，但当前 `DatabaseInit` 尚未在应用停止时显式释放它。应在完善 Host 生命周期管理时补上。
6. **默认网络配置：** 默认 Host 为 `0.0.0.0`、HTTPS 关闭。这会在所有网络接口上监听明文 HTTP。公网或不可信网络部署必须启用 HTTPS，或将服务限制在可信网络和受控入口后。
7. **拼写统一：** `kannji`、`akusennto` 等现有字段名称建议在数据/API 契约冻结前评估是否统一。
8. **测试项目尚不存在：** 当前仓库没有独立测试项目。后续至少应覆盖 Token 签发/验证、过期 Token、错误签名、Claim 缺失、用户禁用和资源归属检查。

本文档描述当前项目约定；当实际实现或团队决定发生变化时，应以新的代码审查结果为依据同步修订。
