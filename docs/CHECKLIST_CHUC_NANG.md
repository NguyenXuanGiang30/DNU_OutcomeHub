# BẢNG CHECKLIST 121 YÊU CẦU CHỨC NĂNG HỆ THỐNG OUTCOMEHUB (THEO MỤC 8 BRD)

> **Căn cứ tài liệu:** [BRD_OutcomeHub_He_thong_do_luong_chuan_dau_ra_OBE.md](BRD_OutcomeHub_He_thong_do_luong_chuan_dau_ra_OBE.md) — Mục 8: Yêu cầu chức năng.
> 
> **Tổng số yêu cầu:** 121 (105 Must, 16 Should).
> 
> **Quy ước trạng thái:** `[x]` Đã hoàn thành | `[-]` Đang thực hiện | `[ ]` Chưa thực hiện
> 
> **Ưu tiên MoSCoW:** `[M]` Must (Bắt buộc cho MVP) | `[S]` Should (Nâng cao sau khi nền tảng ổn định).

---

## 📊 TIẾN ĐỘ TỔNG HỢP

| STT | Phân hệ (Mục BRD) | Mã | Số lượng FR | Hoàn thành | Tỷ lệ |
| :---: | :--- | :---: | :---: | :---: | :---: |
| 1 | **Dashboard và điều hướng** (Mục 8.1) | `FR-DSH` | 5 | 0/5 | 0% |
| 2 | **Cơ cấu, CTĐT và chuẩn đầu ra** (Mục 8.2) | `FR-CTD` | 25 | 8/25 | 32% |
| 3 | **Sinh viên, giảng viên và phân công** (Mục 8.3) | `FR-PEO` | 6 | 6/6 | 100% |
| 4 | **Đề cương, đề thi/bài đánh giá và rubric** (Mục 8.4) | `FR-PRT` | 21 | 6/21 | 28.6% |
| 5 | **Đợt đo lường, nhập điểm và tính kết quả** (Mục 8.5) | `FR-MEA` | 20 | 12/20 | 60% |
| 6 | **Kết quả, phân tích và xuất báo cáo** (Mục 8.6) | `FR-RES` | 14 | 1/14 | 7.1% |
| 7 | **Cải tiến chất lượng (CQI)** (Mục 8.7) | `FR-CQI` | 6 | 0/6 | 0% |
| 8 | **Chatbot và AI** (Mục 8.8) | `FR-AI` | 8 | 0/8 | 0% |
| 9 | **Người dùng, quyền và cấu hình** (Mục 8.9) | `FR-ADM` | 8 | 0/8 | 0% |
| 10 | **Tích hợp và API** (Mục 8.10) | `FR-INT` | 8 | 1/8 | 12.5% |
| | **TỔNG CỘNG** | | **121** | **34 / 121** | **28.1%** |

---

## 8.1. Dashboard và điều hướng (5 Yêu cầu)
- [ ] `[M]` **FR-DSH-01:** Dashboard cá nhân hóa theo vai trò/scope, hiển thị tổng SV, đợt đo, học phần, PLO đạt và kế hoạch CQI.
- [ ] `[M]` **FR-DSH-02:** Bộ lọc nhất quán: Khoa, CTĐT, niên khóa, năm học, học kỳ, đợt đo; giữ trạng thái khi drill-down.
- [ ] `[M]` **FR-DSH-03:** Mọi widget hiển thị thời điểm dữ liệu, batch, nguồn, phiên bản CTĐT và trạng thái công bố.
- [ ] `[M]` **FR-DSH-04:** Từ PLO/CQI drill-down được tới PI, CLO, học phần, lớp, sinh viên và minh chứng theo quyền.
- [ ] `[M]` **FR-DSH-05:** Dữ liệu mẫu/demo phải có nhãn rõ, tách khỏi dữ liệu chính thức và không xuất trong báo cáo chính thức.

---

