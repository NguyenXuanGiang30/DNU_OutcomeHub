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
  semester: string;
  cohort: string;
  thresholdInd: string;
  thresholdCoh: string;
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
    return 'rubric-scoring';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  // Clean Dynamic States (Start with 0 items)
  const [scores, setScores] = useState<StudentScoreRow[]>([]);
  const [periods, setPeriods] = useState<MeasurementPeriodItem[]>([]);

  // Form Fields
  const [formPeriodName, setFormPeriodName] = useState('');
  const [formCohort, setFormCohort] = useState('Khóa K17');

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleScoreChange = (index: number, field: 'crit1' | 'crit2' | 'crit3', value: number) => {
    const updated = [...scores];
    updated[index][field] = value;
    const avg = (updated[index].crit1 + updated[index].crit2 + updated[index].crit3) / 3;
    updated[index].finalScore = Math.round(avg * 10) / 10;
    updated[index].piAttainment = updated[index].finalScore >= 6.0 ? 'MET' : 'NOT_MET';
    setScores(updated);
  };

  const handleSaveScores = () => {
    setToastMessage('✓ Đã lưu thành công điểm Rubric vào hệ thống!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleRunCalculation = () => {
    if (scores.length === 0) {
      setToastMessage('⚠ Chưa có dữ liệu bảng điểm để tính toán CĐR!');
      setTimeout(() => setToastMessage(null), 3000);
      return;
    }
    setToastMessage('✓ Động cơ tính toán OBE đã cập nhật xong kết quả!');
    setTimeout(() => setToastMessage(null), 3500);
  };

  const handleCreatePeriod = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formPeriodName.trim()) return;

    setPeriods([
      ...periods,
      {
        code: `PERIOD-${Date.now().toString().slice(-4)}`,
        name: formPeriodName,
        semester: '2023-2024 (HK1)',
        cohort: formCohort,
        thresholdInd: 'θ_ind ≥ 6.0/10',
        thresholdCoh: 'θ_coh ≥ 80%',
        status: 'ĐANG THU THẬP ĐIỂM',
      },
    ]);

    setFormPeriodName('');
    setIsModalOpen(false);
    setToastMessage('✓ Đã khởi tạo thành công Đợt đo lường CĐR mới!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleAddSampleStudent = () => {
    setScores([
      ...scores,
      {
        studentCode: `2023000${scores.length + 1}`,
        fullName: `Sinh viên Mới ${scores.length + 1}`,
        crit1: 8.0,
        crit2: 8.0,
        crit3: 8.0,
        finalScore: 8.0,
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
            {activeTab === 'periods' && 'Danh Sách Các Đợt Đo Lường'}
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
            <span>Thao Tác Thêm Mới</span>
          </button>
        </div>
      </div>

      {/* TAB 1: PERIODS */}
      {activeTab === 'periods' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Sách Các Đợt Đo Lường Chuẩn Đầu Ra</h3>
              <p className="glass-card-subtitle">Mỗi đợt đo liên kết với Khóa tuyển sinh và ngưỡng đánh giá</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Tạo Đợt Đo Mới</button>
          </div>

          {periods.length === 0 ? (
            <EmptyState
              title="Chưa có Đợt đo lường nào"
              description="Hiện tại chưa có đợt đo lường nào được kích hoạt. Nhấn nút bên dưới để khởi tạo đợt đo CĐR cho học kỳ."
              actionLabel="+ Khởi Tạo Đợt Đo Đầu Tiên"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã Đợt Đo</th>
                    <th>Tên Đợt Đo</th>
                    <th>Học Kỳ</th>
                    <th>Khóa Đối Tượng</th>
                    <th>Ngưỡng Cá Nhân</th>
                    <th>Trạng Thái</th>
                  </tr>
                </thead>
                <tbody>
                  {periods.map((p) => (
                    <tr key={p.code}>
                      <td><strong>{p.code}</strong></td>
                      <td>{p.name}</td>
                      <td>{p.semester}</td>
                      <td><span className="badge badge-primary">{p.cohort}</span></td>
                      <td><code>{p.thresholdInd}</code></td>
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
                Ngưỡng cá nhân đạt chuẩn (θ_ind): <strong>≥ 6.0 / 10</strong>
              </p>
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <button onClick={handleAddSampleStudent} className="btn btn-sm btn-secondary">+ Thêm Sinh Viên</button>
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
              description="Hiện tại lớp học phần này chưa có danh sách sinh viên chấm điểm. Nhấn nút bên dưới để thêm sinh viên hoặc tải lên bảng điểm."
              actionLabel="+ Thêm Sinh Viên Vào Bảng Điểm"
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

      {/* CREATE PERIOD MODAL */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '500px', maxWidth: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>Khởi Tạo Đợt Đo Lường Mới</h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleCreatePeriod}>
              <div className="form-group">
                <label className="form-label">Tên Đợt Đo Lường</label>
                <input required type="text" placeholder="Ví dụ: Đợt Đo CĐR Học Kỳ 1..." value={formPeriodName} onChange={(e) => setFormPeriodName(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Khóa Tuyển Sinh</label>
                <select value={formCohort} onChange={(e) => setFormCohort(e.target.value)} className="form-select">
                  <option>Khóa K17 (2023 - 2027)</option>
                  <option>Khóa K16 (2022 - 2026)</option>
                  <option>Khóa K18 (2024 - 2028)</option>
                </select>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Đợt Đo</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
