# **Game Design Document: *Thermal Disposal***

Ahmad Muqorrobin (5025231254)

1. ## **General Information**

   1. **Judul Game:** Thermal Disposal  
   2. **Genre:** First-Person Psychological Horror  
   3. **Platform:** PC (Windows)  
   4. **Target Audiens:** Penikmat Game Horror/Indie  
   5. **Mode Permainan:** Single Player  
   6. **Engine:** Unity  
2. **Game Overview**  
   Pemain berperan sebagai operator pembuangan sampah di sebuah fasilitas bawah tanah. Tugas sederhana namun repetitif, yakni memproses kantong limbah yang dikirim melalui *conveyor belt* untuk kemudian ditimbang dan akhirnya dibakar di tungku pembakaran raksasa (*furnace*). Pengalaman bermain yang awalnya terasa seperti simulator pekerjaan biasa perlahan berubah menjadi horor psikologis saat limbah-limbah yang datang mulai terlihat aneh serta tugas harian yang semakin aneh.  
3. **High Concept**  
   *Thermal Disposal* menggabungkan kebosanan kerja industrial dengan kengerian *cosmic horror*. Keunikan utama terletak pada “*Moral Complicity*”, dimana pemain tidak hanya menyaksikan horor tapi berperan secara aktif menjadi pelaku hanya demi memenuhi tugas harian. Estetika *low-poly* digunakan untuk menyamarkan detail terutama detail limbah, memaksa imajinasi pemain untuk membayangkan kengerian yang lebih besar.  
4. **Story & Narrative**  
   1. **Latar Belakang**  
      Fasilitas industri *brutalist* di masa depan distopia.  
   2. **Karakter**  
1. Protagonis: Pekerja kontrak tanpa nama.  
2. Antagonis: *The Company*, perusahaan misterius yang mempekerjakan sang protagonis.  
   3. **Alur Cerita**  
1. Awal: Pengenalan pekerjaan yang sederhana dan membosankan.  
2. Munculnya anomali, menandakan bahwa ini bukan pekerjaan membosankan biasa.  
3. *Company* sudah tidak lagi membutuhkan sang protagonis, dengan memberikan tugas akhir agar sang protagonis membakar dirinya sendiri di dalam tungku seperti limbah lainnya.  
5. **Gameplay**  
   1. **Core Gameplay Loop**  
      Day start → Read task from Paper & Clipboard → Receive Trash Bag from Conveyor Belt → Weight the Trash Bag on the Scale → Log task on the Paper & Clipboard → Incinerate the Trash Bag in the Furnace → Log task on the Paper & Clipboard → Repeat until all tasks are completed → Next day  
   2. **Mekanika Game**  
1. Movement: *First-person* *walk* dengan WASD*, look, jump,* dan *crouch*.  
2. Interaksi Objek: Sistem *Pick Up* dan *Put Down* dengan *Raycast* dan interaksi dengan objek dengan *left click* dan tombol huruf “E”.  
3. Interactive Stations:  
   1. Conveyor Belt: Berfungsi memindahkan kantong limbah dari tempat *spawn* menuju ke tengah ruangan.  
   2. Weight Scale: Berfungsi untuk menimbang objek seperti kantong limbah, item lainnya, serta player.  
   3. Furnace: Berfungsi menghapus objek terutama kantong limbah, namun juga bisa menghapus item lainnya.  
   4. Exit Door: Berfungsi untuk melanjutkan progress menuju hari selanjutnya.  
   3. **Tujuan Game**  
      Melakukan kontrak kerja selama 7 hari dan memenuhi tugas di setiap harinya.  
6. **World Design**  
   1. **Dunia Game**  
      Ruang pembakaran sampah yang sempit, berkarat, dan remang-remang.  
   2. **Struktur Level**  
      Satu ruangan utama.  
   3. **Progression**