## 8.2. Cơ cấu, CTĐT và chuẩn đầu ra (25 Yêu cầu)
- [x] `[M]` **FR-CTD-01:** Quản lý cây Trường/Khoa/Viện/Bộ môn/cơ sở, mã, loại, hiệu lực và trạng thái.
- [x] `[M]` **FR-CTD-02:** Quản lý ngành/CTĐT, trình độ, hình thức, đơn vị chủ quản và mã định danh duy nhất.
- [x] `[M]` **FR-CTD-03:** Tạo nhiều ProgramVersion; lưu quyết định, ngày ban hành/hiệu lực, khóa áp dụng, tổng tín chỉ và trạng thái.
- [x] `[M]` **FR-CTD-04:** Quản lý cohort và gắn StudentPath của từng sinh viên với đúng ProgramVersion theo thời gian.
- [x] `[M]` **FR-CTD-05:** Quản lý học phần, phiên bản, tín chỉ, tiên quyết, tương đương/thay thế, bắt buộc/tự chọn/định hướng.
- [x] `[M]` **FR-CTD-06:** Kế thừa PLO1–PLO4 chung từ khung cấp Trường ở trạng thái khóa; quản lý PLO5–PLO9 ngành, miền năng lực/Bloom, mô tả và căn cứ phê duyệt.
- [x] `[M]` **FR-CTD-07:** Kế thừa PI chung của PLO1–PLO4 ở trạng thái khóa; quản lý PI ngành của PLO5–PLO9, mặc định theo cấu trúc khung và cho điều chỉnh số PI ngành khi được phê duyệt.
- [x] `[M]` **FR-CTD-08:** Quản lý CLO/LLO có phiên bản theo học phần, CTĐT, khóa và thời gian hiệu lực.
- [x] `[M]` **FR-CTD-09:** Tạo ma trận CLO–PI/PLO và học phần–PI/PLO có căn cứ; lưu riêng mức I/R/M và cờ A; hiển thị A/RA/MA, cảnh báo IA và không dùng assessmentCode làm cờ A.
- [ ] `[M]` **FR-CTD-10:** Phân tích độ phủ theo từng StudentPath: PLO/PI thiếu học phần/CLO, PI không có học phần A, thiếu mức M, chồng chéo và đường tự chọn thiếu phủ.
- [ ] `[S]` **FR-CTD-11:** Hiển thị lộ trình phát triển CĐR theo học kỳ/khóa/định hướng và so sánh các đường học.
- [ ] `[M]` **FR-CTD-12:** Workflow Nháp–Thẩm định–Đã duyệt–Áp dụng–Hết hiệu lực; giữ ý kiến, biên bản và người phê duyệt.
- [ ] `[M]` **FR-CTD-13:** Nhập/xuất Excel/CSV có template, preview, kiểm tra lỗi và không ghi đè cấu hình đã duyệt.
- [ ] `[S]` **FR-CTD-14:** So sánh hai ProgramVersion, chỉ ra thêm/bỏ/sửa và quản lý crosswalk PLO/PI/học phần.
- [ ] `[M]` **FR-CTD-15:** Quản lý DirectMeasurementPlan theo PI/StudentPath: ưu tiên 1, tối đa 2 nguồn A, học kỳ và owner, trọng số nguồn tổng 100%, nguồn chính thức/đối sánh, neo, version/phê duyệt.
- [ ] `[M]` **FR-CTD-16:** Kiểm tra mỗi PI có nguồn A trên mọi lộ trình thực tế, số nguồn không vượt policy, giới hạn M/A theo loại học phần và workflow ngoại lệ có thẩm quyền.
- [ ] `[M]` **FR-CTD-17:** Quản lý học phần dùng chung bằng CourseVersion/phần lõi dùng chung và mapping do Trường quản trị; đơn vị không được tự sửa, phụ lục khác biệt phải được duyệt.
- [ ] `[M]` **FR-CTD-18:** Quản lý `InstitutionTemplateVersion`: biểu mẫu Bản mô tả CTĐT/ĐCCT, trường bắt buộc, nội dung khóa/mở, quyết định, hiệu lực và trạng thái.
- [ ] `[M]` **FR-CTD-19:** Tạo ProgramVersion mới từ khung đang hiệu lực hoặc sao chép phiên bản trước; ghi nguồn kế thừa và không liên kết sửa đè dữ liệu cũ.
- [ ] `[M]` **FR-CTD-20:** Quản lý PO, Khung năng lực Tầng 1–3 và ma trận PO–PLO–năng lực L/M/H có kiểm tra độ phủ.
- [ ] `[M]` **FR-CTD-21:** Quản lý đầy đủ cấu trúc CTĐT: thông tin tổng quát, đối sánh, khối kiến thức, học phần, tín chỉ, tiên quyết, học kỳ và tổng tín chỉ.
- [ ] `[M]` **FR-CTD-22:** Quản lý CurriculumPath cho hướng chuyên ngành, nhóm tự chọn và từng phương án tốt nghiệp; kiểm tra cơ hội học và đo PI tương đương.
- [ ] `[S]` **FR-CTD-23:** So sánh ProgramVersion với phiên bản khung mới, hiển thị tác động và tạo đề nghị nâng cấp; không tự cập nhật phiên bản đã ban hành.
- [ ] `[M]` **FR-CTD-24:** Sinh/nhập/xuất Bản mô tả CTĐT đúng biểu mẫu từ dữ liệu cấu trúc; giữ số quyết định, khóa áp dụng, phiên bản và checksum.
- [ ] `[M]` **FR-CTD-25:** Chỉ cho ban hành ProgramVersion khi hoàn thành checklist: PLO/PI, chương trình học, mọi StudentPath, ma trận, nguồn A, trọng số và chủ thể phụ trách.

