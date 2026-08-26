import React, { useState } from 'react';
import {
  ClipboardCheck,
  Calculator,
  Lock,
  Upload,
  CheckCircle,
  AlertCircle,
  Save,
  Users,
} from 'lucide-react';

interface StudentScoreRow {
  studentCode: string;
  fullName: string;
  crit1: number; // Max 10
  crit2: number; // Max 10
  crit3: number; // Max 10
  finalScore: number;
  piAttainment: 'MET' | 'NOT_MET';
}

export const MeasurementScoringPage: React.FC = () => {
  const [scores, setScores] = useState<StudentScoreRow[]>([
    { studentCode: '20230001', fullName: 'Nguyễn Văn An', crit1: 8.5, crit2: 9.0, crit3: 8.0, finalScore: 8.5, piAttainment: 'MET' },
    { studentCode: '20230002', fullName: 'Trần Thị Bình', crit1: 7.0, crit2: 7.5, crit3: 8.0, finalScore: 7.5, piAttainment: 'MET' },
    { studentCode: '20230003', fullName: 'Lê Hoàng Cường', crit1: 4.5, crit2: 5.0, crit3: 5.5, finalScore: 5.0, piAttainment: 'NOT_MET' },
    { studentCode: '20230004', fullName: 'Phạm Minh Đức', crit1: 9.0, crit2: 9.5, crit3: 9.0, finalScore: 9.2, piAttainment: 'MET' },
    { studentCode: '20230005', fullName: 'Vũ Thuỳ Giang', crit1: 8.0, crit2: 8.5, crit3: 8.5, finalScore: 8.3, piAttainment: 'MET' },
  ]);

  const handleScoreChange = (index: number, field: 'crit1' | 'crit2' | 'crit3', value: number) => {
    const updated = [...scores];
    updated[index][field] = value;
    const avg = (updated[index].crit1 + updated[index].crit2 + updated[index].crit3) / 3;
    updated[index].finalScore = Math.round(avg * 10) / 10;
    updated[index].piAttainment = updated[index].finalScore >= 6.0 ? 'MET' : 'NOT_MET';
    setScores(updated);
  };

  const metCount = scores.filter((s) => s.piAttainment === 'MET').length;
  const attainmentRate = Math.round((metCount / scores.length) * 100);

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Đợt Đo Lường & Bảng Chấm Điểm Rubric (Mục 8.4 & 8.5)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Nhập điểm theo từng tiêu chí Rubric của bài đánh giá trực tiếp; hệ thống tự động tính toán mức độ đạt CĐR thời gian thực.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn btn-secondary">
            <Upload size={16} />
            <span>Nhập Điểm Từ Excel</span>
          </button>
          <button className="btn btn-primary">
            <Lock size={16} />
            <span>Đóng Băng & Chốt Điểm Đợt Đo</span>
          </button>
        </div>
      </div>

      {/* Scope Selector Bar */}
      <div className="glass-card" style={{ marginBottom: '1.5rem', padding: '1rem 1.5rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: '1rem' }}>
          <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center' }}>
            <div>
              <span className="form-label">Đợt Đo Lường</span>
              <select className="form-select" style={{ width: '220px', marginTop: '0.25rem' }}>
                <option>HK1 (2023 - 2024) - Đợt Chính Thức</option>
              </select>
            </div>
            <div>
              <span className="form-label">Lớp Học Phần</span>
              <select className="form-select" style={{ width: '260px', marginTop: '0.25rem' }}>
                <option>IT4101_01 - Lập trình .NET (Thầy Nam)</option>
              </select>
            </div>
            <div>
              <span className="form-label">Bài Đánh Giá Trực Tiếp</span>
              <select className="form-select" style={{ width: '220px', marginTop: '0.25rem' }}>
                <option>A2: Bài Thực Hành (Đo PI 3.1 & PI 5.1)</option>
              </select>
            </div>
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', backgroundColor: 'var(--bg-surface-elevated)', padding: '0.5rem 1rem', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
            <div>
              <span style={{ fontSize: '0.7rem', color: 'var(--text-secondary)' }}>Tỷ lệ đạt chuẩn PI</span>
              <div style={{ fontSize: '1.25rem', fontWeight: 800, color: attainmentRate >= 80 ? 'var(--emerald-400)' : 'var(--amber-400)' }}>
                {attainmentRate}% ({metCount}/{scores.length} SV)
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Rubric Scoring Grid */}
      <div className="glass-card">
        <div className="glass-card-header">
          <div>
            <h3 className="glass-card-title">
              <ClipboardCheck size={20} className="text-primary-400" />
              Bảng Điểm Rubric Chi Tiết Từng Tiêu Chí (Bài A2)
            </h3>
            <p className="glass-card-subtitle">
              Ngưỡng cá nhân đạt chuẩn (θ_ind): <strong>≥ 6.0 / 10</strong>
            </p>
          </div>
          <button className="btn btn-sm btn-primary">
            <Save size={14} />
            <span>Lưu Tạm Bảng Điểm</span>
          </button>
        </div>

        <div className="table-container">
          <table className="data-table">
            <thead>
              <tr>
                <th>Mã SV</th>
                <th>Họ Và Tên Sinh Viên</th>
                <th style={{ textAlign: 'center' }}>Tiêu Chí 1 (REST API - 40%)</th>
                <th style={{ textAlign: 'center' }}>Tiêu Chí 2 (Database - 30%)</th>
                <th style={{ textAlign: 'center' }}>Tiêu Chí 3 (Unit Test - 30%)</th>
                <th style={{ textAlign: 'center' }}>Điểm Tổng Kết (Thang 10)</th>
                <th style={{ textAlign: 'center' }}>Kết Quả Đạt Chuẩn</th>
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
                  <td style={{ textAlign: 'center', fontWeight: 800, fontSize: '1rem', color: 'var(--text-primary)' }}>
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
      </div>
    </div>
  );
};