| Day | Task | Description |
| ----- | ----- | ----- |
| 1 | Bakar 10 kantong limbah. | Semua kantong limbah terasa berat dan beberapa terdengar “basah”. |
| 2 | Bakar 10 kantong limbah yang setelah menggunakan *scanner* untuk mengecek kontaminasi besi. | *Scanner* memberikan hasil “*no metal detected*” untuk semua kantong limbah, namun kantong ke-9 sedikit bergerak dan memberikan hasil “*organic matter detected*”. |
| 3 | Bakar 10 kantong limbah dengan conveyor yang sedang rusak, sehingga pemain harus mengambil kantong limbah yang datang dari pojok ruangan secara manual satu per satu. | Terdapat suara-suara aneh seperti suara monster dan suara jeritan manusia yang terdengar dari tempat kantong limbah datang yang hanya dapat didengar ketika pemain sangat dekat. |
| 4 | Bakar 10 kantong limbah dengan catatan dari *Company* untuk menghiraukan semua suara aneh. | 3 kantong limbah sedikit bergerak. *Scanner* yang masih ada memberikan hasil “*organic matter detected*” pada ketiganya. Ketiga kantong limbah ini mengeluarkan suara jeritan ketika dibakar, namun “*Company*” memberikan catatan “*Ignore acoustics, they are just trapped gases escaping.*” pada kertas tugas. |
| 5 | Bakar 10 kantong limbah, 5 diantaranya memberikan hasil “OVER\_CAPACITY” pada timbangan, mengharuskan pemain membuka 5 kantong tersebut untuk mengeluarkan sampah berlebih dan memindahkan semuanya ke 1 kantong limbah baru. | 5 kantong limbah “OVER\_CAPACITY” berisikan semacam limbah daging mentah ketika dibuka, dan tercampur di dalamnya barang-barang rumah tangga seperti sepatu, sikat gigi, remot tv, dan lain-lain yang harus dipindah ke kantong limbah baru. |
| 6 | Bakar 10 kantong limbah dan bersihkan tungku (*furnace*) dengan *iron poker* setiap membakar 1 kantong limbah. | Pembakaran tungku hari ini tidak sempurna, menyisakan beberapa sisa sampah yang harus didorong menggunakan *iron poker* agar kemudian dapat terbakar. Sisa-sisa sampah ini beragam mulai dari barang rumah tangga hingga tangan dan kaki manusia. |
| 7 | Sampah terakhir, karakter pemain itu sendiri. | Tugas hari ini hanya 1 kantong limbah, namun *conveyor* tidak berjalan. Kontrak kerja hanya akan selesai jika pemain menyelesaikan semua tugas di 7 hari kerja. Satu-satunya di ruangan ini yang memiliki berat “OPTIMAL” pada timbangan adalah pemain itu sendiri. |

7. **Characters & Enemies**  
   1. **Player:** Tidak memiliki kemampuan bertarung. Hanya bisa bergerak, memindahkan objek, dan berinteraksi dengan objek.  
8. **Game System**  
   1. **Progression System:** Berbasis hari (*Day-by-day system)*, dimana setiap hari baru membuka narasi dan mekanik baru. Pemain perlu menyelesaikan semua tugas pada satu hari sebelum melanjutkan ke hari selanjutnya, dimana melewati satu tugas pun akan mendapat konsekuensi *game over*.  
   2. **Inventory System:** *Physical Inventory*, pemain hanya bisa memegang satu *item*/alat (*interactable object*) dalam satu waktu. *Item*/alat tersebut adalah:  
1. Trash Bag: Kantong limbah yang berperan sebagai *item* utama. Memiliki beberapa varian dan mekanik yang berbeda-beda.  
2. Paper & Clipboard: Kertas yang menginformasi pemain terkait tugas pada hari tersebut sekaligus berperan sebagai wadah pemain mencatat progress tugas.  
3. Scanner: Objek yang berfungsi untuk memindai kandungan limbah di dalam kantong limbah. Tersedia pada hari ke-2 dan seterusnya.  
4. Iron Poker: Tongkat besi yang berfungsi untuk mendorong sisa-sisa sampah dalam *furnace* agar terbakar sempurna. Tersedia pada hari ke-6 dan seterusnya.  
   3. **Quest System:** Berupa tugas harian di *paper & clipboard*. Pemain perlu membaca tugas dari *item* tersebut, dan mencatat progress di *item* yang sama.  
9. **UI/UX Design**  
   1. **Interface:** *Diegetic UI*, tampilan UI yang menyatu dengan dunia *game*. Tugas harian terlihat di objek *paper & clipboard*, bacaan timbangan terlihat langsung pada *weight scale*, hasil *scanner* terlihat pada *item* *scanner* itu sendiri, bukan pada HUD layar.  
   2. **User Experience:** Navigasi intuitif dengan *crosshair* yang hanya terlihat ketika pemain melihat *interactable object* dengan *hint* yang terkait dengan objek tersebut seperti “\[E\] Pick Up”.  
10. **Visual Design**  
    1. **Gaya Artistik:** *Stylized Low-Poly Aesthetic*.  
    2. **Referensi Visual:** *Game Lethal Company*, *Iron Lung*, *Squirrel Stapler*, dan karya David Szymanski lainnya.  
    3. **Desain Audio:** *Heavy industrial ambience*.  
11. **Technology**  
    1. **Alat & Perangkat Lunak**  
1. Unity 6.3 LTS  
2. Microsoft Visual Studio 2022  
3. Figma  
   2. **Spesifikasi Minimum**  
1. **OS:** Windows 10 64-bit  
2. **Processor:** Intel Core i3 (or any equivalent Dual-Core processor)  
3. **Memory:** 4 GB RAM  
4. **Graphics:** Intel HD Graphics 4000 or better (Integrated graphics)  
5. **Storage:** \<1000 MB of available space  
12. **Unique Selling Point**  
1. Simulasi pekerjaan industrial yang berubah menjadi horor psikologis.  
2. Estetika retro *low-poly* yang mendorong pemain untuk mengisi sisi horor dengan imajinasi.  
3. Narasi gelap tentang eksploitasi kelas pekerja.