---

## 8.3. Sinh viên, giảng viên và phân công (6 Yêu cầu)
- [x] `[M]` **FR-PEO-01:** Đồng bộ/quản lý hồ sơ sinh viên, mã SV, lớp, khóa, CTĐT, trạng thái học và lịch sử thay đổi.
- [x] `[M]` **FR-PEO-02:** Tìm/lọc sinh viên theo Khoa–Ngành–Lớp–Khóa; báo cáo phân bổ và dữ liệu thiếu/trùng.
- [x] `[M]` **FR-PEO-03:** Đồng bộ/quản lý giảng viên, mã tài khoản, đơn vị, trạng thái và hồ sơ liên hệ tối thiểu.
- [x] `[M]` **FR-PEO-04:** Phân công giảng dạy, chấm, kiểm tra và phê duyệt theo lớp/học phần/đợt/tiêu chí.
- [x] `[M]` **FR-PEO-05:** Che trường dữ liệu nhạy cảm theo vai trò; báo cáo tổng hợp áp dụng ngưỡng nhóm tối thiểu.
- [x] `[M]` **FR-PEO-06:** Giữ lịch sử chuyển ngành, tạm dừng, học lại, công nhận và thay đổi lớp; không xóa cứng dữ liệu đã dùng đo.

---

## 8.4. Đề cương, đề thi/bài đánh giá và rubric (21 Yêu cầu)
- [x] `[M]` **FR-PRT-01:** Tạo, nhập, tìm, lọc và quản lý SyllabusVersion theo Khoa–ProgramVersion–Khóa áp dụng–CourseVersion; bắt buộc ghi Bản mô tả CTĐT đối chiếu.
- [x] `[M]` **FR-PRT-02:** Biểu diễn ĐCCT dạng cấu trúc: thông tin học phần, mục tiêu, CLO/LLO, học liệu, kế hoạch buổi học, assessmentCode, rubric, bảng 8.3.1/8.3.2, điều kiện và CQI.
- [ ] `[M]` **FR-PRT-03:** Quản lý đề thi/bài tập/dự án/trắc nghiệm/thực hành theo phiên bản và loại đánh giá.
- [x] `[M]` **FR-PRT-04:** Rubric builder theo từng AssessmentItem; hỗ trợ mã tiêu chí, mô tả mức, thang điểm, trọng số trong bài, CLO, vai trò dữ liệu, PI trực tiếp, cờ cốt lõi và quy đổi.
- [ ] `[M]` **FR-PRT-05:** Mapping trực tiếp ở mức bài/phần/câu hỏi/tiêu chí/sản phẩm; bảng 8.3.2 khai báo tỷ trọng trực tiếp từng tiêu chí trong PI và kiểm tra tổng đúng 100%.
- [x] `[M]` **FR-PRT-06:** Hỗ trợ template đánh giá 2/3 tín chỉ và cấu trúc linh hoạt; tổng trọng số học phần mặc định 100%.
- [ ] `[M]` **FR-PRT-07:** Tải PDF/Word/Excel/PowerPoint theo loại; quét mã độc, giới hạn dung lượng, checksum và metadata.
- [ ] `[S]` **FR-PRT-08:** Preview, tải xuống, lịch sử version, so sánh và khôi phục phiên bản theo quyền.
- [ ] `[S]` **FR-PRT-09:** AI tạo nội dung nháp cho đề cương/đề thi/rubric, có nguồn, prompt version và trạng thái duyệt.
- [ ] `[M]` **FR-PRT-10:** Workflow thẩm định/phê duyệt tài liệu; tài liệu đã dùng đo không được sửa tại chỗ.
- [ ] `[M]` **FR-PRT-11:** Gắn minh chứng gốc, phiếu chấm, đáp án/thang điểm và file kết quả với đối tượng học thuật.
- [ ] `[S]` **FR-PRT-12:** Xuất gói portfolio theo học phần/đợt/CTĐT có mục lục, phiên bản, checksum và watermark.
- [x] `[M]` **FR-PRT-13:** Lưu riêng assessmentCode A1/A2/A3, contributionLevel I/R/M và cờ isDirectAssessment; UI/API không dùng chung trường hoặc nhãn gây nhầm.
- [x] `[M]` **FR-PRT-14:** Xuất dữ liệu đo theo sinh viên–lớp học phần–học phần A–bài đánh giá–tiêu chí rubric–PI–tỷ trọng trực tiếp–minh chứng, kèm mọi phiên bản.
- [ ] `[M]` **FR-PRT-15:** Tạo SyllabusVersion từ `SyllabusTemplateVersion` và dữ liệu đã duyệt của ProgramVersion/CourseVersion; tự điền trường kế thừa nhưng không tự suy diễn nội dung.
- [ ] `[M]` **FR-PRT-16:** Kiểm tra PI liên kết, PI trực tiếp, mức I/R/M/A và vai trò học phần phải khớp ma trận/kế hoạch đo của ProgramVersion; sai khác bị chặn hoặc qua phụ lục duyệt.
- [ ] `[M]` **FR-PRT-17:** Bảng 8.3.1 truy vết toàn bộ CLO–PI–AssessmentItem–Criterion–Evidence và phân biệt “đo trực tiếp”, “hỗ trợ” và “chỉ đánh giá CLO”.
- [ ] `[M]` **FR-PRT-18:** Bảng 8.3.2 chỉ xuất hiện cho PI được giao A; chỉ chứa tiêu chí direct và tổng tỷ trọng từng PI bằng 100%; học phần không A không được xuất PI/PLO.
- [ ] `[M]` **FR-PRT-19:** Nếu một criterion gắn nhiều PI, yêu cầu tách criterion để chấm/truy vết riêng; ngoại lệ cần policy và phê duyệt, không ngầm sao chép điểm.
- [ ] `[M]` **FR-PRT-20:** Quản lý phiên bản nội dung giảng dạy theo buổi: LLO, CLO liên kết, số tiết, học liệu, phương pháp, đánh giá/minh chứng và nhiệm vụ tự học.
- [ ] `[M]` **FR-PRT-21:** Chỉ ban hành ĐCCT khi tổng trọng số bài đánh giá=100%, mỗi CLO có đánh giá phù hợp, rubric đầy đủ và các bảng truy vết/đo trực tiếp hợp lệ.

