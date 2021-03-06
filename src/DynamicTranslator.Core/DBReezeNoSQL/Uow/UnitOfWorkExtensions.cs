﻿namespace DynamicTranslator.Core.DBReezeNoSQL.Uow
{
    #region using

    using System;
    using DBreeze.Transactions;
    using Domain.Uow;

    #endregion

    public static class UnitOfWorkExtensions
    {
        public static Transaction GetTransaction(this IActiveUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            if (!(unitOfWork is DbReezeUnitOfWork))
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof (DbReezeUnitOfWork).FullName, nameof(unitOfWork));
            }

            return ((DbReezeUnitOfWork) unitOfWork).Transaction;
        }
    }
}