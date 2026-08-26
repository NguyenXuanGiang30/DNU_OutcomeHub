import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Network,
  Download,
  Plus,
  X,
  Save,
  Edit3,
  Check,
  CheckCircle,
} from 'lucide-react';
import { EmptyState } from '../components/common/EmptyState';

type MappingLevel = 'I' | 'R' | 'M' | 'A' | 'RA' | 'MA' | '-';

interface MatrixCell {
  courseCode: string;
  courseName: string;
  semester: number;
  plos: Record<string, MappingLevel>;
}

interface PloItem {
  code: string;
  scope: string;
  desc: string;
  bloom: string;
  po: string;
  target: string;
}

interface PiItem {
  pi: string;
  plo: string;
  desc: string;
  bloom: string;
  course: string;
}

interface PoItem {
  code: string;
  title: string;
  desc: string;
}

interface CloItem {
  code: string;
  desc: string;
  bloom: string;
  pi: string;
  assessment: string;
}

export const CurriculumMatrixPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/curriculum/programs')) return 'programs';
    if (location.pathname.includes('/curriculum/versions')) return 'versions';
    if (location.pathname.includes('/curriculum/pos')) return 'pos';
    if (location.pathname.includes('/curriculum/plos')) return 'plos';
    if (location.pathname.includes('/curriculum/pis')) return 'pis';
    if (location.pathname.includes('/curriculum/weight-a')) return 'weight-a';
    if (location.pathname.includes('/curriculum/clos')) return 'clos';
    return 'matrix';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isAddCourseModalOpen, setIsAddCourseModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);
  const [isEditingMatrix, setIsEditingMatrix] = useState<boolean>(false);

  // Clean Dynamic States (Start with 0 items)
  const [matrixData, setMatrixData] = useState<MatrixCell[]>([]);
  const [pos, setPos] = useState<PoItem[]>([]);
  const [plos, setPlos] = useState<PloItem[]>([]);
  const [pis, setPis] = useState<PiItem[]>([]);
  const [clos, setClos] = useState<CloItem[]>([]);

  // Form Fields
  const [formCode, setFormCode] = useState('');
  const [formDesc, setFormDesc] = useState('');
  const [formBloom, setFormBloom] = useState('Mức 3: Apply (Vận dụng)');

  const [newCourseCode, setNewCourseCode] = useState('');
  const [newCourseName, setNewCourseName] = useState('');
  const [newCourseSemester, setNewCourseSemester] = useState(1);

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleCellChange = (courseCode: string, plo: string, newLevel: MappingLevel) => {
    setMatrixData((prev) =>
      prev.map((row) => {
        if (row.courseCode === courseCode) {
          return {
            ...row,
            plos: {
              ...row.plos,
              [plo]: newLevel,
            },
          };
        }
        return row;
      })
    );
  };

  const handleSaveMatrix = () => {
    setIsEditingMatrix(false);
    setToastMessage('✓ Đã lưu thành công các thay đổi trong Ma trận liên kết CĐR!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleAddCourseToMatrix = (e: React.FormEvent) => {
    e.preventDefault();
    if (!newCourseCode.trim() || !newCourseName.trim()) return;

    const newRow: MatrixCell = {
      courseCode: newCourseCode,
      courseName: newCourseName,
      semester: Number(newCourseSemester),
      plos: { PLO1: 'I', PLO2: 'I', PLO3: 'I', PLO4: '-', PLO5: '-', PLO6: '-' },
    };
    setMatrixData([...matrixData, newRow]);
    setNewCourseCode('');
    setNewCourseName('');
    setIsAddCourseModalOpen(false);
    setToastMessage(`✓ Đã thêm học phần ${newCourseCode} vào ma trận liên kết!`);
    setTimeout(() => setToastMessage(null), 3000);
  };

  const handleSaveModal = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formCode.trim() || !formDesc.trim()) return;

    if (activeTab === 'pos') {
      setPos([...pos, { code: formCode, title: formDesc, desc: formDesc }]);
    } else if (activeTab === 'plos') {
      setPlos([...plos, { code: formCode, scope: 'CẤP NGÀNH', desc: formDesc, bloom: formBloom, po: 'PO1', target: '80%' }]);
    } else if (activeTab === 'pis') {
      setPis([...pis, { pi: formCode, plo: 'PLO1', desc: formDesc, bloom: formBloom, course: 'Chưa chỉ định' }]);
    } else if (activeTab === 'clos') {
      setClos([...clos, { code: formCode, desc: formDesc, bloom: formBloom, pi: 'PI 1.1', assessment: 'Chưa gán' }]);
    }

    setFormCode('');
    setFormDesc('');
    setIsModalOpen(false);
    setToastMessage('✓ Đã lưu thành công bản ghi mới vào hệ thống!');
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

      {/* Header Banner */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <div style={{ fontSize: '0.75rem', color: 'var(--primary-400)', fontWeight: 700, textTransform: 'uppercase', marginBottom: '0.25rem' }}>
            Chương Trình & Chuẩn Đầu Ra
          </div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            {activeTab === 'matrix' && 'Ma Trận Liên Kết CĐR (I/R/M/A)'}
            {activeTab === 'programs' && 'Chương Trình Đào Tạo Trực Thuộc'}
            {activeTab === 'versions' && 'Quản Lý Các Phiên Bản CTĐT'}
            {activeTab === 'pos' && 'Mục Tiêu Đào Tạo (Program Objectives - PO)'}
            {activeTab === 'plos' && 'Chuẩn Đầu Ra Chương Trình (PLO1 – PLO9)'}
            {activeTab === 'pis' && 'Chỉ Báo Thực Hiện (Performance Indicators - PI)'}
            {activeTab === 'weight-a' && 'Trọng Số Đo Trực Tiếp A Theo CTĐT'}
            {activeTab === 'clos' && 'Chuẩn Đầu Ra Học Phần (CLO)'}
          </h2>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          {activeTab === 'matrix' && (
            <>
              {isEditingMatrix ? (
                <button onClick={handleSaveMatrix} className="btn btn-primary" style={{ backgroundColor: 'var(--emerald-600)' }}>
                  <Save size={16} />
                  <span>Lưu Ma Trận</span>
                </button>
              ) : (
                <button onClick={() => setIsEditingMatrix(true)} className="btn btn-secondary">
                  <Edit3 size={16} />
                  <span>Chỉnh Sửa Ma Trận</span>
                </button>
              )}
              <button onClick={() => setIsAddCourseModalOpen(true)} className="btn btn-primary">
                <Plus size={16} />
                <span>+ Thêm Học Phần Vào Ma Trận</span>
              </button>
            </>
          )}

          {activeTab !== 'matrix' && (
            <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
              <Plus size={16} />
              <span>+ Thêm Mới Dữ Liệu</span>
            </button>
          )}

          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Excel</span>
          </button>
        </div>
      </div>

      {/* Scope Selector Bar */}
      <div className="glass-card" style={{ marginBottom: '1.25rem', padding: '0.875rem 1.25rem' }}>
        <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center', flexWrap: 'wrap' }}>
          <div>
            <span className="form-label">Ngành Đào Tạo</span>
            <select className="form-select" style={{ width: '220px', marginTop: '0.25rem' }}>
              <option>Kỹ thuật Phần mềm (7480201)</option>
              <option>Khoa học Máy tính (7480101)</option>
            </select>
          </div>
          <div>
            <span className="form-label">Phiên Bản CTĐT</span>
            <select className="form-select" style={{ width: '240px', marginTop: '0.25rem' }}>
              <option>KTPM v2023 (9 PLO ABET)</option>
              <option>KTPM v2022 (6 PLO)</option>
            </select>
          </div>
        </div>
      </div>

      {/* TAB 1: MA TRẬN 2D (I/R/M/A) */}
      {activeTab === 'matrix' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Ma Trận Đóng Góp CĐR Của Các Học Phần (I/R/M/A)</h3>
              <p className="glass-card-subtitle">
                Quy ước: <strong>I</strong> = Introduce, <strong>R</strong> = Reinforce, <strong>M</strong> = Master, <strong>A</strong> = Direct Assessment (Đo trực tiếp)
              </p>
            </div>
            <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
              <span className="badge badge-primary">I: Giới thiệu</span>
              <span className="badge badge-cyan">R: Củng cố</span>
              <span className="badge badge-success">M: Thuần thục</span>
              <span className="badge badge-danger">A: Đo trực tiếp</span>
            </div>
          </div>

          {matrixData.length === 0 ? (
            <EmptyState
              title="Chưa có Học phần nào trong Ma trận"
              description="Hiện tại ma trận CĐR chưa có học phần nào. Nhấn nút bên dưới để thêm môn học và thiết lập mức độ đóng góp I/R/M/A."
              actionLabel="+ Thêm Học Phần Đầu Tiên Vào Ma Trận"
              onAction={() => setIsAddCourseModalOpen(true)}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã Môn</th>
                    <th>Tên Học Phần</th>
                    <th style={{ width: '90px' }}>Học Kỳ</th>
                    <th style={{ textAlign: 'center' }}>PLO1</th>
                    <th style={{ textAlign: 'center' }}>PLO2</th>
                    <th style={{ textAlign: 'center' }}>PLO3</th>
                    <th style={{ textAlign: 'center' }}>PLO4</th>
                    <th style={{ textAlign: 'center' }}>PLO5</th>
                    <th style={{ textAlign: 'center' }}>PLO6</th>
                  </tr>
                </thead>
                <tbody>
                  {matrixData.map((row) => (
                    <tr key={row.courseCode}>
                      <td><strong className="badge badge-secondary">{row.courseCode}</strong></td>
                      <td style={{ fontWeight: 600 }}>{row.courseName}</td>
                      <td>HK {row.semester}</td>
                      {['PLO1', 'PLO2', 'PLO3', 'PLO4', 'PLO5', 'PLO6'].map((plo) => {
                        const val = (row.plos[plo] || '-') as MappingLevel;
                        let badgeClass = 'badge-secondary';
                        if (val.includes('A')) badgeClass = 'badge-danger';
                        else if (val === 'M') badgeClass = 'badge-success';
                        else if (val === 'R') badgeClass = 'badge-cyan';
                        else if (val === 'I') badgeClass = 'badge-primary';

                        return (
                          <td key={plo} style={{ textAlign: 'center' }}>
                            {isEditingMatrix ? (
                              <select
                                value={val}
                                onChange={(e) => handleCellChange(row.courseCode, plo, e.target.value as MappingLevel)}
                                className="form-select"
                                style={{ width: '76px', padding: '0.2rem 0.4rem', fontSize: '0.8rem', fontWeight: 800, textAlign: 'center' }}
                              >
                                <option value="-">-</option>
                                <option value="I">I</option>
                                <option value="R">R</option>
                                <option value="M">M</option>
                                <option value="A">A</option>
                                <option value="RA">RA</option>
                                <option value="MA">MA</option>
                              </select>
                            ) : (
                              <span className={`badge ${badgeClass}`} style={{ minWidth: '38px', justifyContent: 'center', fontWeight: 800 }}>
                                {val}
                              </span>
                            )}
                          </td>
                        );
                      })}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* TAB 4: MỤC TIÊU ĐÀO TẠO (PO) */}
      {activeTab === 'pos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Mục Tiêu Đào Tạo (Program Objectives - PO)</h3>
              <p className="glass-card-subtitle">Định hướng năng lực sinh viên sau 3 - 5 năm tốt nghiệp</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Mục Tiêu PO</button>
          </div>

          {pos.length === 0 ? (
            <EmptyState
              title="Chưa có Mục tiêu đào tạo (PO)"
              description="Hiện tại chưa có mục tiêu đào tạo nào. Nhấn nút bên dưới để tạo mục tiêu PO mới."
              actionLabel="+ Thêm Mục Tiêu PO"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              {pos.map((po) => (
                <div key={po.code} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                  <strong style={{ color: 'var(--primary-400)', fontSize: '1rem' }}>{po.code}: {po.title}</strong>
                  <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', marginTop: '0.35rem' }}>{po.desc}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* TAB 5: CHUẨN ĐẦU RA (PLO1 - PLO9) */}
      {activeTab === 'plos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Hệ Thống Chuẩn Đầu Ra (PLO)</h3>
              <p className="glass-card-subtitle">Khung chuẩn đầu ra chương trình đào tạo</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm PLO</button>
          </div>

          {plos.length === 0 ? (
            <EmptyState
              title="Chưa có Chuẩn đầu ra (PLO)"
              description="Hiện tại chưa có chuẩn đầu ra nào được định nghĩa cho CTĐT này. Nhấn nút bên dưới để thêm mới."
              actionLabel="+ Thêm Chuẩn Đầu Ra PLO"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã PLO</th>
                    <th>Phân Cấp</th>
                    <th>Mô Tả Năng Lực Chuẩn Đầu Ra</th>
                    <th>Mức Bloom</th>
                    <th>Liên Kết PO</th>
                    <th>Ngưỡng Đạt</th>
                  </tr>
                </thead>
                <tbody>
                  {plos.map((plo) => (
                    <tr key={plo.code}>
                      <td><strong className="badge badge-primary">{plo.code}</strong></td>
                      <td><span className="badge badge-secondary">{plo.scope}</span></td>
                      <td style={{ fontWeight: 600 }}>{plo.desc}</td>
                      <td><span className="badge badge-bloom badge-cyan">{plo.bloom}</span></td>
                      <td><code>{plo.po}</code></td>
                      <td style={{ color: 'var(--emerald-400)', fontWeight: 700 }}>{plo.target}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* TAB 6: CHỈ BÁO THỰC HIỆN (PI) */}
      {activeTab === 'pis' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Chỉ Báo Thực Hiện (PI)</h3>
              <p className="glass-card-subtitle">Phân rã chỉ số hành vi đo lường từ PLO</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm PI</button>
          </div>

          {pis.length === 0 ? (
            <EmptyState
              title="Chưa có Chỉ báo thực hiện (PI)"
              description="Hiện tại chưa có chỉ báo PI nào. Nhấn nút bên dưới để thêm chỉ báo đo lường mới."
              actionLabel="+ Thêm Chỉ Báo PI"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã PI</th>
                    <th>Thuộc PLO</th>
                    <th>Mô Tả Hành Vi Đo Lường</th>
                    <th>Mức Bloom</th>
                    <th>Học Phần Đảm Nhận Đo (A)</th>
                  </tr>
                </thead>
                <tbody>
                  {pis.map((item) => (
                    <tr key={item.pi}>
                      <td><strong className="badge badge-cyan">{item.pi}</strong></td>
                      <td><code>{item.plo}</code></td>
                      <td style={{ fontWeight: 600 }}>{item.desc}</td>
                      <td><span className="badge badge-bloom badge-secondary">{item.bloom}</span></td>
                      <td><span className="badge badge-danger">{item.course}</span></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* TAB 8: CLO */}
      {activeTab === 'clos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Chuẩn Đầu Ra Học Phần (CLO)</h3>
              <p className="glass-card-subtitle">Ánh xạ từ CLO môn học lên chỉ số PI</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm CLO</button>
          </div>

          {clos.length === 0 ? (
            <EmptyState
              title="Chưa có Chuẩn đầu ra học phần (CLO)"
              description="Hiện tại chưa có CLO nào trong danh mục. Nhấn nút bên dưới để thêm CLO môn học."
              actionLabel="+ Thêm Chuẩn Đầu Ra CLO"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã CLO</th>
                    <th>Mô Tả Chuẩn Đầu Ra Môn Học</th>
                    <th>Mức Bloom</th>
                    <th>Ánh Xạ Tới Chỉ Số PI</th>
                    <th>Bài Đánh Giá</th>
                  </tr>
                </thead>
                <tbody>
                  {clos.map((c) => (
                    <tr key={c.code}>
                      <td><strong className="badge badge-cyan">{c.code}</strong></td>
                      <td>{c.desc}</td>
                      <td><span className="badge badge-bloom badge-cyan">{c.bloom}</span></td>
                      <td><strong>{c.pi}</strong></td>
                      <td>{c.assessment}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* ADD COURSE MODAL */}
      {isAddCourseModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '500px', maxWidth: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>Thêm Học Phần Vào Ma Trận CĐR</h3>
              <button onClick={() => setIsAddCourseModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleAddCourseToMatrix}>
              <div className="form-group">
                <label className="form-label">Mã Học Phần</label>
                <input required type="text" placeholder="Ví dụ: IT4101..." value={newCourseCode} onChange={(e) => setNewCourseCode(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Tên Học Phần</label>
                <input required type="text" placeholder="Nhập tên môn học..." value={newCourseName} onChange={(e) => setNewCourseName(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Học Kỳ Trong CTĐT (1 - 8)</label>
                <input required type="number" min="1" max="8" value={newCourseSemester} onChange={(e) => setNewCourseSemester(Number(e.target.value))} className="form-input" />
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsAddCourseModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Plus size={16} /><span>Thêm Vào Ma Trận</span></button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* CREATE MODAL */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '540px', maxWidth: '100%', maxHeight: '90vh', overflowY: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>
                Thêm Mới: {activeTab === 'pos' ? 'Mục Tiêu PO' : activeTab === 'plos' ? 'Chuẩn Đầu Ra PLO' : activeTab === 'pis' ? 'Chỉ Báo PI' : 'Chuẩn Đầu Ra CLO'}
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveModal}>
              <div className="form-group">
                <label className="form-label">Mã Định Danh (Code)</label>
                <input required type="text" placeholder="Ví dụ: PO1, PLO1, PI 1.1, CLO1..." value={formCode} onChange={(e) => setFormCode(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Mô Tả Năng Lực</label>
                <textarea required rows={3} placeholder="Nhập nội dung mô tả..." value={formDesc} onChange={(e) => setFormDesc(e.target.value)} className="form-textarea" />
              </div>

              <div className="form-group">
                <label className="form-label">Bậc Năng Lực Bloom</label>
                <select value={formBloom} onChange={(e) => setFormBloom(e.target.value)} className="form-select">
                  <option>Mức 1: Remember (Nhận biết)</option>
                  <option>Mức 2: Understand (Thông hiểu)</option>
                  <option>Mức 3: Apply (Vận dụng)</option>
                  <option>Mức 4: Analyze (Phân tích)</option>
                  <option>Mức 5: Evaluate (Đánh giá)</option>
                  <option>Mức 6: Create (Sáng tạo)</option>
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