---

## 8.5. Đợt đo lường, nhập điểm và tính kết quả (20 Yêu cầu)
- [x] `[M]` **FR-MEA-01:** Tạo MeasurementPeriod với mã/tên, năm học, học kỳ, Khoa, CTĐT, niên khóa và mô tả phạm vi.
- [x] `[M]` **FR-MEA-02:** Khai báo riêng $\theta_{ind}$ và $\theta_{coh}$ ở cấp đợt/CLO/PI/PLO; hỗ trợ override có lý do và phê duyệt.
- [x] `[M]` **FR-MEA-03:** Xác định quần thể đo, điều kiện đưa vào/loại trừ, cỡ mẫu tối thiểu và chính sách học lại.
- [x] `[M]` **FR-MEA-04:** Tự chọn đúng nguồn A thuộc StudentPath thực tế từ DirectMeasurementPlan; không cho người chạy đợt tự thêm nguồn ngoài kế hoạch đã duyệt.
- [x] `[M]` **FR-MEA-05:** Đóng băng InstitutionTemplateVersion, ProgramVersion, SyllabusVersion, rubric/bảng 8.3.2, DirectMeasurementPlan/AWeight, neo và CalculationPolicy khi mở thu thập.
- [x] `[M]` **FR-MEA-06:** Phân công người chấm/kiểm tra/duyệt; theo dõi tiến độ theo học phần và hạn.
- [x] `[M]` **FR-MEA-07:** Nhập hoặc đồng bộ Enrollment/CLO theo API/CSV; preview và báo cáo bản ghi lỗi.
- [x] `[M]` **FR-MEA-08:** Nhập điểm tới mức tiêu chí/câu hỏi; lưu điểm gốc, thang tối đa, người nhập và thời điểm.
- [x] `[M]` **FR-MEA-09:** Nhập hàng loạt Excel/CSV và API; idempotency, checksum, delta, retry có kiểm soát.
- [x] `[M]` **FR-MEA-10:** Đối soát mã SV/lớp/học phần, StudentPath, thang điểm, tỷ trọng criterion PI=100%, trọng số nguồn=100%, tối đa 2 nguồn, rubric tương đương và mapping đã duyệt.
- [x] `[M]` **FR-MEA-11:** Điểm nhóm chỉ dùng kết luận cá nhân khi có thành phần cá nhân hoặc quy tắc phân bổ được duyệt.
- [x] `[M]` **FR-MEA-12:** Xử lý vắng/rút/hoãn/học lại/cải thiện/chuyển ngành/công nhận theo CalculationPolicy.
- [x] `[M]` **FR-MEA-13:** Tạo InputSnapshot bất biến trước khi tính; lưu checksum và liên kết về điểm nguồn.
- [x] `[M]` **FR-MEA-14:** Chạy CalculationBatch nền, có tiến độ, log, test vector và khả năng hủy an toàn trước công bố.
- [x] `[M]` **FR-MEA-15:** Lưu ResultBatch có phiên bản; cùng snapshot + policy cho kết quả tái lập.
- [x] `[M]` **FR-MEA-16:** Theo dõi trạng thái từng học phần: chưa phân công/đang nhập/đủ dữ liệu/đã chốt/đã duyệt.
- [x] `[M]` **FR-MEA-17:** Mở lại bắt buộc lý do và phê duyệt; giữ kết quả cũ, tạo delta và lần tính mới.
- [x] `[S]` **FR-MEA-18:** Thu thập khảo sát/đánh giá gián tiếp, chuẩn hóa thang và báo cáo tách khỏi direct.
- [x] `[M]` **FR-MEA-19:** Engine tính hai tầng: PI trong từng học phần A theo tỷ trọng bảng 8.3.2, rồi PI chung theo trọng số nguồn của StudentPath; lưu từng đóng góp và không tự suy tỷ trọng.
- [x] `[M]` **FR-MEA-20:** Áp dụng cổng không bù trừ cho tiêu chí rubric cốt lõi và PI cốt lõi khi kết luận PI/PLO; báo cáo rõ nguyên nhân không đạt.

