import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Plus,
  Search,
  Download,
  Edit2,
  Trash2,
  X,
  Save,
  CheckCircle,
} from 'lucide-react';
import { EmptyState } from '../components/common/EmptyState';

interface OrgUnitItem {
  code: string;
  name: string;
  level: string;
  leader: string;
  count: string;
  status: string;
}

interface ProgramItem {
  code: string;
  name: string;
  faculty: string;
  duration: string;
  credits: number;
  version: string;
  status: string;
}

interface CohortItem {
  code: string;
  name: string;
  start: number;
  end: number;
  count: number;
  version: string;
  status: string;
}

interface StudentItem {
  code: string;
  name: string;
  cohort: string;
  program: string;
  cls: string;
  path: string;
  credits: number;
  status: string;
}

interface CourseItem {
  code: string;
  name: string;
  credits: number;
  hours: string;
  dept: string;
  syllabus: string;
  aTag: string;
}

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

  // Dynamic Data Lists (Clean - Start with 0 records)
  const [orgUnits, setOrgUnits] = useState<OrgUnitItem[]>([]);
  const [programs, setPrograms] = useState<ProgramItem[]>([]);
  const [cohorts, setCohorts] = useState<CohortItem[]>([]);
  const [students, setStudents] = useState<StudentItem[]>([]);
  const [courses, setCourses] = useState<CourseItem[]>([]);

  // Form State
  const [formCode, setFormCode] = useState('');
  const [formName, setFormName] = useState('');
  const [formFaculty, setFormFaculty] = useState('');
  const [formDesc, setFormDesc] = useState('');

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleSaveModal = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formCode.trim() || !formName.trim()) return;

    if (activeTab === 'org-units') {
      setOrgUnits([
        ...orgUnits,
        { code: formCode, name: formName, level: 'KHOA/VIỆN', leader: 'Chưa phân công', count: '0 Ngành', status: 'HOẠT ĐỘNG' },
      ]);
    } else if (activeTab === 'programs') {
      setPrograms([
        ...programs,
        { code: formCode, name: formName, faculty: formFaculty || 'Khoa mặc định', duration: '4 Năm', credits: 145, version: 'Mặc định', status: 'HOẠT ĐỘNG' },
      ]);
    } else if (activeTab === 'cohorts') {
      setCohorts([
        ...cohorts,
        { code: formCode, name: formName, start: new Date().getFullYear(), end: new Date().getFullYear() + 4, count: 0, version: 'Chưa gán', status: 'HOẠT ĐỘNG' },
      ]);
    } else if (activeTab === 'students') {
      setStudents([
        ...students,
        { code: formCode, name: formName, cohort: 'K17', program: 'Chưa phân ngành', cls: 'Lớp 1', path: 'Tiêu chuẩn', credits: 0, status: 'ĐANG HỌC' },
      ]);
    } else if (activeTab === 'courses') {
      setCourses([
        ...courses,
        { code: formCode, name: formName, credits: 3, hours: '2 LT / 1 TH', dept: formFaculty || 'Bộ môn', syllabus: 'Chưa có', aTag: 'Không' },
      ]);
    }

    setFormCode('');
    setFormName('');
    setFormFaculty('');
    setFormDesc('');
    setIsModalOpen(false);
    setToastMessage('✓ Đã lưu thành công dữ liệu mới vào hệ thống!');
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
            {activeTab === 'cohorts' && 'Danh Sách Khóa Tuyển Sinh'}
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
            <span>Thêm Mới Dữ Liệu</span>
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
        </div>
      </div>

      {/* TAB 1: ĐƠN VỊ - KHOA */}
      {activeTab === 'org-units' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Cơ Cấu Tổ Chức Trường / Khoa / Viện / Bộ Môn</h3>
              <p className="glass-card-subtitle">Phân định thẩm quyền quản lý học thuật và phạm vi dữ liệu Scope</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Đơn Vị</button>
          </div>

          {orgUnits.length === 0 ? (
            <EmptyState
              title="Chưa có Đơn vị – Khoa / Viện nào"
              description="Hiện tại chưa có đơn vị quản lý nào trong hệ thống. Hãy thêm Khoa, Viện hoặc Bộ môn đầu tiên."
              actionLabel="+ Thêm Đơn Vị Đầu Tiên"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
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
                  {orgUnits.map((row) => (
                    <tr key={row.code}>
                      <td><code>{row.code}</code></td>
                      <td style={{ fontWeight: 700 }}>{row.name}</td>
                      <td><span className="badge badge-primary">{row.level}</span></td>
                      <td>{row.leader}</td>
                      <td><span className="badge badge-cyan">{row.count}</span></td>
                      <td><span className="badge badge-success">{row.status}</span></td>
                      <td style={{ textAlign: 'right' }}>
                        <button className="btn btn-sm btn-secondary" style={{ marginRight: '0.35rem' }}><Edit2 size={12} /></button>
                        <button onClick={() => setOrgUnits(orgUnits.filter((x) => x.code !== row.code))} className="btn btn-sm btn-secondary"><Trash2 size={12} /></button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
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

          {programs.length === 0 ? (
            <EmptyState
              title="Chưa có Ngành đào tạo nào"
              description="Hiện tại chưa có ngành đào tạo nào trong hệ thống. Nhấn nút bên dưới để thêm ngành mới."
              actionLabel="+ Thêm Ngành Mới"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
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
                  {programs.map((p) => (
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
          )}
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

          {cohorts.length === 0 ? (
            <EmptyState
              title="Chưa có Khóa tuyển sinh nào"
              description="Hiện tại chưa có khóa tuyển sinh nào trong hệ thống. Nhấn nút bên dưới để khởi tạo khóa mới."
              actionLabel="+ Khởi Tạo Khóa Mới"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
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
                  {cohorts.map((c) => (
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
          )}
        </div>
      )}

      {/* TAB 4: SINH VIÊN */}
      {activeTab === 'students' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Hồ Sơ Sinh Viên & Định Tuyến CTĐT Theo Khóa</h3>
              <p className="glass-card-subtitle">Đồng bộ tự động từ hệ thống Quản lý Đào tạo SIS hoặc nhập tay</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Sinh Viên</button>
          </div>

          {students.length === 0 ? (
            <EmptyState
              title="Chưa có dữ liệu Sinh viên"
              description="Hiện tại chưa có hồ sơ sinh viên nào. Bạn có thể thêm sinh viên hoặc kết nối đồng bộ từ hệ thống SIS."
              actionLabel="+ Thêm Sinh Viên Mới"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
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
                  {students.map((s) => (
                    <tr key={s.code}>
                      <td><code>{s.code}</code></td>
                      <td style={{ fontWeight: 700 }}>{s.name}</td>
                      <td><span className="badge badge-primary">{s.cohort}</span></td>
                      <td>{s.program}</td>
                      <td>{s.cls}</td>
                      <td><span className="badge badge-cyan">{s.path}</span></td>
                      <td><strong>{s.credits} TC</strong></td>
                      <td><span className="badge badge-success">{s.status}</span></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
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

          {courses.length === 0 ? (
            <EmptyState
              title="Chưa có Học phần nào"
              description="Hiện tại chưa có môn học nào trong danh mục. Nhấn nút bên dưới để thêm học phần mới."
              actionLabel="+ Thêm Học Phần Mới"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
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
                  {courses.map((c) => (
                    <tr key={c.code}>
                      <td><code>{c.code}</code></td>
                      <td style={{ fontWeight: 700 }}>{c.name}</td>
                      <td><strong>{c.credits} TC</strong></td>
                      <td>{c.hours}</td>
                      <td>{c.dept}</td>
                      <td><span className="badge badge-cyan">{c.syllabus}</span></td>
                      <td><span className="badge badge-secondary">{c.aTag}</span></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* CREATE / EDIT MODAL DIALOG */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '540px', maxWidth: '100%', maxHeight: '90vh', overflowY: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>
                Thêm Mới: {activeTab === 'org-units' ? 'Đơn Vị - Khoa' : activeTab === 'programs' ? 'Ngành Đào Tạo' : activeTab === 'cohorts' ? 'Khóa Tuyển Sinh' : activeTab === 'students' ? 'Hồ Sơ Sinh Viên' : 'Học Phần'}
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveModal}>
              <div className="form-group">
                <label className="form-label">Mã Định Danh (Code)</label>
                <input required type="text" placeholder="Nhập mã định danh..." value={formCode} onChange={(e) => setFormCode(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Tên Gọi Đầy Đủ</label>
                <input required type="text" placeholder="Nhập tên gọi đầy đủ..." value={formName} onChange={(e) => setFormName(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Khoa / Đơn Vị Phụ Trách</label>
                <input type="text" placeholder="Nhập khoa / đơn vị quản lý..." value={formFaculty} onChange={(e) => setFormFaculty(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Ghi Chú & Mô Tả</label>
                <textarea rows={3} className="form-textarea" placeholder="Mô tả bổ sung nếu có..." value={formDesc} onChange={(e) => setFormDesc(e.target.value)} />
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
