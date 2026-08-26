import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Database,
  Building2,
  GraduationCap,
  Calendar,
  Users,
  BookOpen,
  Plus,
  Search,
  Download,
  Edit2,
  Trash2,
  X,
  Save,
  CheckCircle,
} from 'lucide-react';

export const AcademicDataPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/data/programs')) return 'programs';
    if (location.pathname.includes('/data/cohorts')) return 'cohorts';
    if (location.pathname.includes('/data/students')) return 'students';
    if (location.pathname.includes('/data/courses')) return 'courses';
    return 'org-units';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  // Sync state whenever URL pathname changes
  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleTabClick = (key: string) => {
    setActiveTab(key);
    navigate(`/data/${key}`);
  };

  const handleSaveModal = (e: React.FormEvent) => {
    e.preventDefault();
    setIsModalOpen(false);
    setToastMessage('✓ Đã lưu thành công dữ liệu vào hệ thống!');
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

      {/* Page Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <div style={{ fontSize: '0.75rem', color: 'var(--primary-400)', fontWeight: 700, textTransform: 'uppercase', marginBottom: '0.25rem' }}>
            Dữ Liệu Đào Tạo
          </div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            {activeTab === 'org-units' && 'Đơn Vị – Khoa / Viện'}
            {activeTab === 'programs' && 'Ngành Đào Tạo Trực Thuộc'}
            {activeTab === 'cohorts' && 'Danh Sách Khóa Tuyển Sinh (K15 - K18)'}
            {activeTab === 'students' && 'Hồ Sơ Sinh Viên & Định Tuyến CTĐT'}
            {activeTab === 'courses' && 'Danh Mục Học Phần Toàn Trường'}
          </h2>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Excel</span>
          </button>
          <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
            <Plus size={16} />
            <span>+ Thêm Mới Dữ Liệu</span>
          </button>
        </div>
      </div>

      {/* Filter / Search Bar */}
      <div className="glass-card" style={{ marginBottom: '1.25rem', padding: '0.875rem 1.25rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '0.75rem' }}>
          <div style={{ position: 'relative', width: '320px' }}>
            <Search size={16} style={{ position: 'absolute', left: '0.75rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Tìm kiếm mã, tên, đơn vị..."
              className="form-input"
              style={{ paddingLeft: '2.25rem' }}
            />
          </div>

          <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'center' }}>
            <span style={{ fontSize: '0.8125rem', color: 'var(--text-secondary)' }}>Lọc theo Khoa:</span>
            <select className="form-select" style={{ width: '220px' }}>
              <option>Khoa Công nghệ Thông tin</option>
              <option>Khoa Quản trị Kinh doanh</option>
              <option>Khoa Ngôn ngữ Anh</option>
            </select>
          </div>
        </div>
      </div>

      {/* TAB 1: ĐƠN VỊ - KHOA */}
      {activeTab === 'org-units' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Cây Cơ Cấu Tổ Chức Trường / Khoa / Viện / Bộ Môn</h3>
              <p className="glass-card-subtitle">Phân định thẩm quyền quản lý học thuật và phạm vi dữ liệu Scope</p>
            </div>
            <span className="badge badge-success">3 Khoa / 8 Bộ môn</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Đơn Vị</th>
                  <th>Tên Khoa / Viện / Bộ Môn</th>
                  <th>Cấp Đơn Vị</th>
                  <th>Trưởng Đơn Vị</th>
                  <th>Số Ngành Quản Lý</th>
                  <th>Trạng Thái</th>
                  <th style={{ textAlign: 'right' }}>Thao Tác</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: 'OU-CNTT', name: 'Khoa Công nghệ Thông tin', level: 'KHOA/VIỆN', leader: 'PGS. TS. Trần Văn Bình', count: '3 Ngành', status: 'HOẠT ĐỘNG' },
                  { code: 'OU-KTPM', name: 'Bộ môn Kỹ thuật Phần mềm', level: 'BỘ MÔN', leader: 'TS. Lê Hải Nam', count: '1 Ngành', status: 'HOẠT ĐỘNG' },
                  { code: 'OU-HTTT', name: 'Bộ môn Hệ thống Thông tin', level: 'BỘ MÔN', leader: 'TS. Vũ Minh Tuấn', count: '1 Ngành', status: 'HOẠT ĐỘNG' },
                  { code: 'OU-QTKD', name: 'Khoa Quản trị Kinh doanh', level: 'KHOA/VIỆN', leader: 'TS. Nguyễn Hoàng Linh', count: '2 Ngành', status: 'HOẠT ĐỘNG' },
                ].map((row) => (
                  <tr key={row.code}>
                    <td><code>{row.code}</code></td>
                    <td style={{ fontWeight: 700 }}>{row.name}</td>
                    <td><span className="badge badge-primary">{row.level}</span></td>
                    <td>{row.leader}</td>
                    <td><span className="badge badge-cyan">{row.count}</span></td>
                    <td><span className="badge badge-success">{row.status}</span></td>
                    <td style={{ textAlign: 'right' }}>
                      <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-secondary" style={{ marginRight: '0.35rem' }}><Edit2 size={12} /></button>
                      <button className="btn btn-sm btn-secondary"><Trash2 size={12} /></button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 2: NGÀNH ĐÀO TẠO */}
      {activeTab === 'programs' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Ngành Đào Tạo Trực Thuộc</h3>
              <p className="glass-card-subtitle">Quản lý mã ngành chuẩn Bộ GD&ĐT và số tín chỉ yêu cầu</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Ngành Mới</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Ngành (MOET)</th>
                  <th>Tên Ngành Đào Tạo</th>
                  <th>Khoa Phụ Trách</th>
                  <th>Thời Gian Đào Tạo</th>
                  <th>Số Tín Chỉ</th>
                  <th>Phiên Bản CTĐT Đang Dùng</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: '7480201', name: 'Kỹ thuật Phần mềm', faculty: 'Khoa CNTT', duration: '4 Năm (8 Học kỳ)', credits: 145, version: 'v2023 (ABET)', status: 'TUYỂN SINH' },
                  { code: '7480101', name: 'Khoa học Máy tính', faculty: 'Khoa CNTT', duration: '4 Năm (8 Học kỳ)', credits: 142, version: 'v2022', status: 'TUYỂN SINH' },
                  { code: '7480104', name: 'Hệ thống Thông tin', faculty: 'Khoa CNTT', duration: '4 Năm (8 Học kỳ)', credits: 140, version: 'v2021', status: 'TUYỂN SINH' },
                  { code: '7340101', name: 'Quản trị Kinh doanh', faculty: 'Khoa QTKD', duration: '4 Năm (8 Học kỳ)', credits: 135, version: 'v2023', status: 'TUYỂN SINH' },
                ].map((p) => (
                  <tr key={p.code}>
                    <td><strong className="badge badge-secondary">{p.code}</strong></td>
                    <td style={{ fontWeight: 700, color: 'var(--primary-400)' }}>{p.name}</td>
                    <td>{p.faculty}</td>
                    <td>{p.duration}</td>
                    <td><strong>{p.credits} TC</strong></td>
                    <td><span className="badge badge-cyan">{p.version}</span></td>
                    <td><span className="badge badge-success">{p.status}</span></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 3: KHÓA TUYỂN SINH */}
      {activeTab === 'cohorts' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Sách Khóa Tuyển Sinh & Khung Thời Gian Áp Dụng</h3>
              <p className="glass-card-subtitle">Mỗi khóa tuyển sinh được gán đúng phiên bản CTĐT riêng biệt</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Khởi Tạo Khóa Tuyển Sinh</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Khóa</th>
                  <th>Tên Khóa Tuyển Sinh</th>
                  <th>Năm Tuyển Sinh</th>
                  <th>Năm Dự Kiến Tốt Nghiệp</th>
                  <th>Số Sinh Viên</th>
                  <th>Phiên Bản CTĐT Áp Dụng</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: 'K15', name: 'Khóa 15 (2021 - 2025)', start: 2021, end: 2025, count: 420, version: 'CTĐT KTPM v2021 (6 PLO)', status: 'NĂM 4 (SẮP TỐT NGHIỆP)' },
                  { code: 'K16', name: 'Khóa 16 (2022 - 2026)', start: 2022, end: 2026, count: 480, version: 'CTĐT KTPM v2022 (6 PLO)', status: 'NĂM 3 (CHUYÊN NGÀNH)' },
                  { code: 'K17', name: 'Khóa 17 (2023 - 2027)', start: 2023, end: 2027, count: 560, version: 'CTĐT KTPM v2023 (9 PLO ABET)', status: 'NĂM 2 (CƠ SỞ NGÀNH)' },
                  { code: 'K18', name: 'Khóa 18 (2024 - 2028)', start: 2024, end: 2028, count: 620, version: 'CTĐT KTPM v2024 (9 PLO ABET)', status: 'NĂM 1 (ĐẠI CƯƠNG)' },
                ].map((c) => (
                  <tr key={c.code}>
                    <td><strong className="badge badge-primary">{c.code}</strong></td>
                    <td style={{ fontWeight: 700 }}>{c.name}</td>
                    <td>{c.start}</td>
                    <td>{c.end}</td>
                    <td><strong>{c.count} SV</strong></td>
                    <td><span className="badge badge-cyan">{c.version}</span></td>
                    <td><span className="badge badge-success">{c.status}</span></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 4: SINH VIÊN */}
      {activeTab === 'students' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Hồ Sơ Sinh Viên & Định Tuyến CTĐT Theo Khóa</h3>
              <p className="glass-card-subtitle">Đồng bộ tự động từ hệ thống Quản lý Đào tạo SIS</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Sinh Viên</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Sinh Viên</th>
                  <th>Họ Và Tên</th>
                  <th>Khóa</th>
                  <th>Ngành Học</th>
                  <th>Lớp Sinh Hoạt</th>
                  <th>Lộ Trình (StudentPath)</th>
                  <th>Số TC Tích Lũy</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: '20230001', name: 'Nguyễn Văn An', cohort: 'K17', program: 'Kỹ thuật Phần mềm', cls: '17IT01', path: 'Chuyên ngành Web/Cloud', credits: 45, status: 'ĐANG HỌC' },
                  { code: '20230002', name: 'Trần Thị Bình', cohort: 'K17', program: 'Kỹ thuật Phần mềm', cls: '17IT01', path: 'Chuyên ngành Mobile/AI', credits: 48, status: 'ĐANG HỌC' },
                  { code: '20230003', name: 'Lê Hoàng Cường', cohort: 'K17', program: 'Kỹ thuật Phần mềm', cls: '17IT02', path: 'Chuyên ngành Web/Cloud', credits: 42, status: 'CẢNH BÁO CĐR' },
                  { code: '20220015', name: 'Phạm Minh Đức', cohort: 'K16', program: 'Kỹ thuật Phần mềm', cls: '16IT01', path: 'Chuyên ngành Web/Cloud', credits: 82, status: 'ĐANG HỌC' },
                ].map((s) => (
                  <tr key={s.code}>
                    <td><code>{s.code}</code></td>
                    <td style={{ fontWeight: 700 }}>{s.name}</td>
                    <td><span className="badge badge-primary">{s.cohort}</span></td>
                    <td>{s.program}</td>
                    <td>{s.cls}</td>
                    <td><span className="badge badge-cyan">{s.path}</span></td>
                    <td><strong>{s.credits} TC</strong></td>
                    <td>
                      <span className={`badge ${s.status === 'ĐANG HỌC' ? 'badge-success' : 'badge-warning'}`}>
                        {s.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 5: HỌC PHẦN */}
      {activeTab === 'courses' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Học Phần Trong Toàn Trường</h3>
              <p className="glass-card-subtitle">Mỗi học phần có thể gắn với nhiều phiên bản Đề cương BM13 của từng CTĐT</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Học Phần</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Học Phần</th>
                  <th>Tên Học Phần</th>
                  <th>Số TC</th>
                  <th>Lý Thuyết / Thực Hành</th>
                  <th>Bộ Môn Quản Lý</th>
                  <th>Đề Cương Đang Áp Dụng</th>
                  <th>Đảm Nhận Đo A</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: 'IT1101', name: 'Nhập môn Lập trình C/C++', credits: 3, hours: '2 LT / 1 TH', dept: 'Bộ môn KTPM', syllabus: 'BM13 v2.0 (2023)', aTag: 'Không' },
                  { code: 'IT2102', name: 'Cấu trúc Dữ liệu & Giải thuật', credits: 3, hours: '2 LT / 1 TH', dept: 'Bộ môn KTPM', syllabus: 'BM13 v2.0 (2023)', aTag: 'Không' },
                  { code: 'IT4101', name: 'Lập trình .NET Nâng cao', credits: 3, hours: '2 LT / 1 TH', dept: 'Bộ môn KTPM', syllabus: 'BM13 v2.1 (2023)', aTag: 'Đo PI 3.1, PI 5.1' },
                  { code: 'IT4205', name: 'Kiểm thử Phần mềm & QA', credits: 3, hours: '2 LT / 1 TH', dept: 'Bộ môn KTPM', syllabus: 'BM13 v2.0 (2023)', aTag: 'Đo PI 5.2' },
                  { code: 'IT4999', name: 'Khóa luận Tốt nghiệp', credits: 10, hours: 'Đồ án tốt nghiệp', dept: 'Khoa CNTT', syllabus: 'BM13 v2.0 (2023)', aTag: 'Đo toàn bộ PLO' },
                ].map((c) => (
                  <tr key={c.code}>
                    <td><code>{c.code}</code></td>
                    <td style={{ fontWeight: 700 }}>{c.name}</td>
                    <td><strong>{c.credits} TC</strong></td>
                    <td>{c.hours}</td>
                    <td>{c.dept}</td>
                    <td><span className="badge badge-cyan">{c.syllabus}</span></td>
                    <td>
                      <span className={`badge ${c.aTag.includes('Đo') ? 'badge-danger' : 'badge-secondary'}`}>
                        {c.aTag}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* CREATE / EDIT MODAL DIALOG */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '540px', maxWidth: '100%', maxHeight: '90vh', overflowY: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>
                Thêm Mới / Cập Nhật: {activeTab === 'org-units' ? 'Đơn Vị - Khoa' : activeTab === 'programs' ? 'Ngành Đào Tạo' : activeTab === 'cohorts' ? 'Khóa Tuyển Sinh' : activeTab === 'students' ? 'Hồ Sơ Sinh Viên' : 'Học Phần'}
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveModal}>
              <div className="form-group">
                <label className="form-label">Mã Định Danh (Code)</label>
                <input required type="text" placeholder="Ví dụ: IT4101, K17, 7480201..." className="form-input" defaultValue="IT_NEW_01" />
              </div>

              <div className="form-group">
                <label className="form-label">Tên Gọi Đầy Đủ</label>
                <input required type="text" placeholder="Nhập tên đối tượng..." className="form-input" defaultValue="Lập trình Cloud Native & Kubernetes" />
              </div>

              <div className="form-group">
                <label className="form-label">Khoa / Đơn Vị Phụ Trách</label>
                <select className="form-select">
                  <option>Khoa Công nghệ Thông tin</option>
                  <option>Khoa Quản trị Kinh doanh</option>
                </select>
              </div>

              <div className="form-group">
                <label className="form-label">Ghi Chú & Mô Tả</label>
                <textarea rows={3} className="form-textarea" placeholder="Mô tả bổ sung nếu có..." defaultValue="Áp dụng từ năm học 2023-2024" />
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Dữ Liệu</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