---

## 8.6. Kết quả, phân tích và xuất báo cáo (14 Yêu cầu)
- [x] `[M]` **FR-RES-01:** Danh sách đợt đo với thời gian, scope, mục tiêu, trạng thái, tiến độ và quyền xem.
- [x] `[M]` **FR-RES-02:** Dashboard chương trình theo Khoa–CTĐT–Khóa–Đợt, hiển thị tiến độ PI/PLO.
- [x] `[M]` **FR-RES-03:** Báo cáo học phần: lượt SV–CLO, đạt/chưa đạt, tỷ lệ, minh chứng và người phụ trách.
- [x] `[M]` **FR-RES-04:** Báo cáo PLO: nội dung, PI con, lượt đạt/tổng, ngưỡng, tỷ lệ, trạng thái và CQI.
- [x] `[M]` **FR-RES-05:** Báo cáo PI: PLO cha, StudentPath, học phần A, điểm PI từng học phần, trọng số A/đóng góp, neo, nguồn CLO/tiêu chí, ngưỡng và biện pháp.
- [x] `[M]` **FR-RES-06:** Báo cáo CLO: học phần, miền/Bloom, điểm, lượt đạt/tổng, tỷ lệ và drill-down rubric.
- [x] `[M]` **FR-RES-07:** Báo cáo sinh viên: tiến độ CLO/PI/PLO, dữ liệu thiếu và cảnh báo; chỉ theo đúng scope.
- [x] `[M]` **FR-RES-08:** Tổng hợp theo lớp, học kỳ, khóa, CTĐT, Khoa và Trường; hỗ trợ so sánh các nhóm hợp lệ.
- [x] `[M]` **FR-RES-09:** Hiển thị direct/indirect riêng; kết quả kết hợp phải chỉ rõ $\alpha$ và policy.
- [x] `[M]` **FR-RES-10:** Mọi tỷ lệ hiển thị tử số, mẫu số, số loại trừ/thiếu, cỡ mẫu, thời điểm và batch.
- [ ] `[S]` **FR-RES-11:** So sánh kỳ/khóa có cảnh báo khác công thức, ngưỡng, quần thể, mapping hoặc nguồn minh chứng.
- [ ] `[S]` **FR-RES-12:** Cảnh báo sớm theo PLO/PI/CLO/SV: đỏ/vàng, lý do, mức thiếu mục tiêu và hành động.
- [ ] `[M]` **FR-RES-13:** Xuất Excel/PDF/Word và gói kiểm định; áp dụng phân quyền, watermark, checksum và audit.
- [ ] `[M]` **FR-RES-14:** Báo cáo tuân thủ ma trận I/R/M/A/RA/MA: nguồn A theo PI–StudentPath, tỷ trọng criterion/nguồn, IA legacy, ngoại lệ và trạng thái phê duyệt.

