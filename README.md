# Sosyal Ağ Analizi ve Görselleştirme Projesi

Bu proje, sosyal ağ verilerini analiz etmek, görselleştirmek ve çeşitli grafik algoritmalarını (BFS, DFS, Dijkstra, A*) uygulamak amacıyla geliştirilmiştir. C# ve WPF teknolojileri kullanılmıştır.

## Özellikler

- **Veri Yükleme ve Kaydetme:**
  - CSV formatında düğüm ve kenar verilerini yükleme.
  - Mevcut grafiği tekrar CSV olarak dışa aktarma.
  
- **Görselleştirme:**
  - Yüklenen grafiğin basit çizimi (Düğümler ve bağlantılar).
  - *Not: Görselleştirme şu an için basit bir yerleşim algoritması kullanmaktadır.*

- **Algoritmalar:**
  1. **BFS (Breadth-First Search):** Genişlik öncelikli arama ile grafiği gezer.
  2. **DFS (Depth-First Search):** Derinlik öncelikli arama ile grafiği gezer.
  3. **Dijkstra:** Ağırlıklı graflarda iki düğüm arasındaki en kısa yolu bulur.
  4. **A* (A Star):** Sezgisel (heuristic) yöntem kullanarak en kısa yolu daha hızlı bulur.
  5. **Merkezilik (Centrality):** *(Geliştirme aşamasında)* Düğümlerin önem derecelerini hesaplar.

- **Ağırlık Hesaplama:**
  - Kenar ağırlıkları, düğümlerin etkinlik, etkileşim ve bağlantı sayısı özelliklerine göre dinamik olarak hesaplanır.

## Kurulum ve Çalıştırma

1. Projeyi klonlayın veya indirin.
2. Visual Studio veya uygun bir IDE ile `SocialNetworkAnalysis.sln` dosyasını açın.
3. Projeyi derleyin (Build).
4. `SocialNetworkAnalysis.UI` projesini başlangıç projesi olarak ayarlayıp çalıştırın.

## Kullanım

1. **Veri Yükleme:** Sol paneldeki "CSV Yükle" butonu ile uygun formatlı bir CSV dosyası seçin.
2. **Algoritma Seçimi:** Sağ panelden "Başlangıç" ve "Bitiş" düğümlerini seçin (veya ID'lerini girin).
3. **Çalıştırma:** İlgili algoritma butonuna (BFS, DFS, vb.) tıklayın.
4. **Sonuçlar:** Algoritma çıktıları ve bulunan yollar "Bilgi / Sonuçlar" panelinde gösterilecektir.

## CSV Formatı
Giriş dosyası aşağıdaki sütunları içermelidir:
`Id, Name, Activity, Interaction, ConnectionCount, Neighbors(opt)`

## Geliştiriciler
- **Ad Soyad:** [Adınız]
- **Ders:** Lab Proje II
