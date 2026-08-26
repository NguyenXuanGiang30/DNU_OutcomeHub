import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  ClipboardCheck,
  Calculator,
  Upload,
  CheckCircle,
  Plus,
  X,
  Save,
  Target,
} from 'lucide-react';
import { EmptyState } from '../components/common/EmptyState';

interface StudentScoreRow {
  studentCode: string;
  fullName: string;
  crit1: number;
  crit2: number;
  crit3: number;
  finalScore: number;
  piAttainment: 'MET' | 'NOT_MET';
}

interface MeasurementPeriodItem {
  code: string;
  name: string;
  faculty: string;
  academicYear: string;
  semester: string;
  cohort: string;
  program: string;
  thetaInd: number;
  thetaCoh: number;
  status: string;
}

export const MeasurementScoringPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/measurement/periods')) return 'periods';
    if (location.pathname.includes('/measurement/sources')) return 'sources';
    if (location.pathname.includes('/measurement/assignments')) return 'assignments';
    if (location.pathname.includes('/measurement/sync-grades')) return 'sync-grades';
    if (location.pathname.includes('/measurement/data-validation')) return 'data-validation';
    if (location.pathname.includes('/measurement/calculation')) return 'calculation';
    if (location.pathname.includes('/measurement/evidence')) return 'evidence';
    return 'periods';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  // Dynamic States
  const [periods, setPeriods] = useState<MeasurementPeriodItem[]>([]);
  const [scores, setScores] = useState<StudentScoreRow[]>([]);

  // Form Fields matching User Mockup exactly
  const [formPeriodCode, setFormPeriodCode] = useState('');
  const [formPeriodName, setFormPeriodName] = useState('');
  const [formAcademicYear, setFormAcademicYear] = useState('2024 - 2025');
  const [formSemester, setFormSemester] = useState('Học kỳ 1');
  const [formCohort, setFormCohort] = useState('Khóa 17 (2023 - 2027)');
  const [formFaculty, setFormFaculty] = useState('Khoa Công nghệ Thông tin');
  const [formCohortFilter, setFormCohortFilter] = useState('Tất cả niên khóa');
  const [formProgram, setFormProgram] = useState('Kỹ thuật Phần mềm');
  const [formThetaInd, setFormThetaInd] = useState<number>(50);
  const [formThetaCoh, setFormThetaCoh] = useState<number>(70);

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleScoreChange = (index: number, field: 'crit1' | 'crit2' | 'crit3', value: number) => {
    const updated = [...scores];
    updated[index][field] = value;
    const avg = (updated[index].crit1 + updated[index].crit2 + updated[index].crit3) / 3;
    updated[index].finalScore = Math.round(avg * 10) / 10;
    updated[index].piAttainment = updated[index].finalScore >= (formThetaInd / 10) ? 'MET' : 'NOT_MET';
    setScores(updated);
  };

  const handleSaveScores = () => {
    setToastMessage('✓ Đã lưu thành công điểm Rubric vào hệ thống!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleRunCalculation = () => {
    if (scores.length === 0 && periods.length === 0) {
      setToastMessage('⚠ Vui lòng tạo đợt đo và nhập bảng điểm để tính toán CĐR!');
      setTimeout(() => setToastMessage(null), 3000);
      return;
    }
    setToastMessage('✓ Động cơ tính toán OBE đã cập nhật xong kết quả!');
    setTimeout(() => setToastMessage(null), 3500);
  };

  const handleCreatePeriod = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formPeriodCode.trim() || !formPeriodName.trim()) return;

    const newPeriod: MeasurementPeriodItem = {
      code: formPeriodCode.trim().toUpperCase(),
      name: formPeriodName.trim(),
      faculty: formFaculty,
      academicYear: formAcademicYear,
      semester: formSemester,
      cohort: formCohort,
      program: formProgram,
      thetaInd: formThetaInd,
      thetaCoh: formThetaCoh,
      status: 'ĐANG THU THẬP ĐIỂM',
    };

    setPeriods([...periods, newPeriod]);
    setFormPeriodCode('');
    setFormPeriodName('');
    setIsModalOpen(false);
    setToastMessage(`✓ Đã khởi tạo thành công Đợt đo lường: ${newPeriod.code}!`);
    setTimeout(() => setToastMessage(null), 3500);
  };

  const handleAddSampleStudent = () => {
    setScores([
      ...scores,
      {
        studentCode: `2023000${scores.length + 1}`,
        fullName: `Sinh viên ${scores.length + 1}`,
        crit1: 8.0,
        crit2: 7.5,
        crit3: 8.0,
        finalScore: 7.8,
        piAttainment: 'MET',
      },
    ]);
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
          <div style={{ fontSize: '0.75rem', color: 'var(--primary-400)', fontWeight: 700, textTransform: 'uppercase', marginBottom: '0.25rem' }}>
            Đo Lường Chuẩn Đầu Ra
          </div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            {activeTab === 'periods' && 'Danh Sách Các Đợt Đo Lường (Measurement Periods)'}
            {activeTab === 'sources' && 'Nguồn Đo Trực Tiếp PI (A Sources)'}
            {activeTab === 'assignments' && 'Phân Công Giảng Viên Chấm Điểm Rubric'}
            {activeTab === 'sync-grades' && 'Nhập & Đồng Bộ Điểm Quá Trình'}
            {activeTab === 'rubric-scoring' && 'Bảng Điểm Rubric Chi Tiết Từng Tiêu Chí'}
            {activeTab === 'data-validation' && 'Kiểm Tra Dữ Liệu Điểm Đo Lường'}
            {activeTab === 'calculation' && 'Động Cơ Tính Toán Kết Quả Đạt CĐR'}
            {activeTab === 'evidence' && 'Minh Chứng Đo Lường & Bài Làm Mẫu'}
          </h2>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button onClick={handleRunCalculation} className="btn btn-secondary">
            <Calculator size={16} />
            <span>Chạy Tính Toán CĐR</span>
          </button>
          <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
            <Plus size={16} />
            <span>Tạo Đợt Đo Lường Mới</span>
          </button>
        </div>
      </div>

      {/* TAB 1: PERIODS */}
      {activeTab === 'periods' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Sách Các Đợt Đo Lường Chuẩn Đầu Ra</h3>
              <p className="glass-card-subtitle">Mỗi đợt đo liên kết với Khoa, Ngành/CTĐT, Niên khóa và bộ Mục tiêu đo lường θ_ind / θ_coh</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">Tạo Đợt Đo Mới</button>
          </div>

          {periods.length === 0 ? (
            <EmptyState
              title="Chưa có Đợt đo lường nào"
              description="Hiện tại chưa có đợt đo lường nào được kích hoạt. Nhấn nút bên dưới để mở form khởi tạo đợt đo chuẩn OBE."
              actionLabel="Tạo Đợt Đo Lường Đầu Tiên"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã Đợt Đo</th>
                    <th>Tên Đợt Đo Lường</th>
                    <th>Khoa Phụ Trách</th>
                    <th>Ngành / CTĐT</th>
                    <th>Năm Học & Học Kỳ</th>
                    <th>Niên Khóa</th>
                    <th style={{ textAlign: 'center' }}>% Điểm Đạt (θ_ind)</th>
                    <th style={{ textAlign: 'center' }}>% SV Đạt/PI (θ_coh)</th>
                    <th>Trạng Thái</th>
                  </tr>
                </thead>
                <tbody>
                  {periods.map((p) => (
                    <tr key={p.code}>
                      <td><strong className="badge badge-primary">{p.code}</strong></td>
                      <td style={{ fontWeight: 700 }}>{p.name}</td>
                      <td>{p.faculty}</td>
                      <td><span className="badge badge-cyan">{p.program}</span></td>
                      <td>{p.academicYear} ({p.semester})</td>
                      <td><span className="badge badge-secondary">{p.cohort}</span></td>
                      <td style={{ textAlign: 'center', fontWeight: 700, color: 'var(--emerald-400)' }}>≥ {p.thetaInd}%</td>
                      <td style={{ textAlign: 'center', fontWeight: 700, color: 'var(--primary-400)' }}>≥ {p.thetaCoh}%</td>
                      <td><span className="badge badge-success">{p.status}</span></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* TAB 5: RUBRIC SCORING */}
      {activeTab === 'rubric-scoring' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <ClipboardCheck size={20} className="text-primary-400" />
                Bảng Điểm Rubric Chi Tiết Từng Tiêu Chí
              </h3>
              <p className="glass-card-subtitle">
                Ngưỡng cá nhân đạt chuẩn (θ_ind): <strong>≥ {formThetaInd}% ({formThetaInd / 10}/10)</strong>
              </p>
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <button onClick={handleAddSampleStudent} className="btn btn-sm btn-secondary">Thêm Sinh Viên</button>
              {scores.length > 0 && (
                <button onClick={handleSaveScores} className="btn btn-sm btn-primary">
                  <Save size={14} />
                  <span>Lưu Điểm</span>
                </button>
              )}
            </div>
          </div>

          {scores.length === 0 ? (
            <EmptyState
              title="Chưa có dữ liệu bảng điểm Rubric"
              description="Hiện tại lớp học phần này chưa có danh sách sinh viên chấm điểm. Nhấn nút bên dưới để thêm sinh viên."
              actionLabel="Thêm Sinh Viên Vào Bảng Điểm"
              onAction={handleAddSampleStudent}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã SV</th>
                    <th>Họ Và Tên</th>
                    <th style={{ textAlign: 'center' }}>Tiêu Chí 1 (40%)</th>
                    <th style={{ textAlign: 'center' }}>Tiêu Chí 2 (30%)</th>
                    <th style={{ textAlign: 'center' }}>Tiêu Chí 3 (30%)</th>
                    <th style={{ textAlign: 'center' }}>Điểm Tổng (Thang 10)</th>
                    <th style={{ textAlign: 'center' }}>Đạt Chuẩn</th>
                  </tr>
                </thead>
                <tbody>
                  {scores.map((row, idx) => (
                    <tr key={row.studentCode}>
                      <td><strong>{row.studentCode}</strong></td>
                      <td style={{ fontWeight: 600 }}>{row.fullName}</td>
                      <td style={{ textAlign: 'center' }}>
                        <input
                          type="number"
                          min="0"
                          max="10"
                          step="0.5"
                          value={row.crit1}
                          onChange={(e) => handleScoreChange(idx, 'crit1', parseFloat(e.target.value) || 0)}
                          className="form-input"
                          style={{ width: '70px', textAlign: 'center', display: 'inline-block' }}
                        />
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <input
                          type="number"
                          min="0"
                          max="10"
                          step="0.5"
                          value={row.crit2}
                          onChange={(e) => handleScoreChange(idx, 'crit2', parseFloat(e.target.value) || 0)}
                          className="form-input"
                          style={{ width: '70px', textAlign: 'center', display: 'inline-block' }}
                        />
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <input
                          type="number"
                          min="0"
                          max="10"
                          step="0.5"
                          value={row.crit3}
                          onChange={(e) => handleScoreChange(idx, 'crit3', parseFloat(e.target.value) || 0)}
                          className="form-input"
                          style={{ width: '70px', textAlign: 'center', display: 'inline-block' }}
                        />
                      </td>
                      <td style={{ textAlign: 'center', fontWeight: 800, fontSize: '1rem' }}>
                        {row.finalScore}
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <span className={`badge ${row.piAttainment === 'MET' ? 'badge-success' : 'badge-danger'}`}>
                          {row.piAttainment === 'MET' ? 'ĐẠT CHUẨN' : 'CHƯA ĐẠT'}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* TAB: SYNC GRADES */}
      {activeTab === 'sync-grades' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Nhập & Đồng Bộ Điểm Quá Trình Từ SIS / LMS</h3>
              <p className="glass-card-subtitle">Nạp điểm tự động qua API hoặc tải lên file bảng điểm</p>
            </div>
          </div>

          <div style={{ padding: '2.5rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '2px dashed var(--border-strong)', textAlign: 'center' }}>
            <Upload size={40} style={{ color: 'var(--primary-400)', margin: '0 auto 0.75rem auto' }} />
            <h4 style={{ color: 'var(--text-primary)', marginBottom: '0.25rem' }}>Kéo thả file Excel bảng điểm vào đây hoặc nhấn để chọn file</h4>
            <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginBottom: '1.25rem' }}>Hỗ trợ file .xlsx, .csv theo mẫu chuẩn BM13</p>
            <button onClick={() => { setToastMessage('✓ Vui lòng chọn file từ máy tính'); setTimeout(() => setToastMessage(null), 2000); }} className="btn btn-primary">Chọn File Tải Lên</button>
          </div>
        </div>
      )}

      {/* EXACT USER MOCKUP MODAL: TẠO ĐỢT ĐO LƯỜNG */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 100, backgroundColor: 'rgba(0, 0, 0, 0.75)', backdropFilter: 'blur(10px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '840px', maxWidth: '100%', maxHeight: '92vh', overflowY: 'auto', padding: '2rem', borderRadius: '16px', border: '1px solid rgba(255, 255, 255, 0.15)', boxShadow: '0 25px 50px -12px rgba(0, 0, 0, 0.5)' }}>
            {/* Modal Header */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '1.75rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '1rem' }}>
              <div>
                <h2 style={{ fontSize: '1.5rem', fontWeight: 900, color: 'var(--text-primary)', letterSpacing: '0.02em', textTransform: 'uppercase' }}>
                  TẠO ĐỢT ĐO LƯỜNG MỚI
                </h2>
                <p style={{ fontSize: '0.8rem', color: 'var(--text-muted)', fontStyle: 'italic', marginTop: '0.25rem', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                  CẬP NHẬT DỮ LIỆU NỀN CHO HỆ THỐNG ĐO LƯỜNG OBE
                </p>
              </div>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon" style={{ borderRadius: '50%', padding: '0.4rem' }}>
                <X size={18} />
              </button>
            </div>

            {/* Modal Form */}
            <form onSubmit={handleCreatePeriod}>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem' }}>
                {/* LEFT COLUMN */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
                  {/* Mã Đợt Đo */}
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label" style={{ fontSize: '0.75rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                      MÃ ĐỢT ĐO LƯỜNG *
                    </label>
                    <input
                      required
                      type="text"
                      placeholder="VD: OBE-2024-HK1-IT"
                      value={formPeriodCode}
                      onChange={(e) => setFormPeriodCode(e.target.value)}
                      className="form-input"
                      style={{ padding: '0.75rem 1rem', fontSize: '0.9rem', borderRadius: '10px' }}
                    />
                  </div>

                  {/* Tên Đợt Đo */}
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label" style={{ fontSize: '0.75rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                      TÊN ĐỢT ĐO LƯỜNG *
                    </label>
                    <input
                      required
                      type="text"
                      placeholder="VD: Đợt đo lường HK1 - Năm học 2024-2025"
                      value={formPeriodName}
                      onChange={(e) => setFormPeriodName(e.target.value)}
                      className="form-input"
                      style={{ padding: '0.75rem 1rem', fontSize: '0.9rem', borderRadius: '10px' }}
                    />
                  </div>

                  {/* Năm Học & Học Kỳ */}
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label" style={{ fontSize: '0.75rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                        NĂM HỌC *
                      </label>
                      <select
                        value={formAcademicYear}
                        onChange={(e) => setFormAcademicYear(e.target.value)}
                        className="form-select"
                        style={{ padding: '0.75rem 1rem', fontSize: '0.9rem', borderRadius: '10px' }}
                      >
                        <option>2024 - 2025</option>
                        <option>2023 - 2024</option>
                        <option>2022 - 2023</option>
                      </select>
                    </div>

                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label" style={{ fontSize: '0.75rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                        HỌC KỲ *
                      </label>
                      <select
                        value={formSemester}
                        onChange={(e) => setFormSemester(e.target.value)}
                        className="form-select"
                        style={{ padding: '0.75rem 1rem', fontSize: '0.9rem', borderRadius: '10px' }}
                      >
                        <option>Học kỳ 1</option>
                        <option>Học kỳ 2</option>
                        <option>Học kỳ 3 (Hè)</option>
                      </select>
                    </div>
                  </div>

                  {/* Niên Khóa */}
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label" style={{ fontSize: '0.75rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                      NIÊN KHÓA *
                    </label>
                    <select
                      value={formCohort}
                      onChange={(e) => setFormCohort(e.target.value)}
                      className="form-select"
                      style={{ padding: '0.75rem 1rem', fontSize: '0.9rem', borderRadius: '10px' }}
                    >
                      <option>Khóa 17 (2023 - 2027)</option>
                      <option>Khóa 16 (2022 - 2026)</option>
                      <option>Khóa 15 (2021 - 2025)</option>
                      <option>Khóa 18 (2024 - 2028)</option>
                    </select>
                  </div>
                </div>

                {/* RIGHT COLUMN */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
                  {/* Khoa Phụ Trách */}
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label" style={{ fontSize: '0.75rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                      KHOA PHỤ TRÁCH *
                    </label>
                    <select
                      value={formFaculty}
                      onChange={(e) => setFormFaculty(e.target.value)}
                      className="form-select"
                      style={{ padding: '0.75rem 1rem', fontSize: '0.9rem', borderRadius: '10px' }}
                    >
                      <option>Khoa Công nghệ Thông tin</option>
                      <option>Khoa Quản trị Kinh doanh</option>
                      <option>Khoa Dược</option>
                      <option>Khoa Ngôn ngữ Anh</option>
                    </select>
                  </div>

                  {/* Lọc Theo Niên Khóa & Ngành / CTĐT */}
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label" style={{ fontSize: '0.75rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                        LỌC THEO NIÊN KHÓA
                      </label>
                      <select
                        value={formCohortFilter}
                        onChange={(e) => setFormCohortFilter(e.target.value)}
                        className="form-select"
                        style={{ padding: '0.75rem 1rem', fontSize: '0.9rem', borderRadius: '10px' }}
                      >
                        <option>Tất cả niên khóa</option>
                        <option>Khóa K17</option>
                        <option>Khóa K16</option>
                      </select>
                    </div>

                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label" style={{ fontSize: '0.75rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                        NGÀNH / CTĐT *
                      </label>
                      <select
                        value={formProgram}
                        onChange={(e) => setFormProgram(e.target.value)}
                        className="form-select"
                        style={{ padding: '0.75rem 1rem', fontSize: '0.9rem', borderRadius: '10px' }}
                      >
                        <option>Kỹ thuật Phần mềm</option>
                        <option>Khoa học Máy tính</option>
                        <option>Hệ thống Thông tin</option>
                      </select>
                    </div>
                  </div>

                  {/* MỤC TIÊU ĐO LƯỜNG BOX */}
                  <div style={{ backgroundColor: 'rgba(99, 102, 241, 0.06)', border: '1px solid rgba(99, 102, 241, 0.25)', borderRadius: '14px', padding: '1.25rem' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '1rem', color: '#1e3a8a' }}>
                      <Target size={18} style={{ color: 'var(--primary-400)' }} />
                      <span style={{ fontWeight: 900, fontSize: '0.85rem', textTransform: 'uppercase', letterSpacing: '0.05em', color: 'var(--primary-300)' }}>
                        MỤC TIÊU ĐO LƯỜNG
                      </span>
                    </div>

                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                      {/* % ĐIỂM SV CẦN ĐẠT */}
                      <div>
                        <label style={{ display: 'block', fontSize: '0.72rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', marginBottom: '0.35rem' }}>
                          % ĐIỂM SV CẦN ĐẠT
                        </label>
                        <input
                          required
                          type="number"
                          min="1"
                          max="100"
                          value={formThetaInd}
                          onChange={(e) => setFormThetaInd(Number(e.target.value))}
                          className="form-input"
                          style={{ textAlign: 'center', fontSize: '1.15rem', fontWeight: 800, padding: '0.5rem', borderRadius: '10px' }}
                        />
                        <span style={{ display: 'block', fontSize: '0.68rem', color: 'var(--text-muted)', fontStyle: 'italic', marginTop: '0.35rem', lineHeight: '1.3' }}>
                          (Mục tiêu số 2: SV đạt bao nhiêu % điểm max)
                        </span>
                      </div>

                      {/* % SV CẦN ĐẠT/PI */}
                      <div>
                        <label style={{ display: 'block', fontSize: '0.72rem', fontWeight: 800, color: 'var(--text-secondary)', textTransform: 'uppercase', marginBottom: '0.35rem' }}>
                          % SV CẦN ĐẠT/PI
                        </label>
                        <input
                          required
                          type="number"
                          min="1"
                          max="100"
                          value={formThetaCoh}
                          onChange={(e) => setFormThetaCoh(Number(e.target.value))}
                          className="form-input"
                          style={{ textAlign: 'center', fontSize: '1.15rem', fontWeight: 800, padding: '0.5rem', borderRadius: '10px' }}
                        />
                        <span style={{ display: 'block', fontSize: '0.68rem', color: 'var(--text-muted)', fontStyle: 'italic', marginTop: '0.35rem', lineHeight: '1.3' }}>
                          (Mục tiêu số 1: % số SV trong đợt cần đạt PI)
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              {/* Action Buttons */}
              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '1rem', marginTop: '2rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1.25rem' }}>
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className="btn btn-secondary"
                  style={{ minWidth: '120px', padding: '0.75rem 1.5rem', fontWeight: 800, textTransform: 'uppercase', letterSpacing: '0.05em' }}
                >
                  HỦY
                </button>
                <button
                  type="submit"
                  className="btn btn-primary"
                  style={{ minWidth: '220px', padding: '0.75rem 1.75rem', fontWeight: 900, textTransform: 'uppercase', letterSpacing: '0.05em', backgroundColor: '#1e3a8a', borderColor: '#2563eb' }}
                >
                  TẠO ĐỢT ĐO LƯỜNG NGAY
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