---

## 8.7. Cải tiến chất lượng (CQI) (6 Yêu cầu)
- [ ] `[M]` **FR-CQI-01:** Tạo ImprovementPlan từ kết quả/cảnh báo/phát hiện định tính; giữ liên kết nguồn.
- [ ] `[M]` **FR-CQI-02:** Lưu vấn đề, phân tích nguyên nhân, hành động, chủ trì, phối hợp, hạn, KPI, baseline và nguồn lực.
- [ ] `[M]` **FR-CQI-03:** Workflow phê duyệt–thực hiện–xác minh–đóng/mở lại; lưu ý kiến và lịch sử.
- [ ] `[S]` **FR-CQI-04:** Nhắc hạn, escalation, dashboard quá hạn và minh chứng thực hiện có checksum.
- [ ] `[M]` **FR-CQI-05:** Liên kết kỳ đo lại; so sánh trước/sau và ghi kết luận tác động hoặc chưa đủ bằng chứng.
- [ ] `[M]` **FR-CQI-06:** Chỉ đóng kế hoạch khi có minh chứng và người có quyền xác minh; cho phép mở action tiếp theo.

---

## 8.8. Chatbot và AI (8 Yêu cầu)
- [ ] `[S]` **FR-AI-01:** Chatbot hỏi đáp Khoa, CTĐT, học phần, kết quả và CQI theo dữ liệu được phép xem.
- [ ] `[M]` **FR-AI-02:** Câu trả lời có trích dẫn đối tượng/báo cáo, thời điểm dữ liệu và công thức liên quan.
- [ ] `[M]` **FR-AI-03:** Không trả dữ liệu cá nhân ngoài scope; áp dụng masking, ngưỡng nhóm và audit câu hỏi nhạy cảm.
- [ ] `[S]` **FR-AI-04:** AI trích xuất BM13/PDF/Word theo schema, giữ trang/vùng nguồn, confidence và nhãn inferred.
- [ ] `[S]` **FR-AI-05:** Phát hiện mâu thuẫn, trọng số sai, mã trùng, PI thiếu phủ và dữ liệu cần bổ sung; không tự sửa.
- [ ] `[M]` **FR-AI-06:** Hàng đợi human-in-the-loop chấp nhận/sửa/từ chối theo trường; giữ before/after và lý do.
- [ ] `[M]` **FR-AI-07:** Quản lý version prompt, loại câu hỏi, model, schema, ground-truth test và rollback.
- [ ] `[M]` **FR-AI-08:** Chống prompt injection từ tài liệu; giới hạn công cụ/API và không dùng dữ liệu để huấn luyện ngoài khi chưa phép.

