import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
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
  UserCheck,
  Plus,
  X,
  Save,
  CheckCircle,
} from 'lucide-react';

export const SyllabusPortfolioPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/syllabus/plans')) return 'plans';
    if (location.pathname.includes('/syllabus/blueprints')) return 'blueprints';
    if (location.pathname.includes('/syllabus/rubrics')) return 'rubrics';
    if (location.pathname.includes('/syllabus/approvals')) return 'approvals';
    if (location.pathname.includes('/syllabus/exam-approvals')) return 'exam-approvals';
    return 'bm13';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleTabClick = (key: string) => {
    setActiveTab(key);
    navigate(`/syllabus/${key}`);
  };

  const handleSaveModal = (e: React.FormEvent) => {
    e.preventDefault();
    setIsModalOpen(false);
    setToastMessage('✓ Đã cập nhật thành công đề cương / rubric!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  return (
    <div className="animate-fade-in">
      {/* Toast Alert */}
      {toastMessage && (
        <div style={{ position: 'fixed', top: '85px', right: '2rem', zIndex: 100, backgroundColor: 'var(--emerald-500)', color: '#fff', padding: '0.75rem 1.25rem', borderRadius: 'var(--radius-md)', boxShadow: 'var(--glass-shadow)', display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: 600 }}>
          <CheckCircle size={18} />
          <span>{toastMessage}</span>
        </div>
      )}

      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Đề Cương & Đánh Giá Học Phần
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
          <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
            <Plus size={16} />
            <span>+ Thêm Mới / Cập Nhật</span>
          </button>
        </div>
      </div>

      {/* Navigation Sub-Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem', overflowX: 'auto' }}>
        {[
          { key: 'bm13', label: '1. Đề Cương Chi Tiết (BM13)', icon: BookOpen },
          { key: 'plans', label: '2. Kế Hoạch Đánh Giá (A1, A2, A3)', icon: ClipboardList },
          { key: 'blueprints', label: '3. Đề Thi – Bài Đánh Giá (Blueprint)', icon: FileCheck },
          { key: 'rubrics', label: '4. Tiêu Chí Chấm Rubric', icon: Layers },
          { key: 'approvals', label: '5. Phê Duyệt Đề Cương', icon: UserCheck },
          { key: 'exam-approvals', label: '6. Phê Duyệt Đề Thi', icon: ShieldCheck },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => handleTabClick(tab.key)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
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
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Bài Đánh Giá</button>
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

      {/* TAB 3: ĐỀ THI & BLUEPRINT */}
      {activeTab === 'blueprints' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Ma Trận Đề Thi & Ngân Hàng Câu Hỏi (Exam Blueprint)</h3>
              <p className="glass-card-subtitle">Phân bổ câu hỏi theo chuẩn năng lực Bloom</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Tạo Cấu Trúc Đề Thi</button>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1rem' }}>
            {[
              { section: 'Phần 1: Trắc nghiệm Kiến thức', items: '10 Câu (3.0 Điểm)', bloom: 'REMEMBER / UNDERSTAND' },
              { section: 'Phần 2: Viết Code Xử Lý', items: '2 Bài tập (4.0 Điểm)', bloom: 'APPLY (Mức 3)' },
              { section: 'Phần 3: Thiết Kế & Tối Ưu', items: '1 Bài toán (3.0 Điểm)', bloom: 'ANALYZE (Mức 4)' },
            ].map((s, i) => (
              <div key={i} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <strong style={{ color: 'var(--text-primary)' }}>{s.section}</strong>
                <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
                  • Điểm số: <strong>{s.items}</strong><br />
                  • Mức Bloom: <span className="badge badge-bloom badge-cyan">{s.bloom}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 4: RUBRIC */}
      {activeTab === 'rubrics' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Tiêu Chí Chấm Điểm Rubric Định Lượng</h3>
              <p className="glass-card-subtitle">4 mức đánh giá: Xuất sắc (8.5-10), Đạt tốt (7.0-8.4), Đạt (6.0-6.9), Chưa đạt (&lt;6.0)</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Tiêu Chí Rubric</button>
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
              <h3 className="glass-card-title">Quy Trình Phê Duyệt Số Hóa Đề Cương BM13</h3>
              <p className="glass-card-subtitle">Tuân thủ nghiêm ngặt quy định Tách biệt nhiệm vụ SoD 3 cấp</p>
            </div>
            <button className="btn btn-sm btn-primary"><CheckCircle2 size={14} /> Ký Duyệt Cấp Khoa</button>
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
              <h3 className="glass-card-title">Phê Duyệt Đề Thi & Đáp Án Thang Điểm (Exam Blueprint)</h3>
              <p className="glass-card-subtitle">Hội đồng khảo thí kiểm tra độ khớp ngân hàng câu hỏi</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Nộp Đề Thi Mới</button>
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
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* MODAL DIALOG */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '540px', maxWidth: '100%', maxHeight: '90vh', overflowY: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>
                Thao Tác: {activeTab === 'plans' ? 'Thêm Bài Đánh Giá' : activeTab === 'rubrics' ? 'Tiêu Chí Rubric' : 'Đề Cương & Đề Thi'}
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveModal}>
              <div className="form-group">
                <label className="form-label">Tên Bài / Tiêu Chí</label>
                <input required type="text" placeholder="Nhập tên..." className="form-input" defaultValue="Bài thực hành kiểm thử tự động" />
              </div>

              <div className="form-group">
                <label className="form-label">Tỷ Trọng Điểm (%)</label>
                <input required type="number" step="5" defaultValue="30" className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">CLO Đảm Nhận</label>
                <select className="form-select">
                  <option>CLO1 - Xây dựng RESTful API</option>
                  <option>CLO3 - Kiểm thử Unit Test</option>
                </select>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Thay Đổi</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
