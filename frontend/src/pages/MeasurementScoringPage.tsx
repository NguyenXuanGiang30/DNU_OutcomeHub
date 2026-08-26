import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  ClipboardCheck,
  Calculator,
  Lock,
  Upload,
  CheckCircle,
  AlertCircle,
  Save,
  Users,
  Calendar,
  Layers,
  UserCheck,
  FolderArchive,
  FileCheck2,
  RefreshCw,
  Plus,
  X,
} from 'lucide-react';

interface StudentScoreRow {
  studentCode: string;
  fullName: string;
  crit1: number;
  crit2: number;
  crit3: number;
  finalScore: number;
  piAttainment: 'MET' | 'NOT_MET';
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

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleTabClick = (key: string) => {
    setActiveTab(key);
    navigate(`/measurement/${key}`);
  };

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

  const handleSaveScores = () => {
    setToastMessage('✓ Đã lưu thành công điểm Rubric vào hệ thống!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleRunCalculation = () => {
    setToastMessage('✓ Động cơ tính toán OBE đã cập nhật xong kết quả đạt CĐR toàn trường!');
    setTimeout(() => setToastMessage(null), 3500);
  };

  const metCount = scores.filter((s) => s.piAttainment === 'MET').length;
  const attainmentRate = Math.round((metCount / scores.length) * 100);

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
            Đo Lường Chuẩn Đầu Ra
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Quản lý đợt đo, nguồn đo PI, phân công chấm, chấm Rubric theo tiêu chí, kiểm tra dữ liệu và tính toán CĐR.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button onClick={handleRunCalculation} className="btn btn-secondary">
            <Calculator size={16} />
            <span>Chạy Tính Toán CĐR</span>
          </button>
          <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
            <Plus size={16} />
            <span>+ Tạo Đợt Đo / Nguồn Đo</span>
          </button>
        </div>
      </div>

      {/* Navigation Sub-Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem', overflowX: 'auto' }}>
        {[
          { key: 'periods', label: '1. Đợt Đo Lường', icon: Calendar },
          { key: 'sources', label: '2. Nguồn Đo PI (A)', icon: Layers },
          { key: 'assignments', label: '3. Phân Công Giảng Viên', icon: UserCheck },
          { key: 'sync-grades', label: '4. Nhập – Đồng Bộ Điểm', icon: RefreshCw },
          { key: 'rubric-scoring', label: '5. Chấm Theo Rubric', icon: ClipboardCheck },
          { key: 'data-validation', label: '6. Kiểm Tra Dữ Liệu', icon: FileCheck2 },
          { key: 'calculation', label: '7. Tính Toán Kết Quả', icon: Calculator },
          { key: 'evidence', label: '8. Minh Chứng Đo Lường', icon: FolderArchive },
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

      {/* Scope Selector Bar */}
      <div className="glass-card" style={{ marginBottom: '1.25rem', padding: '0.875rem 1.25rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: '1rem' }}>
          <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center' }}>
            <div>
              <span className="form-label">Đợt Đo Lường</span>
              <select className="form-select" style={{ width: '240px', marginTop: '0.25rem' }}>
                <option>HK1 (2023 - 2024) - Đợt Đo Chính Thức</option>
              </select>
            </div>
            <div>
              <span className="form-label">Khóa & Ngành Áp Dụng</span>
              <select className="form-select" style={{ width: '240px', marginTop: '0.25rem' }}>
                <option>Khóa K17 - Ngành KTPM (v2023)</option>
                <option>Khóa K16 - Ngành KTPM (v2022)</option>
              </select>
            </div>
            <div>
              <span className="form-label">Lớp Học Phần</span>
              <select className="form-select" style={{ width: '260px', marginTop: '0.25rem' }}>
                <option>IT4101_01 - Lập trình .NET (TS. Lê Hải Nam)</option>
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

      {/* TAB 1: ĐỢT ĐO LƯỜNG (PERIODS) */}
      {activeTab === 'periods' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Sách Các Đợt Đo Lường Chuẩn Đầu Ra (Measurement Periods)</h3>
              <p className="glass-card-subtitle">Mỗi đợt đo liên kết với Khóa tuyển sinh, CTĐT và ngưỡng đánh giá cụ thể</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Tạo Đợt Đo Lường Mới</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Đợt Đo</th>
                  <th>Tên Đợt Đo</th>
                  <th>Năm Học & Học Kỳ</th>
                  <th>Khóa Đối Tượng</th>
                  <th>Ngưỡng Đạt Cá Nhân</th>
                  <th>Ngưỡng Khóa (Kỳ vọng)</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>PERIOD-2023-HK1</strong></td>
                  <td>Đo Lường CĐR Học Kỳ 1 (2023 - 2024)</td>
                  <td>2023-2024 (HK1)</td>
                  <td><span className="badge badge-primary">Khóa K17</span></td>
                  <td><code>θ_ind ≥ 6.0/10</code></td>
                  <td><code>θ_coh ≥ 80%</code></td>
                  <td><span className="badge badge-success">ĐANG THU THẬP ĐIỂM</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 2: NGUỒN ĐO PI (SOURCES) */}
      {activeTab === 'sources' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Nguồn Đo Trực Tiếp PI (A Sources)</h3>
              <p className="glass-card-subtitle">Các bài đánh giá và học phần được chỉ định đo CĐR trong học kỳ</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Nguồn Đo A</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Chỉ Số PI</th>
                  <th>Học Phần Đảm Nhận</th>
                  <th>Bài Đánh Giá</th>
                  <th>Phương Pháp Đánh Giá</th>
                  <th>Số Lớp Đo</th>
                  <th>Tỷ Trọng Trong PI</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong className="badge badge-cyan">PI 3.1</strong></td>
                  <td>IT4101: Lập trình .NET Nâng cao</td>
                  <td>Bài Thực Hành A2 & Đồ Án A3</td>
                  <td>Chấm Rubric Thực Hành & Demo</td>
                  <td>3 Lớp (120 SV)</td>
                  <td><strong>60.0%</strong></td>
                </tr>
                <tr>
                  <td><strong className="badge badge-cyan">PI 5.1</strong></td>
                  <td>IT4101: Lập trình .NET Nâng cao</td>
                  <td>Bài Thực Hành A2 (Unit Test)</td>
                  <td>Chấm Rubric Tự Động</td>
                  <td>3 Lớp (120 SV)</td>
                  <td><strong>50.0%</strong></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 3: PHÂN CÔNG GIẢNG VIÊN (ASSIGNMENTS) */}
      {activeTab === 'assignments' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Phân Công Giảng Viên Chấm Điểm Rubric Theo Lớp Học Phần</h3>
              <p className="glass-card-subtitle">Đảm bảo đúng thẩm quyền Scope chấm thi</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Phân Công Cán Bộ Chấm</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Lớp Học Phần</th>
                  <th>Tên Học Phần</th>
                  <th>Giảng Viên Giảng Dạy</th>
                  <th>Cán Bộ Chấm Rubric CĐR</th>
                  <th>Số Lượng SV</th>
                  <th>Tiến Độ Nhập Điểm</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><code>IT4101_01</code></td>
                  <td>Lập trình .NET Nâng cao</td>
                  <td>TS. Lê Hải Nam</td>
                  <td><strong>TS. Lê Hải Nam (GV001)</strong></td>
                  <td>40 SV</td>
                  <td><span className="badge badge-success">Đã nhập 40/40 (100%)</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 4: NHẬP - ĐỒNG BỘ ĐIỂM (SYNC-GRADES) */}
      {activeTab === 'sync-grades' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Nhập & Đồng Bộ Điểm Quá Trình Từ SIS / LMS</h3>
              <p className="glass-card-subtitle">Nạp điểm tự động qua API hoặc tải lên bảng điểm Excel chuẩn</p>
            </div>
            <button onClick={handleSaveScores} className="btn btn-sm btn-primary">Đồng Bộ LMS Canvas</button>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div style={{ padding: '1.5rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '2px dashed var(--border-strong)', textAlign: 'center' }}>
              <Upload size={36} style={{ color: 'var(--primary-400)', margin: '0 auto 0.5rem auto' }} />
              <h4 style={{ color: 'var(--text-primary)', marginBottom: '0.25rem' }}>Kéo thả file Excel bảng điểm vào đây hoặc nhấn để chọn file</h4>
              <p style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Hỗ trợ file .xlsx, .csv theo mẫu chuẩn BM13</p>
            </div>
          </div>
        </div>
      )}

      {/* TAB 5: CHẤM THEO RUBRIC (RUBRIC-SCORING) */}
      {activeTab === 'rubric-scoring' && (
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
            <button onClick={handleSaveScores} className="btn btn-sm btn-primary">
              <Save size={14} />
              <span>Lưu Điểm Vào Cơ Sở Dữ Liệu</span>
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
      )}

      {/* TAB 6: KIỂM TRA DỮ LIỆU */}
      {activeTab === 'data-validation' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Kiểm Tra Dữ Liệu Điểm Đo Lường Trước Khi Tính Toán</h3>
              <p className="glass-card-subtitle">Phát hiện điểm rỗng, vượt thang điểm 10 hoặc sinh viên thiếu bài thi</p>
            </div>
            <span className="badge badge-success">Dữ liệu hợp lệ 100% (0 Lỗi)</span>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
            <div style={{ padding: '1rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <div>
                <strong style={{ color: 'var(--text-primary)' }}>1. Kiểm tra tính đầy đủ của sinh viên</strong>
                <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>40/40 sinh viên đều có đủ điểm 3 tiêu chí Rubric</p>
              </div>
              <span className="badge badge-success">HỢP LỆ</span>
            </div>
          </div>
        </div>
      )}

      {/* TAB 7: TÍNH TOÁN KẾT QUẢ */}
      {activeTab === 'calculation' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Động Cơ Tính Toán Kết Quả Đạt CĐR (Calculation Engine)</h3>
              <p className="glass-card-subtitle">Kích hoạt động cơ tính điểm chuẩn đầu ra theo công thức chuẩn OBE</p>
            </div>
            <button onClick={handleRunCalculation} className="btn btn-primary">
              <Calculator size={16} />
              <span>Chạy Tính Toán CĐR Toàn Bộ Đợt Đo</span>
            </button>
          </div>

          <div style={{ padding: '1.5rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
            <h4 style={{ color: 'var(--emerald-400)', marginBottom: '0.5rem' }}>✓ Kết quả tính toán gần nhất:</h4>
            <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', display: 'flex', flexDirection: 'column', gap: '0.4rem' }}>
              <div>• Tổng số sinh viên đã tính: <strong>1,248 SV</strong></div>
              <div>• Tỷ lệ đạt PI 3.1: <strong style={{ color: 'var(--emerald-400)' }}>89.2% (Vượt ngưỡng 80%)</strong></div>
              <div>• Tỷ lệ đạt PI 5.1: <strong style={{ color: 'var(--amber-400)' }}>74.5% (Cần kích hoạt CQI)</strong></div>
            </div>
          </div>
        </div>
      )}

      {/* TAB 8: MINH CHỨNG */}
      {activeTab === 'evidence' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Minh Chứng Đo Lường & Bài Làm Sinh Viên (Evidence Archive)</h3>
              <p className="glass-card-subtitle">Lưu trữ bài làm mẫu (Giỏi, Khá, Yếu) có bảo vệ mã băm SHA-256</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Tải Lên Minh Chứng</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Tên Minh Chứng</th>
                  <th>Phân Loại Mẫu</th>
                  <th>Học Phần</th>
                  <th>Chỉ Số PI Đo Lường</th>
                  <th>Mã Băm Toàn Vẹn</th>
                  <th>Thao Tác</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>BaiLam_A2_MauGioi_20230004.pdf</strong></td>
                  <td><span className="badge badge-success">Mẫu Giỏi (Điểm 9.2)</span></td>
                  <td>IT4101 .NET</td>
                  <td>PI 3.1 & PI 5.1</td>
                  <td><code>a1b2...9f8e</code></td>
                  <td><button className="btn btn-sm btn-secondary">Xem</button></td>
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
                Tạo Mới / Cập Nhật: {activeTab === 'periods' ? 'Đợt Đo Lường' : activeTab === 'sources' ? 'Nguồn Đo PI' : 'Minh Chứng'}
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={(e) => { e.preventDefault(); setIsModalOpen(false); handleSaveScores(); }}>
              <div className="form-group">
                <label className="form-label">Tên Đợt Đo / Nguồn Đo</label>
                <input required type="text" placeholder="Nhập tên..." className="form-input" defaultValue="Đợt Đo CĐR Học Kỳ 2 (2023-2024)" />
              </div>

              <div className="form-group">
                <label className="form-label">Khóa Tuyển Sinh Đối Tượng</label>
                <select className="form-select">
                  <option>Khóa K17 (2023 - 2027)</option>
                  <option>Khóa K16 (2022 - 2026)</option>
                </select>
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
