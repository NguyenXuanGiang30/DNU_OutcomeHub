import React, { useState, useMemo } from 'react';
import {
  Users,
  Activity,
  AlertCircle,
  Search,
  RotateCcw,
  Plus,
  Table as TableIcon,
  BarChart3,
  X,
  Save,
  CheckCircle,
  UserCheck,
  Edit2,
  Trash2,
} from 'lucide-react';
import {
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
} from 'recharts';

interface StudentItem {
  id: string;
  code: string;
  fullName: string;
  email: string;
  cohort: string;
  className: string;
  faculty: string;
  program: string;
  status: string;
}

export const StudentManagementPage: React.FC = () => {
  const [viewMode, setViewMode] = useState<'list' | 'report'>('list');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedFaculty, setSelectedFaculty] = useState('ALL');
  const [selectedProgram, setSelectedProgram] = useState('ALL');
  const [selectedClass, setSelectedClass] = useState('ALL');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  // Dynamic Student Dataset (Clean initial state)
  const [students, setStudents] = useState<StudentItem[]>([]);

  // Form Fields for Modal
  const [formCode, setFormCode] = useState('');
  const [formFullName, setFormFullName] = useState('');
  const [formEmail, setFormEmail] = useState('');
  const [formCohort, setFormCohort] = useState('Khóa K17 (2023 - 2027)');
  const [formClass, setFormClass] = useState('17IT01');
  const [formFaculty, setFormFaculty] = useState('Khoa Công nghệ Thông tin');
  const [formProgram, setFormProgram] = useState('Kỹ thuật Phần mềm');

  // Filter Logic
  const filteredStudents = useMemo(() => {
    return students.filter((s) => {
      const matchSearch =
        !searchTerm.trim() ||
        s.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
        s.fullName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        s.className.toLowerCase().includes(searchTerm.toLowerCase());

      const matchFaculty = selectedFaculty === 'ALL' || s.faculty === selectedFaculty;
      const matchProgram = selectedProgram === 'ALL' || s.program === selectedProgram;
      const matchClass = selectedClass === 'ALL' || s.className === selectedClass;

      return matchSearch && matchFaculty && matchProgram && matchClass;
    });
  }, [students, searchTerm, selectedFaculty, selectedProgram, selectedClass]);

  // Unique Faculties and Programs count
  const facultyCount = useMemo(() => {
    return new Set(students.map((s) => s.faculty)).size;
  }, [students]);

  const programCount = useMemo(() => {
    return new Set(students.map((s) => s.program)).size;
  }, [students]);

  // Reset Filters
  const handleResetFilters = () => {
    setSearchTerm('');
    setSelectedFaculty('ALL');
    setSelectedProgram('ALL');
    setSelectedClass('ALL');
  };

  // Add New Student
  const handleSaveStudent = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formCode.trim() || !formFullName.trim()) return;

    const newStudent: StudentItem = {
      id: `std-${Date.now()}`,
      code: formCode.trim().toUpperCase(),
      fullName: formFullName.trim(),
      email: formEmail.trim() || `${formCode.toLowerCase()}@dnu.edu.vn`,
      cohort: formCohort,
      className: formClass,
      faculty: formFaculty,
      program: formProgram,
      status: 'Đang học',
    };

    setStudents([...students, newStudent]);
    setFormCode('');
    setFormFullName('');
    setFormEmail('');
    setIsModalOpen(false);
    setToastMessage(`✓ Đã thêm sinh viên ${newStudent.code} - ${newStudent.fullName}!`);
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleDeleteStudent = (code: string) => {
    setStudents(students.filter((s) => s.code !== code));
    setToastMessage('✓ Đã xóa sinh viên khỏi danh sách!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  return (
    <div className="animate-fade-in" style={{ paddingBottom: '2rem' }}>
      {/* Toast Alert */}
      {toastMessage && (
        <div style={{ position: 'fixed', top: '85px', right: '2rem', zIndex: 100, backgroundColor: 'var(--emerald-500)', color: '#fff', padding: '0.75rem 1.25rem', borderRadius: 'var(--radius-md)', boxShadow: 'var(--glass-shadow)', display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: 600 }}>
          <CheckCircle size={18} />
          <span>{toastMessage}</span>
        </div>
      )}

      {/* TOP HEADER */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '1.5rem' }}>
        <div>
          <h1 style={{ fontSize: '2rem', fontWeight: 900, color: 'var(--text-primary)', letterSpacing: '-0.02em', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <span>QUẢN LÝ</span>
            <span style={{ color: 'var(--primary-400)' }}>SINH VIÊN</span>
          </h1>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.35rem', display: 'flex', alignItems: 'center', gap: '0.4rem' }}>
            <Users size={16} />
            <span>Quản lý hồ sơ, lớp học phần và kết quả đo lường OBE</span>
          </p>
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          {/* Toggle View: Danh sách / Báo cáo */}
          <div style={{ display: 'flex', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: '10px', padding: '0.25rem', border: '1px solid var(--border-medium)' }}>
            <button
              onClick={() => setViewMode('list')}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '0.4rem',
                padding: '0.5rem 1rem',
                borderRadius: '8px',
                border: 'none',
                backgroundColor: viewMode === 'list' ? '#1e3a8a' : 'transparent',
                color: viewMode === 'list' ? '#fff' : 'var(--text-secondary)',
                fontWeight: 700,
                fontSize: '0.8125rem',
                cursor: 'pointer',
                transition: 'all 0.2s',
              }}
            >
              <TableIcon size={15} />
              <span>Danh sách</span>
            </button>

            <button
              onClick={() => setViewMode('report')}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '0.4rem',
                padding: '0.5rem 1rem',
                borderRadius: '8px',
                border: 'none',
                backgroundColor: viewMode === 'report' ? '#1e3a8a' : 'transparent',
                color: viewMode === 'report' ? '#fff' : 'var(--text-secondary)',
                fontWeight: 700,
                fontSize: '0.8125rem',
                cursor: 'pointer',
                transition: 'all 0.2s',
              }}
            >
              <BarChart3 size={15} />
              <span>Báo cáo</span>
            </button>
          </div>

          <button onClick={() => setIsModalOpen(true)} className="btn btn-primary" style={{ padding: '0.55rem 1.25rem' }}>
            <Plus size={16} />
            <span>Thêm Sinh Viên</span>
          </button>
        </div>
      </div>

      {/* KPI CARDS (Row of 3 cards) */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1.25rem', marginBottom: '1.5rem' }}>
        {/* Card 1: Tổng sinh viên */}
        <div className="glass-card" style={{ padding: '1.25rem 1.5rem', display: 'flex', alignItems: 'center', gap: '1.25rem', borderRadius: '16px' }}>
          <div style={{ width: '48px', height: '48px', borderRadius: '12px', backgroundColor: 'rgba(99, 102, 241, 0.12)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--primary-400)' }}>
            <Users size={24} />
          </div>
          <div>
            <div style={{ fontSize: '0.72rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
              TỔNG SINH VIÊN
            </div>
            <div style={{ fontSize: '1.75rem', fontWeight: 900, color: 'var(--text-primary)', marginTop: '0.15rem' }}>
              {students.length}
            </div>
          </div>
        </div>

        {/* Card 2: Số lượng khoa */}
        <div className="glass-card" style={{ padding: '1.25rem 1.5rem', display: 'flex', alignItems: 'center', gap: '1.25rem', borderRadius: '16px' }}>
          <div style={{ width: '48px', height: '48px', borderRadius: '12px', backgroundColor: 'rgba(16, 185, 129, 0.12)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--emerald-400)' }}>
            <Activity size={24} />
          </div>
          <div>
            <div style={{ fontSize: '0.72rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
              SỐ LƯỢNG KHOA
            </div>
            <div style={{ fontSize: '1.75rem', fontWeight: 900, color: 'var(--text-primary)', marginTop: '0.15rem' }}>
              {facultyCount}
            </div>
          </div>
        </div>

        {/* Card 3: Số lượng ngành */}
        <div className="glass-card" style={{ padding: '1.25rem 1.5rem', display: 'flex', alignItems: 'center', gap: '1.25rem', borderRadius: '16px' }}>
          <div style={{ width: '48px', height: '48px', borderRadius: '12px', backgroundColor: 'rgba(245, 158, 11, 0.12)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--amber-400)' }}>
            <AlertCircle size={24} />
          </div>
          <div>
            <div style={{ fontSize: '0.72rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
              SỐ LƯỢNG NGÀNH
            </div>
            <div style={{ fontSize: '1.75rem', fontWeight: 900, color: 'var(--text-primary)', marginTop: '0.15rem' }}>
              {programCount}
            </div>
          </div>
        </div>
      </div>

      {/* FILTER CARD BAR */}
      <div className="glass-card" style={{ padding: '1rem 1.25rem', marginBottom: '1.5rem', borderRadius: '16px' }}>
        <div style={{ display: 'grid', gridTemplateColumns: '2fr 1.2fr 1.2fr 1.2fr auto', gap: '1rem', alignItems: 'center' }}>
          {/* Search */}
          <div style={{ position: 'relative' }}>
            <Search size={16} style={{ position: 'absolute', left: '0.85rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Tìm tên, mã số, lớp..."
              className="form-input"
              style={{ paddingLeft: '2.4rem', borderRadius: '10px' }}
            />
          </div>

          {/* Khoa */}
          <select
            value={selectedFaculty}
            onChange={(e) => setSelectedFaculty(e.target.value)}
            className="form-select"
            style={{ borderRadius: '10px' }}
          >
            <option value="ALL">Tất cả Khoa</option>
            <option value="Khoa Công nghệ Thông tin">Khoa Công nghệ Thông tin</option>
            <option value="Khoa Quản trị Kinh doanh">Khoa Quản trị Kinh doanh</option>
            <option value="Khoa Dược">Khoa Dược</option>
          </select>

          {/* Ngành */}
          <select
            value={selectedProgram}
            onChange={(e) => setSelectedProgram(e.target.value)}
            className="form-select"
            style={{ borderRadius: '10px' }}
          >
            <option value="ALL">Tất cả Ngành</option>
            <option value="Kỹ thuật Phần mềm">Kỹ thuật Phần mềm</option>
            <option value="Khoa học Máy tính">Khoa học Máy tính</option>
            <option value="Hệ thống Thông tin">Hệ thống Thông tin</option>
          </select>

          {/* Lớp */}
          <select
            value={selectedClass}
            onChange={(e) => setSelectedClass(e.target.value)}
            className="form-select"
            style={{ borderRadius: '10px' }}
          >
            <option value="ALL">Tất cả Lớp học</option>
            <option value="17IT01">Lớp 17IT01</option>
            <option value="17IT02">Lớp 17IT02</option>
            <option value="18IT01">Lớp 18IT01</option>
          </select>

          {/* Reset */}
          <button
            onClick={handleResetFilters}
            className="btn btn-secondary"
            style={{ padding: '0.65rem 1.25rem', borderRadius: '10px', display: 'flex', alignItems: 'center', gap: '0.4rem' }}
          >
            <RotateCcw size={15} />
            <span>Reset</span>
          </button>
        </div>
      </div>

      {/* VIEW 1: DANH SÁCH (TABLE) */}
      {viewMode === 'list' && (
        <div className="glass-card" style={{ padding: '1.25rem', borderRadius: '16px' }}>
          {filteredStudents.length === 0 ? (
            <div style={{ textAlign: 'center', padding: '4rem 1.5rem' }}>
              <div style={{ width: '64px', height: '64px', borderRadius: '50%', backgroundColor: 'rgba(255, 255, 255, 0.04)', border: '1px solid var(--border-medium)', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 1.25rem auto', color: 'var(--text-muted)' }}>
                <Users size={32} />
              </div>
              <h3 style={{ fontSize: '1rem', fontWeight: 800, color: 'var(--text-secondary)', letterSpacing: '0.05em', textTransform: 'uppercase' }}>
                KHÔNG TÌM THẤY SINH VIÊN
              </h3>
              <p style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginTop: '0.35rem' }}>
                Chưa có dữ liệu sinh viên phù hợp với bộ lọc hiện tại. Nhấn nút bên dưới để thêm mới.
              </p>
              <button onClick={() => setIsModalOpen(true)} className="btn btn-primary" style={{ marginTop: '1.25rem' }}>
                <Plus size={16} />
                <span>Thêm Sinh Viên Mới</span>
              </button>
            </div>
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>SINH VIÊN</th>
                    <th>LỚP / KHÓA</th>
                    <th>KHOA / NGÀNH</th>
                    <th>TRẠNG THÁI</th>
                    <th style={{ textAlign: 'right' }}>THAO TÁC</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredStudents.map((s) => (
                    <tr key={s.id}>
                      <td>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                          <div style={{ width: '36px', height: '36px', borderRadius: '50%', backgroundColor: 'rgba(99, 102, 241, 0.15)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--primary-400)', fontWeight: 800 }}>
                            {s.fullName.slice(0, 1)}
                          </div>
                          <div>
                            <div style={{ fontWeight: 800, color: 'var(--text-primary)' }}>{s.fullName}</div>
                            <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{s.code} • {s.email}</div>
                          </div>
                        </div>
                      </td>
                      <td>
                        <div style={{ fontWeight: 700 }}>{s.className}</div>
                        <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>{s.cohort}</div>
                      </td>
                      <td>
                        <div style={{ fontWeight: 700, color: 'var(--primary-400)' }}>{s.program}</div>
                        <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>{s.faculty}</div>
                      </td>
                      <td>
                        <span className="badge badge-success">{s.status}</span>
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <button onClick={() => handleDeleteStudent(s.code)} className="btn btn-sm btn-secondary" title="Xóa sinh viên">
                          <Trash2 size={13} />
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* VIEW 2: BÁO CÁO (ANALYTICS & CHARTS) */}
      {viewMode === 'report' && (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
          {/* Section 1: Phân bố sinh viên theo khóa/ngành */}
          <div className="glass-card" style={{ padding: '1.5rem', borderRadius: '16px' }}>
            <h3 style={{ fontSize: '1rem', fontWeight: 800, color: 'var(--text-primary)', textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <BarChart3 size={18} style={{ color: 'var(--primary-400)' }} />
              <span>PHÂN BỐ SINH VIÊN THEO KHÓA/NGÀNH</span>
            </h3>

            {students.length === 0 ? (
              <div style={{ height: '260px', display: 'flex', alignItems: 'center', justifyContent: 'center', border: '1px dashed var(--border-medium)', borderRadius: '12px', color: 'var(--text-muted)', fontSize: '0.875rem' }}>
                Chưa có dữ liệu sinh viên để hiển thị biểu đồ phân bố
              </div>
            ) : (
              <div style={{ width: '100%', height: '300px' }}>
                <ResponsiveContainer>
                  <BarChart data={[{ name: 'K17 - KTPM', count: students.length }]}>
                    <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.08)" />
                    <XAxis dataKey="name" stroke="var(--text-secondary)" />
                    <YAxis stroke="var(--text-secondary)" />
                    <Tooltip contentStyle={{ backgroundColor: 'var(--bg-surface-elevated)', borderColor: 'var(--border-strong)', borderRadius: '8px' }} />
                    <Bar dataKey="count" name="Số lượng sinh viên" fill="#6366f1" radius={[6, 6, 0, 0]} />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            )}
          </div>

          {/* Section 2: Số lượng sinh viên theo ngành */}
          <div className="glass-card" style={{ padding: '1.5rem', borderRadius: '16px' }}>
            <h3 style={{ fontSize: '1rem', fontWeight: 800, color: 'var(--text-primary)', textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '1rem' }}>
              SỐ LƯỢNG SINH VIÊN THEO NGÀNH
            </h3>

            {students.length === 0 ? (
              <div style={{ padding: '2rem', textAlign: 'center', color: 'var(--text-muted)', fontSize: '0.85rem' }}>
                Chưa có dữ liệu theo ngành đào tạo
              </div>
            ) : (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', padding: '0.75rem 1rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: '10px' }}>
                  <span style={{ fontWeight: 700 }}>Kỹ thuật Phần mềm</span>
                  <span style={{ fontWeight: 900, color: 'var(--primary-400)' }}>{students.length} Sinh viên</span>
                </div>
              </div>
            )}
          </div>
        </div>
      )}

      {/* MODAL THÊM SINH VIÊN */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 100, backgroundColor: 'rgba(0, 0, 0, 0.75)', backdropFilter: 'blur(10px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '600px', maxWidth: '100%', maxHeight: '90vh', overflowY: 'auto', padding: '2rem', borderRadius: '16px', border: '1px solid rgba(255, 255, 255, 0.15)' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.25rem', fontWeight: 900, color: 'var(--text-primary)' }}>
                Thêm Mới Hồ Sơ Sinh Viên
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveStudent}>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                <div className="form-group">
                  <label className="form-label">Mã Sinh Viên (MSSV) *</label>
                  <input required type="text" placeholder="VD: 20230001" value={formCode} onChange={(e) => setFormCode(e.target.value)} className="form-input" />
                </div>

                <div className="form-group">
                  <label className="form-label">Họ Và Tên Sinh Viên *</label>
                  <input required type="text" placeholder="VD: Nguyễn Văn An" value={formFullName} onChange={(e) => setFormFullName(e.target.value)} className="form-input" />
                </div>
              </div>

              <div className="form-group">
                <label className="form-label">Email Trường</label>
                <input type="email" placeholder="VD: an.nv2023@dnu.edu.vn" value={formEmail} onChange={(e) => setFormEmail(e.target.value)} className="form-input" />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                <div className="form-group">
                  <label className="form-label">Khóa Tuyển Sinh</label>
                  <select value={formCohort} onChange={(e) => setFormCohort(e.target.value)} className="form-select">
                    <option>Khóa K17 (2023 - 2027)</option>
                    <option>Khóa K16 (2022 - 2026)</option>
                    <option>Khóa K18 (2024 - 2028)</option>
                  </select>
                </div>

                <div className="form-group">
                  <label className="form-label">Lớp Sinh Hoạt</label>
                  <input type="text" placeholder="VD: 17IT01" value={formClass} onChange={(e) => setFormClass(e.target.value)} className="form-input" />
                </div>
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                <div className="form-group">
                  <label className="form-label">Khoa Phụ Trách</label>
                  <select value={formFaculty} onChange={(e) => setFormFaculty(e.target.value)} className="form-select">
                    <option>Khoa Công nghệ Thông tin</option>
                    <option>Khoa Quản trị Kinh doanh</option>
                    <option>Khoa Dược</option>
                  </select>
                </div>

                <div className="form-group">
                  <label className="form-label">Ngành Đào Tạo</label>
                  <select value={formProgram} onChange={(e) => setFormProgram(e.target.value)} className="form-select">
                    <option>Kỹ thuật Phần mềm</option>
                    <option>Khoa học Máy tính</option>
                    <option>Hệ thống Thông tin</option>
                  </select>
                </div>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.75rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Sinh Viên</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
