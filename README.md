# PayTR POS Seçim Servisi

E-ticaret ödeme anında, müşterinin kart bilgilerine göre en düşük maliyetli POS sağlayıcısını seçip işlemi o POS'a yönlendiren .NET Core Web API servisi.

## Özellikler

- ✅ Farklı POS sağlayıcıları ile entegrasyon desteği
- ✅ Mock POS servisten otomatik oran çekme ve önbellekleme
- ✅ Günlük otomatik oran güncelleme (23:59 Europe/Istanbul)
- ✅ En düşük maliyetli POS seçim algoritması
- ✅ SOLID prensiplerine uygun mimari
- ✅ Docker desteği
- ✅ Swagger/OpenAPI dokümantasyonu

## Kullanılan Teknolojiler

- **.NET 9.0** - Framework
- **ASP.NET Core Web API** - Web API framework
- **Newtonsoft.Json** - JSON serialization/deserialization
- **Swagger/OpenAPI** - API dokümantasyonu
- **Docker** - Containerization
- **Background Services** - Scheduled jobs

## Kurulum

### Gereksinimler

- .NET 9.0 SDK
- Docker (opsiyonel)

### Yerel Geliştirme

1. Projeyi klonlayın:
```bash
git clone <repository-url>
cd PayTr
```

2. Bağımlılıkları yükleyin:
```bash
dotnet restore
```

3. Projeyi çalıştırın:
```bash
dotnet run
```

API `https://localhost:7210` veya `http://localhost:5004` adresinde çalışacaktır.

Swagger UI'ya erişmek için: `https://localhost:7210/swagger/index.html`

## Docker ile Çalıştırma

### Docker Build ve Run

```bash
docker build -t pos-selection .
docker run -p 8080:8080 pos-selection
```

### Docker Compose

```bash
docker-compose up -d
```

API `http://localhost:8080` adresinde çalışacaktır.

## API Kullanımı

### Endpoint

**POST** `/api/Pos/v1/selectBest`

### Request Örneği

```json
{
  "amount": 362.22,
  "installment": 6,
  "currency": "TRY",
  "card_type": "credit",
  "card_brand": "bonus"
}
```

### Response Örneği

```json
{
  "filters": {
    "amount": 362.22,
    "installment": 6,
    "currency": "TRY",
    "card_type": "credit",
    "card_brand": "bonus"
  },
  "overall_min": {
    "pos_name": "KuveytTurk",
    "card_type": "credit",
    "card_brand": "saglam",
    "installment": 6,
    "currency": "TRY",
    "commission_rate": 0.0260,
    "price": 9.42,
    "payable_total": 371.64
  }
}
```

### Request Parametreleri

| Parametre | Tip | Zorunlu | Açıklama |
|-----------|-----|---------|----------|
| amount | decimal | ✅ | Ödeme tutarı |
| installment | int | ✅ | Taksit sayısı |
| currency | string | ✅ | Para birimi (TRY, USD, EUR, GBP) |
| card_type | string | ❌ | Kart tipi (credit, debit) |
| card_brand | string | ❌ | Kart markası (bonus, maximum, vb.) |

## Maliyet Hesaplama

### TRY Para Birimi
```
cost = max(amount * commission_rate, min_fee)
```

### USD Para Birimi
```
cost = max(amount * commission_rate * 1.01, min_fee)
```

### Yuvarlama
Tüm fiyatlar 2 ondalık basamak olarak **half-up** yöntemiyle yuvarlanır.

## POS Seçim Algoritması

En uygun POS seçimi aşağıdaki öncelik sırasına göre yapılır:

1. **En düşük maliyet (cost)**
2. **Daha yüksek priority değeri** (eşit maliyet durumunda)
3. **Daha düşük commission_rate** (eşit maliyet ve priority durumunda)
4. **Alfabetik pos_name** (son çare)

## Mimari

Proje SOLID prensiplerine uygun olarak geliştirilmiştir:

- **Models**: Veri modelleri (`Ratio`, `PosSelectionRequest`, `PosSelectionResponse`)
- **Services**: İş mantığı servisleri
  - `IRatioService` / `RatioService`: Mock POS servisten oran çekme ve önbellekleme
  - `IPosSelectionService` / `PosSelectionService`: POS seçim algoritması
  - `RatioRefreshBackgroundService`: Günlük otomatik oran güncelleme
- **Controllers**: API endpoint'leri

## Oran Güncelleme

Oranlar her gün **23:59 (Europe/Istanbul)** saatinde otomatik olarak güncellenir. Uygulama başlatıldığında da ilk yükleme yapılır.

Mock API Endpoint: `https://6899a45bfed141b96ba02e4f.mockapi.io/paytr/ratios`

## Test Senaryoları

Case dokümanında belirtilen test senaryoları:

1. **TRY, credit, 6 taksit** (amount = 362.22)
2. **USD, credit, 3 taksit, bonus** (amount = 395.00)
3. **TRY, credit, 3 taksit - min_fee etkisi** (amount = 60.00)
4. **TRY, credit, 12 taksit - tie-breaker (priority)** (amount = 100.00)

## Geliştirme Notları

- Proje .NET 9.0 ile geliştirilmiştir
- JSON serialization için **Newtonsoft.Json** kullanılmaktadır
- Tüm modellerde `JsonProperty` attribute'ları ile snake_case property isimleri kullanılmaktadır
- Tüm servisler dependency injection ile yönetilmektedir
- Logging için built-in ILogger kullanılmaktadır
- Hata yönetimi ve validasyonlar eklenmiştir
- Memory cache ile oranlar önbelleğe alınmaktadır