---

## 8.9. Người dùng, quyền và cấu hình (8 Yêu cầu)
- [ ] `[M]` **FR-ADM-01:** SSO OIDC/SAML; ánh xạ danh tính tổ chức; xử lý khóa/nghỉ và phiên đăng nhập.
- [ ] `[M]` **FR-ADM-02:** Quản lý Role/Permission và scope Khoa–CTĐT–Khóa–Học phần–Lớp–Đợt.
- [ ] `[M]` **FR-ADM-03:** Gán vai trò có hiệu lực/thời hạn; hỗ trợ template và phê duyệt với vai trò nhạy cảm.
- [ ] `[M]` **FR-ADM-04:** Separation of duties giữa nhập/chấm, kiểm tra, duyệt/công bố và quản trị hệ thống.
- [ ] `[M]` **FR-ADM-05:** Audit bất biến cho đăng nhập, xem/xuất điểm, thay đổi cấu hình, tính, duyệt và mở khóa.
- [ ] `[M]` **FR-ADM-06:** Quản lý từ điển, năm học/HK, ngưỡng mặc định, lịch đồng bộ và trạng thái dịch vụ.
- [ ] `[M]` **FR-ADM-07:** Chính sách lưu trữ, xóa/ẩn danh, legal hold và xuất toàn bộ dữ liệu khi kết thúc hợp đồng.
- [ ] `[M]` **FR-ADM-08:** Trang quản trị chỉ hiển thị chức năng được phép; API luôn kiểm tra quyền server-side, không tin UI.

---

## 8.10. Tích hợp và API (8 Yêu cầu)
- [x] `[M]` **FR-INT-01:** Cung cấp API versioned `/api/v1`, OpenAPI, mã lỗi chuẩn và chính sách tương thích ngược.
- [ ] `[M]` **FR-INT-02:** Tích hợp SIS/LMS cho SV, CTĐT, khóa, lớp, enrollment, điểm và trạng thái học.
- [ ] `[M]` **FR-INT-03:** Hỗ trợ tải gia tăng theo updated_since/cursor, idempotency key, checksum và tải lại có kiểm soát.
- [ ] `[M]` **FR-INT-04:** Staging/quality gate cách ly bản ghi lỗi; dashboard đối soát và quy trình sửa ở nguồn.
- [ ] `[S]` **FR-INT-05:** Tích hợp DMS/Google Drive/SharePoint theo cấu hình; quyền tối thiểu, metadata và checksum.
- [ ] `[S]` **FR-INT-06:** Xuất dữ liệu tổng hợp cho BI/kho dữ liệu; không cho truy vấn vượt scope hoặc nhóm quá nhỏ.
- [ ] `[S]` **FR-INT-07:** Webhook/job bất đồng bộ cho chốt điểm, tính xong, công bố, lỗi đồng bộ và CQI quá hạn.
- [ ] `[M]` **FR-INT-08:** Service account theo scope, rotation/revocation, rate limit, request ID, metrics và audit API.
