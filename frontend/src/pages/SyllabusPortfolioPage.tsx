import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import {
  BookOpen,
  FileSpreadsheet,
  Layers,
  FileCheck,
  FolderArchive,
  Download,
  CheckCircle2,
  ShieldCheck,
  ClipboardList,
  FileText,
  Clock,
  UserCheck,
} from 'lucide-react';

export const SyllabusPortfolioPage: React.FC = () => {
  const location = useLocation();

  const getSubSection = () => {
    if (location.pathname.includes('/syllabus/plans')) return 'plans';
    if (location.pathname.includes('/syllabus/blueprints')) return 'blueprints';
    if (location.pathname.includes('/syllabus/rubrics')) return 'rubrics';
    if (location.pathname.includes('/syllabus/approvals')) return 'approvals';
    if (location.pathname.includes('/syllabus/exam-approvals')) return 'exam-approvals';
    return 'bm13';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Đề Cương & Đánh Giá Học Phần (Mục 8.3)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Quản lý Đề cương BM13, Kế hoạch đánh giá, Ma trận đề thi (Blueprint), Rubric và Quy trình phê duyệt số hóa.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Gói Portfolio (.ZIP)</span>
          </button>
          <button className="btn btn-primary">
            <CheckCircle2 size={16} />
            <span>Ban Hành Đề Cương</span>
          </button>
        </div>
      </div>

      {/* Navigation Sub-Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem', overflowX: 'auto' }}>
        {[
          { key: 'bm13', label: 'Đề Cương Chi Tiết (Mẫu BM13)', icon: BookOpen },
          { key: 'plans', label: 'Kế Hoạch Đánh Giá (A1, A2, A3)', icon: ClipboardList },
          { key: 'blueprints', label: 'Ma Trận Đề Thi (Exam Blueprint)', icon: FileCheck },
          { key: 'rubrics', label: 'Tiêu Chí Chấm Rubric', icon: Layers },
          { key: 'table832', label: 'Bảng 8.3.2 (Tỷ Trọng PI 100%)', icon: FileSpreadsheet, badge: 'Cốt Lõi' },
          { key: 'approvals', label: 'Phê Duyệt Đề Cương', icon: UserCheck },
          { key: 'exam-approvals', label: 'Phê Duyệt Đề Thi', icon: ShieldCheck },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
            {tab.badge && <span className="badge badge-bloom badge-cyan">{tab.badge}</span>}
          </button>
        ))}
      </div>

      {/* Scope Selector */}
      <div className="glass-card" style={{ marginBottom: '1.25rem', padding: '0.875rem 1.25rem' }}>
        <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center', flexWrap: 'wrap' }}>
          <div>
            <span className="form-label">Chọn Học Phần</span>
            <select className="form-select" style={{ width: '280px', marginTop: '0.25rem' }}>
              <option>IT4101 - Lập trình .NET Nâng cao (3 TC)</option>
              <option>IT2102 - Cấu trúc Dữ liệu & Giải thuật (3 TC)</option>
            </select>
          </div>
          <div>
            <span className="form-label">Phiên Bản Đề Cương</span>
            <select className="form-select" style={{ width: '220px', marginTop: '0.25rem' }}>
              <option>Phiên bản v2.1 (Năm học 2023 - 2024)</option>
              <option>Phiên bản v2.0 (Năm học 2022 - 2023)</option>
            </select>
          </div>
          <div>
            <span className="form-label">Trạng Thái Phê Duyệt</span>
            <div style={{ marginTop: '0.5rem' }}>
              <span className="badge badge-success">TRƯỞNG KHOA ĐÃ KÝ DUYỆT (PUBLISHED)</span>
            </div>
          </div>
        </div>
      </div>

      {/* TAB 1: ĐỀ CƯƠNG BM13 */}
      {activeTab === 'bm13' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Đề Cương Chi Tiết Học Phần (Mẫu BM13 - Đại Học Đại Nam)</h3>
              <p className="glass-card-subtitle">Mã môn: <strong>IT4101</strong> | Tên: <strong>Lập trình .NET Nâng cao</strong></p>
            </div>
            <button className="btn btn-sm btn-secondary">Tải File PDF BM13</button>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
            <div style={{ padding: '1rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
              <h4 style={{ color: 'var(--primary-400)', fontSize: '0.95rem', marginBottom: '0.5rem' }}>1. Thông tin chung</h4>
              <div style={{ fontSize: '0.8125rem', color: 'var(--text-secondary)', display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '0.5rem' }}>
                <div>• Số tín chỉ: <strong>3 TC (2 LT / 1 TH)</strong></div>
                <div>• Học phần tiên quyết: <strong>IT2102</strong></div>
                <div>• Giảng viên phụ trách: <strong>TS. Lê Hải Nam</strong></div>
              </div>
            </div>

            <div style={{ padding: '1rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
              <h4 style={{ color: 'var(--primary-400)', fontSize: '0.95rem', marginBottom: '0.5rem' }}>2. Mô tả học phần</h4>
              <p style={{ fontSize: '0.8125rem', color: 'var(--text-secondary)', lineHeight: '1.5' }}>
                Học phần cung cấp kiến thức chuyên sâu về nền tảng .NET 8 / C#, xây dựng RESTful Web API an toàn, tích hợp cơ sở dữ liệu với Entity Framework Core, áp dụng kiến trúc Clean Architecture, viết Unit Test tự động và triển khai ứng dụng lên Cloud Docker.
              </p>
            </div>

            <div style={{ padding: '1rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
              <h4 style={{ color: 'var(--primary-400)', fontSize: '0.95rem', marginBottom: '0.5rem' }}>3. Chuẩn đầu ra học phần (CLO) & Ánh xạ CĐR</h4>
              <div className="table-container">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Mã CLO</th>
                      <th>Nội Dung Chuẩn Đầu Ra</th>
                      <th>Mức Bloom</th>
                      <th>Ánh Xạ PI (CTĐT)</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td><strong className="badge badge-cyan">CLO1</strong></td>
                      <td>Xây dựng RESTful API chuẩn và tích hợp Entity Framework Core</td>
                      <td><span className="badge badge-bloom badge-cyan">APPLY (Mức 3)</span></td>
                      <td><strong>PI 3.1</strong> (Hiện thực giải pháp)</td>
                    </tr>
                    <tr>
                      <td><strong className="badge badge-cyan">CLO2</strong></td>
                      <td>Áp dụng Clean Architecture và phân lớp Dependency Injection</td>
                      <td><span className="badge badge-bloom badge-cyan">ANALYZE (Mức 4)</span></td>
                      <td><strong>PI 2.1</strong> (Phân tích thiết kế)</td>
                    </tr>
                    <tr>
                      <td><strong className="badge badge-cyan">CLO3</strong></td>
                      <td>Kiểm thử Unit Test và đánh giá an toàn bảo mật API</td>
                      <td><span className="badge badge-bloom badge-cyan">EVALUATE (Mức 5)</span></td>
                      <td><strong>PI 5.1</strong> (Kiểm thử phần mềm)</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* TAB 2: KẾ HOẠCH ĐÁNH GIÁ (PLANS) */}
      {activeTab === 'plans' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Kế Hoạch Đánh Giá Học Phần (A1, A2, A3)</h3>
              <p className="glass-card-subtitle">Phân bổ tỷ trọng điểm môn học theo quy định khảo thí (Tổng 100%)</p>
            </div>
            <span className="badge badge-success">Tổng: 100.0% Hợp lệ</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Bài Đánh Giá</th>
                  <th>Tên Bài Đánh Giá</th>
                  <th>Hình Thức Thực Hiện</th>
                  <th>Tỷ Trọng Môn (%)</th>
                  <th>CLO Đánh Giá</th>
                  <th>Vai Trò Đo Lường</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong className="badge badge-secondary">A1</strong></td>
                  <td>Đánh giá quá trình (Chuyên cần & Mini Quiz)</td>
                  <td>Trắc nghiệm LMS tuần 1 - 5</td>
                  <td><strong>20.0%</strong></td>
                  <td>CLO1</td>
                  <td><span className="badge badge-secondary">Đánh giá môn</span></td>
                </tr>
                <tr>
                  <td><strong className="badge badge-danger">A2</strong></td>
                  <td>Bài thực hành giữa kỳ (Web API & Unit Test)</td>
                  <td>Thực hành máy tính tuần 8</td>
                  <td><strong>30.0%</strong></td>
                  <td>CLO1, CLO3</td>
                  <td><span className="badge badge-danger">Đo Trực Tiếp (A - PI 3.1, 5.1)</span></td>
                </tr>
                <tr>
                  <td><strong className="badge badge-danger">A3</strong></td>
                  <td>Đồ án cuối kỳ (Hệ thống phần mềm hoàn chỉnh)</td>
                  <td>Bảo vệ đồ án & Vấn đáp</td>
                  <td><strong>50.0%</strong></td>
                  <td>CLO1, CLO2, CLO3</td>
                  <td><span className="badge badge-danger">Đo Trực Tiếp (A - PI 3.1)</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 3: BẢNG 8.3.2 (TỶ TRỌNG TRỰC TIẾP PI 100%) */}
      {activeTab === 'table832' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <Layers size={20} className="text-emerald-400" />
                Bảng 8.3.2: Phân Bổ Tỷ Trọng Trực Tiếp Từng Tiêu Chí Trong PI (Tổng Đúng 100%)
              </h3>
              <p className="glass-card-subtitle">
                Đảm nhận đo trực tiếp: <strong>PI 3.1, PI 5.1</strong>
              </p>
            </div>
            <span className="badge badge-success">Kiểm tra hợp lệ 100%</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Chỉ Số PI Giao Đo (A)</th>
                  <th>Bài Đánh Giá</th>
                  <th>Mã Tiêu Chí Rubric</th>
                  <th>Nội Dung Tiêu Chí Đánh Giá</th>
                  <th>CLO</th>
                  <th>Điểm Tối Đa</th>
                  <th style={{ textAlign: 'center' }}>Tỷ Trọng Trong PI (%)</th>
                  <th style={{ textAlign: 'center' }}>Trạng Thái Tổng</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td rowSpan={2} style={{ fontWeight: 800, color: 'var(--primary-400)', verticalAlign: 'middle', borderRight: '1px solid var(--border-subtle)' }}>
                    PI 3.1
                  </td>
                  <td><span className="badge badge-secondary">A2</span></td>
                  <td><code>CRIT-3.1-A</code></td>
                  <td>Thiết kế và xây dựng RESTful Web API an toàn</td>
                  <td><span className="badge badge-cyan">CLO1</span></td>
                  <td>10.0</td>
                  <td style={{ textAlign: 'center', fontWeight: 700 }}>40.0%</td>
                  <td rowSpan={2} style={{ textAlign: 'center', verticalAlign: 'middle', borderLeft: '1px solid var(--border-subtle)' }}>
                    <span className="badge badge-success">Tổng: 100.0% ✓</span>
                  </td>
                </tr>
                <tr>
                  <td><span className="badge badge-secondary">A3</span></td>
                  <td><code>CRIT-3.1-B</code></td>
                  <td>Tích hợp cơ sở dữ liệu và triển khai hệ thống hoàn chỉnh</td>
                  <td><span className="badge badge-cyan">CLO2</span></td>
                  <td>10.0</td>
                  <td style={{ textAlign: 'center', fontWeight: 700 }}>60.0%</td>
                </tr>

                <tr>
                  <td rowSpan={2} style={{ fontWeight: 800, color: 'var(--primary-400)', verticalAlign: 'middle', borderRight: '1px solid var(--border-subtle)' }}>
                    PI 5.1
                  </td>
                  <td><span className="badge badge-secondary">A2</span></td>
                  <td><code>CRIT-5.1-A</code></td>
                  <td>Viết Unit Test & Tích hợp kiểm thử tự động CI/CD</td>
                  <td><span className="badge badge-cyan">CLO3</span></td>
                  <td>10.0</td>
                  <td style={{ textAlign: 'center', fontWeight: 700 }}>50.0%</td>
                  <td rowSpan={2} style={{ textAlign: 'center', verticalAlign: 'middle', borderLeft: '1px solid var(--border-subtle)' }}>
                    <span className="badge badge-success">Tổng: 100.0% ✓</span>
                  </td>
                </tr>
                <tr>
                  <td><span className="badge badge-secondary">A3</span></td>
                  <td><code>CRIT-5.1-B</code></td>
                  <td>Báo cáo kiểm thử bảo mật và hiệu năng tải</td>
                  <td><span className="badge badge-cyan">CLO3</span></td>
                  <td>10.0</td>
                  <td style={{ textAlign: 'center', fontWeight: 700 }}>50.0%</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 4: RUBRICS */}
      {activeTab === 'rubrics' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Tiêu Chí Chấm Điểm Rubric Định Lượng</h3>
              <p className="glass-card-subtitle">4 mức đánh giá: Xuất sắc (8.5-10), Đạt tốt (7.0-8.4), Đạt (6.0-6.9), Chưa đạt (&lt;6.0)</p>
            </div>
            <button className="btn btn-sm btn-primary">+ Thêm Tiêu Chí Rubric</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Tiêu Chí</th>
                  <th>Nội Dung Tiêu Chí</th>
                  <th>Xuất Sắc (8.5 - 10)</th>
                  <th>Đạt Tốt (7.0 - 8.4)</th>
                  <th>Đạt Chuẩn (6.0 - 6.9)</th>
                  <th>Chưa Đạt (&lt; 6.0)</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>CRIT-3.1-A</strong></td>
                  <td>Xây dựng REST API chuẩn</td>
                  <td>Đầy đủ CRUD, JWT Auth, Swagger doc, mã code chuẩn Clean Architecture</td>
                  <td>Đủ CRUD, có JWT Auth, chạy ổn định nhưng thiếu xử lý lỗi nâng cao</td>
                  <td>Đủ CRUD cơ bản, chưa có xác thực JWT</td>
                  <td>Không chạy được API hoặc thiếu hơn 50% tính năng</td>
                </tr>
                <tr>
                  <td><strong>CRIT-5.1-A</strong></td>
                  <td>Unit Test & Code Coverage</td>
                  <td>Độ bao phủ code $\ge 80\%$, kiểm thử đầy đủ các ca ngoại lệ</td>
                  <td>Độ bao phủ $60\% - 79\%$, có kiểm thử các hàm chính</td>
                  <td>Độ bao phủ $50\% - 59\%$, kiểm thử cơ bản</td>
                  <td>Độ bao phủ $&lt; 50\%$ hoặc test bị fail</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 5: PHÊ DUYỆT ĐỀ CƯƠNG */}
      {activeTab === 'approvals' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Quy Trình Ký Duyệt Số Hóa Đề Cương BM13</h3>
              <p className="glass-card-subtitle">Tách biệt nhiệm vụ SoD: Tác giả soạn thảo ➔ Trưởng BM thẩm định ➔ Trưởng Khoa phê duyệt</p>
            </div>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {[
              { step: '1. Biên Soạn Đề Cương', actor: 'TS. Lê Hải Nam (Giảng viên)', status: 'HOÀN THÀNH', time: '10/08/2023 09:30' },
              { step: '2. Thẩm Định Ma Trận CĐR & Bảng 8.3.2', actor: 'TS. Vũ Minh Tuấn (Trưởng BM KTPM)', status: 'ĐÃ THẨM ĐỊNH', time: '12/08/2023 14:15' },
              { step: '3. Phê Duyệt & Ban Hành Quyết Định', actor: 'PGS. TS. Trần Văn Bình (Trưởng Khoa CNTT)', status: 'ĐÃ PHÊ DUYỆT', time: '15/08/2023 16:00' },
            ].map((st, i) => (
              <div key={i} style={{ padding: '1rem 1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                  <h4 style={{ color: 'var(--text-primary)', fontSize: '0.95rem', fontWeight: 700 }}>{st.step}</h4>
                  <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>Người thực hiện: <strong>{st.actor}</strong> ({st.time})</p>
                </div>
                <span className="badge badge-success"><ShieldCheck size={14} /> {st.status}</span>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 6: PHÊ DUYỆT ĐỀ THI */}
      {activeTab === 'exam-approvals' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Phê Duyệt Đề Thi & Đáp Án Thang Điểm (Exam Blueprint Approval)</h3>
              <p className="glass-card-subtitle">Hội đồng khảo thí và Trưởng bộ môn kiểm tra độ tương thích chuẩn Bloom</p>
            </div>
            <button className="btn btn-sm btn-primary">+ Nộp Đề Thi Mới</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Đề Thi</th>
                  <th>Học Phần</th>
                  <th>Kỳ Thi</th>
                  <th>Người Ra Đề</th>
                  <th>Kiểm Tra Bloom</th>
                  <th>Trạng Thái Phê Duyệt</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>EXAM-IT4101-2023-01</strong></td>
                  <td>IT4101: Lập trình .NET</td>
                  <td>Cuối kỳ HK1 (2023 - 2024)</td>
                  <td>TS. Lê Hải Nam</td>
                  <td><span className="badge badge-success">Khớp 100% Blueprint</span></td>
                  <td><span className="badge badge-success">ĐÃ DUYỆT (CHỜ THI)</span></td>
                </tr>
                <tr>
                  <td><strong>EXAM-IT2102-2023-01</strong></td>
                  <td>IT2102: Cấu trúc Dữ liệu</td>
                  <td>Cuối kỳ HK1 (2023 - 2024)</td>
                  <td>ThS. Nguyễn Văn Toàn</td>
                  <td><span className="badge badge-success">Khớp 100% Blueprint</span></td>
                  <td><span className="badge badge-success">ĐÃ DUYỆT (CHỜ THI)</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};
